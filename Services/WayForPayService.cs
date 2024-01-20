using Croptor.Application.Orders.Queries.CreateCallbackResponse;
using Croptor.Application.Orders.Queries.CreateRequest;
using Croptor.Infrastructure.Persistence.Repositories;
using CroptorAuth.Models;
using System.Security.Cryptography;
using System.Text;

namespace CroptorAuth.Services;

public class WayForPayService(
    UserRepository userRepository,
    UserProvider userProvider,
    OrderRepository orderRepository)
{
    public async Task<Order> CreateOrder(int amount)
    {
        if (userProvider.UserId is null)
        {
            throw new UserNotAuthenticatedException();
        }

        Guid userId = userProvider.UserId.Value;
        Order order = Order.Create(userId, amount);

        await orderRepository.AddAsync(order);


        return order;
    }

    public async Task ApproveOrder(Order order)
    {
        ApplicationUser user = await userRepository.GetUserAsync(order.UserId);
        DateOnly expireDate;
        if (user.Plan.ExpireDate.HasValue)
            expireDate = user.Plan.ExpireDate.Value;
        else
            expireDate = DateOnly.FromDateTime(DateTime.Now);
        expireDate = expireDate.AddMonths(order.Amount);
        user.Plan = Plan.Create(PlanType.Pro, expireDate);
        // await userManager.ReplaceClaimAsync(user, new Claim("plan", "Free"), new Claim("plan", "Pro"));
        await orderRepository.DeleteOrderAsync(order);
    }

    public WayForPayRequest CreateRequest(Order order, string account, string secretKey)
    {
        WayForPayRequest wfpr = new WayForPayRequest(order, account);

        wfpr.MerchantSignature = HashParams([
            wfpr.MerchantAccount,
            wfpr.MerchantDomainName,
            wfpr.OrderReference,
            wfpr.OrderDate.ToString(),
            wfpr.Amount.ToString(),
            wfpr.Currency,
            wfpr.ProductName,
            wfpr.ProductCount.ToString(),
            wfpr.ProductPrice.ToString()
        ], secretKey);

        return wfpr;
    }
    public WayForPayCallbackResponse CreateCallbackResponse(Order order, string secretKey)
    {
        WayForPayCallbackResponse wfpcr = new WayForPayCallbackResponse(order);

        wfpcr.Signature = HashParams([
            wfpcr.OrderReference.ToString(),
            wfpcr.Status,
            wfpcr.Time.ToString()
        ], secretKey);

        return wfpcr;
    }

    public string HashParams(List<string> data, string keyString)
    {
        byte[] source = Encoding.UTF8.GetBytes(String.Join(";", data));
        byte[] key = Encoding.UTF8.GetBytes(keyString);
        return BitConverter.ToString(HMACMD5.HashData(key, source)).Replace("-", "").ToLower();
    }
    private class UserNotAuthenticatedException : Exception
    {
        public override string Message => "Not authenticated user cant do this";
    }
    public Task<Order> GetOrder(Guid id)
    {
        return orderRepository.GetOrderAsync(id);
    }
}