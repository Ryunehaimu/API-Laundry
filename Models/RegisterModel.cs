using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class RegisterModel
{
[Required(ErrorMessage = "Nama Lengkap wajib diisi.")]
    [StringLength(100)]
    public string NamaLengkap { get; set; } 

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email tidak valid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Nomor Telepon wajib diisi.")]
    [RegularExpression(@"^(\+62|0)\d{9,13}$", ErrorMessage = "Format telepon tidak valid.")] 
    public string TeleponPengguna { get; set; }

    [Required(ErrorMessage = "Password wajib diisi.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password harus minimal 6 karakter.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}