using System.ComponentModel.DataAnnotations;

namespace UPL.Models.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email hoặc tên đăng nhập")]
    [Display(Name = "Email hoặc Tên đăng nhập")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ghi nhớ")] 
    public bool RememberMe { get; set; }
}

