namespace NguyenHoangNgocChauRazorPages.Models;

public class AuthenticatedUser
{
    public int AccountID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? StaffRoleValue { get; set; }
}
