using CroptorAuth.Models;

namespace Croptor.Application.Orders.Queries.CreateRequest;

public record WayForPayRequest
{
    public string MerchantAccount { get; init; }
    public string MerchantDomainName { get; init; } = "croptor.com";
    public string MerchantSignature { get; set; } = null!;
    public string ReturnUrl { get; init; } = "https://croptor.com";
    public string ServiceUrl { get; init; } = "https://croptor.com/orders/callback";
    public string OrderReference { get; init; }
    public long OrderDate { get; init; } = 1;
    public int Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public int ProductCount { get; init; }
    public string ProductName { get; init; } = "ProPlan";
    public int ProductPrice { get; init; }
    public string ClientAccountId { get; init; }

    public WayForPayRequest(Order order, string account)
    {
        MerchantAccount = account;
        OrderReference = order.Id.ToString();
        ProductPrice = 9;
        ProductCount = order.Amount;
        Amount = ProductPrice * ProductCount;
        ClientAccountId = order.UserId.ToString();
    }

    public WayForPayRequest()
    {

    }
    public override string ToString()
    {
        return $"MerchantAccount: {MerchantAccount}, " +
               $"MerchantDomainName: {MerchantDomainName}, " +
               $"MerchantSignature: {MerchantSignature}, " +
               $"ReturnUrl: {ReturnUrl}, " +
               $"ServiceUrl: {ServiceUrl}, " +
               $"OrderReference: {OrderReference}, " +
               $"OrderDate: {OrderDate}, " +
               $"Amount: {Amount}, " +
               $"Currency: {Currency}, " +
               $"ProductCount: {ProductCount}, " +
               $"ProductName: {ProductName}, " +
               $"ProductPrice: {ProductPrice}, " +
               $"ClientAccountId: {ClientAccountId}";
    }
}