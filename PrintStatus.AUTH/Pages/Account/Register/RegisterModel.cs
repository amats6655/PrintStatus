using System.ComponentModel.DataAnnotations;

namespace PrintStatus.AUTH.Pages.Register;

public class RegisterModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }

    public string ReturnUrl { get; set; }
}