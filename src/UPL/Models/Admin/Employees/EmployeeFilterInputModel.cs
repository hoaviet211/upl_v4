using System.ComponentModel.DataAnnotations;

namespace UPL.Models.Admin.Employees;

public class EmployeeFilterInputModel
{
    [Display(Name = "Từ khóa")]
    public string? SearchTerm { get; set; }

    [Display(Name = "Role")]
    public string? Role { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }
}
