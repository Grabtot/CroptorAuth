﻿using CroptorAuth.Models;

namespace Croptor.Application.Orders.Queries.CreateCallbackResponse;

public record WayForPayCallbackResponse
{
    public Guid OrderReference { get; init; }
    public string Status { get; init; } = "accept";
    public long Time { get; init; } = 1;
    public string Signature { get; set; } = null!;

    public WayForPayCallbackResponse(Order order)
    {
        OrderReference = order.Id;
    }
};