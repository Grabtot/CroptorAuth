using Croptor.Api.ViewModels.Order;
using Croptor.Application.Orders.Queries.CreateCallbackResponse;
using Croptor.Application.Orders.Queries.CreateRequest;
using CroptorAuth.Models;
using CroptorAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CroptorAuth.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController(WayForPayService service, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<WayForPayRequest>> Create([FromQuery] int amount)
    {
        var keyString = configuration["WayForPay:Key"];
        if (keyString == null) return BadRequest("SecretKey is null");
        var account = configuration["WayForPay:MerchantLogin"];
        if (account == null) return BadRequest("MerchantLogin is null");

        Order order = await service.CreateOrder(amount);

        var request = service.CreateRequest(order, account, keyString);

        return Ok(request);
    }

    [HttpPost("callback")]
    public async Task<ActionResult<WayForPayCallbackResponse>> Callback(WayForPayCallback callback)
    {
        if (callback.TransactionStatus == "Approved")
        {
            var keyString = configuration["WayForPay:Key"];
            if (keyString == null) return BadRequest("SecretKey is null");
            var account = configuration["WayForPay:MerchantLogin"];
            if (account == null) return BadRequest("MerchantLogin is null");

            Order order = await service.GetOrder(callback.OrderReference);

            var request = service.CreateRequest(order, account, keyString);

            if (request.MerchantSignature == callback.MerchantSignature)
            {
                var response = service.CreateCallbackResponse(order,keyString);
                await service.ApproveOrder(order);
                return Ok(response);
            }
            else return BadRequest("Signatures aren't the same");
        }
        else return BadRequest("TransactionStatus must equal \"Approved\"");
    }
}