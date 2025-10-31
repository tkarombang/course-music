namespace Frontend.Models
{
  public class InvoiceModel
  {
    public string? Email { get; set; }
    public string? NoInvoice { get; set; }
    public DateTime TanggalBeli { get; set; }
    public int JumlahKursus { get; set; }
    public decimal TotalHarga { get; set; }
  }

  public class DashboardData
  {
    public IEnumerable<InvoiceModel> Invoices { get; set; } = [];
    public int TotalActiveUsers { get; set; }
    public int TotalPayment { get; set; }
    public int TotalMember { get; set; }
  }
}