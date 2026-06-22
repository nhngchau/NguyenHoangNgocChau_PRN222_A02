namespace NguyenHoangNgocChauRazorPages.Models.ViewModels;

/// <summary>Payload broadcast over SignalR so clients can update the news table without reloading.</summary>
public class NewsRealtimeDto
{
    public string Action { get; set; } = "";          // created | updated | deleted
    public string Id { get; set; } = "";
    public string? Title { get; set; }
    public string? Headline { get; set; }
    public string? Content { get; set; }
    public string? Source { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; } = "N/A";
    public string Created { get; set; } = "";
    public bool Status { get; set; }
    public int StateId { get; set; }
    public string Tags { get; set; } = "";            // CSV of tag ids (for the edit modal)
    public List<string> TagNames { get; set; } = new();
}
