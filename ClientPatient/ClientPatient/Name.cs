
using System.Text.Json.Serialization;

public class Name
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Use { get; set; } = "official";
    public string Family { get; set; }
    public List<string> Given { get; set; } =  new List<string>();
    
    public Name() { }
}