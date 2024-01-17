namespace Croptor.Api.ViewModels.Order;

public record WayForPayCallback
{
    public string MerchantAccount { get; set; }
    public Guid OrderReference  { get; set; }
    public string MerchantSignature { get; set; }
    public string TransactionStatus { get; set; }
}