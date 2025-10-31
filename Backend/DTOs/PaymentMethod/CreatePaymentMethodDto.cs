namespace Backend.DTOs.PaymentMethod
{
     public class CreatePaymentMethodDto
    {
        public string NameMethod { get; set; }
        public IFormFile? LogoMethod { get; set; }
        public string Status { get; set; }
    }
}