using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace UPL.Common;

public static class StaffRoleHelper
{
    private static readonly string[] StaffRoleNames = new[]
    {
        RoleConstants.Admin,
        RoleConstants.Staff,
        "Trưởng phòng tư vấn",
        "Phòng tư vấn",
        "Điều phối lớp",
        "Hỗ trợ điều phối",
        "Trưởng phòng đào tạo",
        "Phòng đào tạo",
        "Giám đốc"
    };

    private static readonly HashSet<string> StaffRoleSet = new(StaffRoleNames, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyCollection<string> Roles => StaffRoleNames;

    public static bool IsStaffRole(string? role) => role != null && StaffRoleSet.Contains(role);

    public static bool HasStaffAccess(ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        foreach (var claim in user.Claims)
        {
            if (claim.Type == ClaimTypes.Role && StaffRoleSet.Contains(claim.Value))
            {
                return true;
            }
        }

        return false;
    }

    public static HashSet<string> CreateRoleSet()
    {
        return new HashSet<string>(StaffRoleSet, StringComparer.OrdinalIgnoreCase);
    }
}
