namespace Croptor.Api.ViewModels.Order;

public record WayForPayCallback
{
    public string Reason { get; set; }
    public string MerchantSignature { get; set; }
    
    
    public string MerchantAccount { get; set; }
    public Guid OrderReference { get; set; }
    public int Amount { get; set; }
    public string Currency { get; set; }
    public string AuthCode { get; set; }
    public string CardPan { get; set; }
    public string TransactionStatus { get; set; }
    public string ReasonCode { get; set; }
    
    public override string ToString()
    {
        return $"MerchantAccount: {MerchantAccount}, " +
               $"Reason: {Reason}, " +
               $"OrderReference: {OrderReference}, " +
               $"MerchantSignature: {MerchantSignature}, " +
               $"TransactionStatus: {TransactionStatus}";
    }
}