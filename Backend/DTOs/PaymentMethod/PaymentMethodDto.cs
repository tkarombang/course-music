namespace Backend.DTOs.PaymentMethod
{
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        public string NameMethod { get; set; }
        public string LogoMethod { get; set; }
        public string Status { get; set; }
    }
}