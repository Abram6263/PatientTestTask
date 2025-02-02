using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PatientApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Получить пациента по ID", Description = "Возвращает информацию о пациенте по уникальному идентификатору.")]
    [SwaggerResponse(200, "Пациент найден", typeof(Patient))]
    [SwaggerResponse(404, "Пациент с таким ID не найден")]
    public async Task<ActionResult<Patient>> GetPatient(int id)
    {
        var patient = await _context.Patients.Include(p => p.Name).FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Создать нового пациента", Description = "Создает нового пациента в системе.")]
    [SwaggerResponse(201, "Пациент успешно создан", typeof(Patient))]
    [SwaggerResponse(400, "Пациент с таким ID или именем уже существует")]
    public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
    {
        var existingPatient = await _context.Patients.Include(p => p.Name)
            .FirstOrDefaultAsync(p => p.Id == patient.Id || p.Name.Id == patient.Name.Id);

        if (existingPatient != null)
        {
            if (existingPatient.Id == patient.Id)
                return BadRequest("Пациент с таким Id уже существует.");
            if (existingPatient.Name.Id == patient.Name.Id)
                return BadRequest("Имя с таким Id уже существует.");
        }

        patient.BirthDate = patient.BirthDate.ToUniversalTime();
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return Created(nameof(CreatePatient), patient);
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Обновить данные пациента", Description = "Обновляет информацию о пациенте.")]
    [SwaggerResponse(200, "Пациент успешно обновлен", typeof(Patient))]
    [SwaggerResponse(404, "Пациент или имя не найдены")]
    public async Task<ActionResult<Patient>> UpdatePatient(Patient patient)
    {
        patient.BirthDate = patient.BirthDate.ToUniversalTime();
        _context.Entry(patient).State = EntityState.Modified;
        _context.Entry(patient.Name).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            if (!_context.Patients.Any(p => p.Id == patient.Id) || !_context.Names.Any(n => n.Id == patient.Name.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(patient);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Удалить пациента", Description = "Удаляет пациента по ID.")]
    [SwaggerResponse(200, "Пациент успешно удален")]
    [SwaggerResponse(404, "Пациент с таким ID не найден")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return NotFound();
        
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Поиск пациентов по дате рождения", Description = "Позволяет искать пациентов по фильтрам на основе даты рождения.")]
    [SwaggerResponse(200, "Список найденных пациентов", typeof(List<Patient>))]
    [SwaggerResponse(400, "Неверный формат даты или отсутствует фильтр")]
    public async Task<IActionResult> SearchByDate( string dateFilter)
    {
        if (string.IsNullOrEmpty(dateFilter))
        {
            return BadRequest("Не указан фильтр даты.");
        }

        var patients = _context.Patients.Include(p => p.Name).AsQueryable();
        
        var match = Regex.Match(dateFilter, @"(eq|ne|lt|gt|ge|le|sa|eb|ap)?(\d{4}(-\d{2}(-\d{2}(T\d{2}:\d{2}(:\d{2}(\.\d{3})?)?)?)?)?)");
        if (!match.Success)
        {
            return BadRequest("Неверный формат даты.");
        }

        string? prefix = match.Groups[1].Value;
        if (!DateTime.TryParse(match.Groups[2].Value, out DateTime filterDate))
        {
            return BadRequest("Некорректное значение даты.");
        }

        switch (prefix)
        {
            case "eq":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime().Date == filterDate.ToUniversalTime().Date);
                break;
            case "ne":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime().Date != filterDate.ToUniversalTime().Date);
                break;
            case "lt":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() < filterDate.ToUniversalTime());
                break;
            case "gt":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() > filterDate.ToUniversalTime());
                break;
            case "ge":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() >= filterDate.ToUniversalTime());
                break;
            case "le":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() <= filterDate.ToUniversalTime());
                break;
            case "sa":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() > filterDate.ToUniversalTime());
                break;
            case "eb":
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() < filterDate.ToUniversalTime());
                break;
            case "ap":
                // так как "ap" означает "примерно", берем ±7 дней от указанной даты
                DateTime lowerBound = filterDate.AddDays(-7).ToUniversalTime();
                DateTime upperBound = filterDate.AddDays(7).ToUniversalTime();
                patients = patients.Where(p => p.BirthDate.ToUniversalTime() >= lowerBound && p.BirthDate.ToUniversalTime() <= upperBound);
                break;
            default:
                patients = patients.Where(p => p.BirthDate.ToUniversalTime().Date == filterDate.ToUniversalTime().Date);
                break;
        }

        var result = await patients.ToListAsync();
        return Ok(result);
    }
}