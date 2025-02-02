public class Patient
{
    public int Id { get; set; }
    public Name Name { get; set; }
    public string Gender { get; set; } = "unknown";
    public DateTime BirthDate { get; set; }
    public bool Active { get; set; }
}