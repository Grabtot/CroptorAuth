namespace Croptor.Application.Orders.Queries.CreateCallbackResponse;

public record WayForPayCallbackResponse
{
    public Guid OrderReference { get; init; }
    public string Status { get; init; }
    public long Time { get; init; } = 2;
    public string Signature { get; set; } = null!;

    public WayForPayCallbackResponse(Guid orderId, string status = "accept")
    {
        OrderReference = orderId;
        Status = status;
    }

    public override string ToString()
    {
        return $"OrderReference: {OrderReference} " +
            $"Status: {Status} " +
            $"Time: {Time} " +
            $"Signature: {Signature}";
    }
};