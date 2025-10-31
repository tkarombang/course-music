using Microsoft.AspNetCore.Components.Forms;

public class CategoryCreateModel
{
    public int IdCategory { get; set; }
    public string NameCategory { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }

    public IBrowserFile? ImageCategory { get; set; } // file kategori
    public IBrowserFile? ImageBanner { get; set; }   // file banner

    public string? CurrentImageUrl { get; set; } // optional
    public string? CurrentBannerUrl { get; set; } // optional
}
