namespace PBL3_Course.Services
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl(HttpContext context,VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
        
    }
}