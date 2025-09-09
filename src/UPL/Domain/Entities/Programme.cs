namespace UPL.Domain.Entities;

public class Programme
{
    public int Id { get; set; }
    public string ProgrammeName { get; set; } = string.Empty;
    public string? AdmissionCycle { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

