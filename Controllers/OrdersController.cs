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
            using var reader = new StreamReader(HttpContext.Request.Body);
            body = await reader.ReadToEndAsync();
        }
        catch (Exception e)
        {
            body = "Can't get";
        }

        string headersAsString;
        try
        {
            IHeaderDictionary headers = HttpContext.Request.Headers;
        
            // Convert the headers to a string
            headersAsString = headers.Count > 0
                ? string.Join(Environment.NewLine, headers.Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value)}"))
                : string.Empty;
        }
        catch (Exception _)
        {
            headersAsString = "Can't get";
        }

        string queryStringAsString;
        try
        {
            var queryString = HttpContext.Request.Query;

            queryStringAsString = queryString.Count > 0
                ? "?" + string.Join("&", queryString.Select(kv => $"{kv.Key}={kv.Value}"))
                : string.Empty;
        }
        catch (Exception _)
        {
            queryStringAsString = "Can't get";
        }

        string formDataAsString;
        try
        {
            var formData = HttpContext.Request.Form;

            // Convert the form data to a string
            formDataAsString = formData.Count > 0
                ? string.Join("&", formData.Select(kv => $"{kv.Key}={kv.Value}"))
                : string.Empty;
        }
        catch (Exception _)
        {
            formDataAsString = "Can't get";
        }
        
        
        Log.Information($"" +
                        $"Method: {HttpContext.Request.Method}\n" +
                        $"Headers: \n{headersAsString}\n" +
                        $"\n\nContentType: {HttpContext.Request.ContentType}\n" +
                        $"Body: {body}\n" +
                        $"QueryString: {queryStringAsString}\n" +
                        $"FormData: {formDataAsString}\n");

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