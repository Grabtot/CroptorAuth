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

        string? keyString = configuration["WayForPay:Key"];
        if (keyString == null)
        {
            Log.Error("SecretKey is null");
            return Problem(statusCode: 500);
        }

        if (callback is { TransactionStatus: "Approved" })
        {
            string? account = configuration["WayForPay:MerchantLogin"];
            if (account == null)
            {
                Log.Error("MerchantLogin is null");
                return Problem(statusCode: 500);
            }

            Log.Debug($"Callback: {callback}");
            string signature = service.HashParams([
                callback.MerchantAccount,
                callback.OrderReference.ToString(),
                callback.Amount.ToString(),
                callback.Currency,
                callback.AuthCode,
                callback.CardPan,
                callback.TransactionStatus,
                callback.ReasonCode
            ], keyString);
            Log.Debug(signature);

            if (callback.MerchantSignature == signature)
            {
                Order order = await service.GetOrder(callback.OrderReference);
                WayForPayCallbackResponse success = service.CreateCallbackResponse(order.Id, keyString);
                await service.ApproveOrder(order);

                Log.Debug($"Callback response: {success}");
                return Ok(success);
            }

            Log.Error("Signatures aren't the same");
            return Refuse(service, callback, keyString);
        }


        Log.Error("TransactionStatus must equal \"Approved\"");
        return Refuse(service, callback!, keyString);
    }

    private ActionResult<WayForPayCallbackResponse> Refuse(WayForPayService service, WayForPayCallback callback, string keyString)
    {

        WayForPayCallbackResponse response = service
            .CreateCallbackResponse(callback.OrderReference, keyString, "refuse");

        Log.Debug($"Callback response: {response}");
        return Ok(response);
    }
}