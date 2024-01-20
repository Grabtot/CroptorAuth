namespace Croptor.Api.ViewModels.Order;

public record WayForPayCallback
{
    public string MerchantAccount { get; set; }
    public string Reason { get; set; }
    public Guid OrderReference { get; set; }
    public string MerchantSignature { get; set; }
    public string TransactionStatus { get; set; }

    public override string ToString()
    {
        return $"MerchantAccount: {MerchantAccount}, " +
               $"Reason: {Reason}, " +
               $"OrderReference: {OrderReference}, " +
               $"MerchantSignature: {MerchantSignature}, " +
               $"TransactionStatus: {TransactionStatus}";
    }

}