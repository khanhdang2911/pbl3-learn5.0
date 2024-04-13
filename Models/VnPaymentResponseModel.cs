namespace PBL3_Course
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }
    public class VnPaymentRequestModel
    {
        public int OrderId{set;get;}
        public string FullName{set;get;}
        public string Description{set;get;}
        public double Amount{set;get;}
        public int courseId{set;get;}

        public DateTime CreatedDate{set;get;}
    }
}