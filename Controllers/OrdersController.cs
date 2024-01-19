﻿using Croptor.Api.ViewModels.Order;
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

    [HttpPost("callback")]
    [Consumes("text/plain")]
    public async Task<ActionResult<WayForPayCallbackResponse>> Callback([FromBody] string jsonData)
    {
        WayForPayCallback? callback = JsonConvert.DeserializeObject<WayForPayCallback>(jsonData);

        if (callback is { TransactionStatus: "Approved" })
        {
            string? keyString = configuration["WayForPay:Key"];
            if (keyString == null)
                return BadRequest("SecretKey is null");
            string? account = configuration["WayForPay:MerchantLogin"];
            if (account == null)
                return BadRequest("MerchantLogin is null");

            Order order = await service.GetOrder(callback.OrderReference);

            WayForPayRequest request = service.CreateRequest(order, account, keyString);

            Log.Information($"Request signature: {request.MerchantSignature}, Callback signature {callback.MerchantSignature}," +
                $" {request.MerchantSignature == callback.MerchantSignature}");
            if (request.MerchantSignature == callback.MerchantSignature)
            {
                WayForPayCallbackResponse response = service.CreateCallbackResponse(order, keyString);
                await service.ApproveOrder(order);
                return Ok(response);
            }
            else
                return BadRequest("Signatures aren't the same");
        }
        else
        {
            Log.Error($"callback fail. Reason {callback.Reason}");
            return BadRequest("TransactionStatus must equal \"Approved\"");
        }
    }
}