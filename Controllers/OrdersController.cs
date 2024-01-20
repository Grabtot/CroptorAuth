using Croptor.Api.ViewModels.Order;
using Croptor.Application.Orders.Queries.CreateCallbackResponse;
using Croptor.Application.Orders.Queries.CreateRequest;
using CroptorAuth.Models;
using CroptorAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace CroptorAuth.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController(WayForPayService service, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}")]
    public async Task<ActionResult<WayForPayRequest>> Create(int amount)
    {
        string? keyString = configuration["WayForPay:Key"];
        if (keyString == null)
            return BadRequest("SecretKey is null");
        string? account = configuration["WayForPay:MerchantLogin"];
        if (account == null)
            return BadRequest("MerchantLogin is null");

        Order order = await service.CreateOrder(amount);

        WayForPayRequest request = service.CreateRequest(order, account, keyString);

        return Ok(request);
    }

    [Route("callback")]
    public async Task<ActionResult<WayForPayCallbackResponse>> Callback()
    {
        string body;
        try
        {
            using StreamReader reader = new(HttpContext.Request.Body);
            body = await reader.ReadToEndAsync();
        }
        catch (Exception)
        {
            return BadRequest("There is no body");
        }
        Log.Debug($"Body: {body}");
        WayForPayCallback? callback = JsonConvert.DeserializeObject<WayForPayCallback>(body);

        if (callback is { TransactionStatus: "Approved" })
        {
            string? keyString = configuration["WayForPay:Key"];
            if (keyString == null)
            {
                Log.Error("SecretKey is null");
                return BadRequest("SecretKey is null");
            }
            string? account = configuration["WayForPay:MerchantLogin"];
            if (account == null)
            {
                Log.Error("MerchantLogin is null");
                return BadRequest("MerchantLogin is null");
            }

            Order order = await service.GetOrder(callback.OrderReference);

            WayForPayRequest request = service.CreateRequest(order, account, keyString);
            Log.Debug($"Request: {request}");
            Log.Debug($"Callback: {callback}");

            if (request.MerchantSignature == callback.MerchantSignature)
            {
                WayForPayCallbackResponse response = service.CreateCallbackResponse(order, keyString);
                await service.ApproveOrder(order);
                return Ok(response);
            }
            else
            {
                Log.Error("Signatures aren't the same");
                return BadRequest("Signatures aren't the same");
            }
        }
        else
        {
            Log.Error("TransactionStatus must equal \"Approved\"");
            return BadRequest("TransactionStatus must equal \"Approved\"");
        }
    }
}