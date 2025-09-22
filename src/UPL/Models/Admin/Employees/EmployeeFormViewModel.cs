using System.ComponentModel.DataAnnotations;

namespace UPL.Models.Admin.Employees;

public class EmployeeFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [Display(Name = "Họ và tên")]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(256)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Kích hoạt tài khoản")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    [StringLength(200, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    public string? Password { get; set; }

    [Display(Name = "Xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Số điện thoại")]
    [StringLength(50)]
    public string? Phone { get; set; }

    [Display(Name = "Zalo")]
    [StringLength(50)]
    public string? ZaloNumber { get; set; }

    [Display(Name = "Chức danh")]
    [StringLength(150)]
    public string? Position { get; set; }

    [Display(Name = "Ghi chú nội bộ")]
    [DataType(DataType.MultilineText)]
    public string? Note { get; set; }

    [Display(Name = "Roles")]
    public List<string> SelectedRoles { get; set; } = new();

    public IReadOnlyList<EmployeeRoleOption> RoleOptions { get; set; } = Array.Empty<EmployeeRoleOption>();

    public bool IsEdit => Id.HasValue;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsEdit && string.IsNullOrWhiteSpace(Password))
        {
            yield return new ValidationResult("Vui lòng nhập mật khẩu", new[] { nameof(Password) });
        }

        if (!IsEdit && string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            yield return new ValidationResult("Vui lòng xác nhận mật khẩu", new[] { nameof(ConfirmPassword) });
        }

        if (ContainsHtml(FullName))
        {
            yield return new ValidationResult("Không nhập HTML", new[] { nameof(FullName) });
        }
        if (ContainsHtml(Email))
        {
            yield return new ValidationResult("Không nhập HTML", new[] { nameof(Email) });
        }
        if (!string.IsNullOrEmpty(Phone) && ContainsHtml(Phone))
        {
            yield return new ValidationResult("Không nhập HTML", new[] { nameof(Phone) });
        }
        if (!string.IsNullOrEmpty(ZaloNumber) && ContainsHtml(ZaloNumber))
        {
            yield return new ValidationResult("Không nhập HTML", new[] { nameof(ZaloNumber) });
        }
        if (!string.IsNullOrEmpty(Position) && ContainsHtml(Position))
        {
            yield return new ValidationResult("Không nhập HTML", new[] { nameof(Position) });
        }
        if (!string.IsNullOrEmpty(Note) && ContainsHtml(Note))
        {
            yield return new ValidationResult("Không nhập HTML", new[] { nameof(Note) });
        }
    }

    private static bool ContainsHtml(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return value.Contains('<') || value.Contains('>');
    }
}
