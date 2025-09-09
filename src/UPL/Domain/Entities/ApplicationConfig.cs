namespace UPL.Domain.Entities;

public class ApplicationConfig
{
    public int Id { get; set; }
    public string CodeConfig { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public bool IsRelativeContactInformation { get; set; }
    public bool IsIdCard { get; set; }
    public bool IsEmail { get; set; }
    public bool IsImages { get; set; }
    public bool IsStudentProfileDetail { get; set; }
}

