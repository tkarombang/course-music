namespace Backend.DTOs.Invoice
{
    public class CourseInvoiceDto
{
    public int IdCourse { get; set; }
    public string NamaCourse { get; set; } = string.Empty;
    public decimal Harga { get; set; }
    public string Kategori { get; set; } = string.Empty;
    public DateTime Jadwal { get; set; }
}

}
