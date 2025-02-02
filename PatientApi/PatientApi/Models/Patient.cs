using System.ComponentModel.DataAnnotations;

namespace PatientApi.Models;

public class Patient
{
    public int Id { get; set; }
    [Required]
    public Name Name { get; set; }
    [RegularExpression("unknown|male|female|other",
        ErrorMessage = "Допустимые значение: unknown, male, female, other")]
    public string Gender { get; set; } = "unknown";
    [Required]
    public DateTime BirthDate { get; set; }
    public bool Active { get; set; }
}