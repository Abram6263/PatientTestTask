using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PatientApi.Models;

public class Name
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Use { get; set; } = "official";
    [Required]
    public string Family { get; set; }
    public List<string> Given { get; set; } =  new List<string>();

    [JsonIgnore]
    public Patient? Patient { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(Patient))]
    public int PatientId { get; set; }
    
    public Name() { }
}