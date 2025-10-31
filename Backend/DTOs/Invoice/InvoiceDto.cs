namespace Backend.DTOs.Invoice
{
    public class InvoiceDto
    {
        public string Email { get; set; } = string.Empty;
        public string NoInvoice { get; set; }
        public DateTime TanggalBeli { get; set; }
        public int JumlahKursus { get; set; }
        public decimal TotalHarga { get; set; }

        public List<CourseInvoiceDto> Courses { get; set; }
    }
}
