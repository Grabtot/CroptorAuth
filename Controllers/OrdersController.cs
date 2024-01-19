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

    [HttpPost("callback")]
    public async Task<ActionResult<WayForPayCallbackResponse>> Callback()
    {
        
        string data;
        using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
        {
            data = await reader.ReadToEndAsync();
        }
        var queryString = HttpContext.Request.Query;

        string queryStringAsString = queryString.Count > 0
            ? "?" + string.Join("&", queryString.Select(kv => $"{kv.Key}={kv.Value}"))
            : string.Empty;
        
        var formData = HttpContext.Request.Form;

        // Convert the form data to a string
        string formDataAsString = formData.Count > 0
            ? string.Join("&", formData.Select(kv => $"{kv.Key}={kv.Value}"))
            : string.Empty;
        
        Log.Information($"" +
                        $"ContentType: {HttpContext.Request.ContentType}," +
                        $"Body: {data}, " +
                        $"QueryString: {queryStringAsString}" +
                        $"FormData: {formDataAsString}");

        return StatusCode(500, "not implemented");

        // WayForPayCallback? callback = JsonConvert.DeserializeObject<WayForPayCallback>(jsonData);
        //
        // if (callback is { TransactionStatus: "Approved" })
        // {
        //     string? keyString = configuration["WayForPay:Key"];
        //     if (keyString == null)
        //         return BadRequest("SecretKey is null");
        //     string? account = configuration["WayForPay:MerchantLogin"];
        //     if (account == null)
        //         return BadRequest("MerchantLogin is null");
        //
        //     Order order = await service.GetOrder(callback.OrderReference);
        //
        //     WayForPayRequest request = service.CreateRequest(order, account, keyString);
        //
        //     if (request.MerchantSignature == callback.MerchantSignature)
        //     {
        //         WayForPayCallbackResponse response = service.CreateCallbackResponse(order, keyString);
        //         await service.ApproveOrder(order);
        //         return Ok(response);
        //     }
        //     else
        //         return BadRequest("Signatures aren't the same");
        // }
        // else
        //     return BadRequest("TransactionStatus must equal \"Approved\"");
    }
}