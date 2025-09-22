namespace UPL.Models.Admin.Employees;

public class EmployeeListViewModel
{
    public EmployeeFilterInputModel Filter { get; set; } = new();
    public IReadOnlyList<EmployeeListItemViewModel> Items { get; set; } = Array.Empty<EmployeeListItemViewModel>();
    public IReadOnlyList<EmployeeRoleOption> RoleOptions { get; set; } = Array.Empty<EmployeeRoleOption>();
}
