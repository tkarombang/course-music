namespace Backend.DTOs.Checkout
{
    public class CreateCheckoutDto
    {
        public int IdUser { get; set; }
        public int IdCourse { get; set; }
        public DateTime Jadwal { get; set; }
    }
}