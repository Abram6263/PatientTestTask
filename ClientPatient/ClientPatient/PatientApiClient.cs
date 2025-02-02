using System.Text;
using System.Text.Json;

public class PatientApiClient
{
    private readonly HttpClient _httpClient;

    public PatientApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddPatientAsync(Patient patient)
    {
        var jsonPatient = JsonSerializer.Serialize(patient);
        var content = new StringContent(jsonPatient, Encoding.UTF8, @"application/json");
        var response = await _httpClient.PostAsync("http://localhost:8080/Patient/CreatePatient", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Patient успешно добавлен");
        }
        else
        {
            response.EnsureSuccessStatusCode();
        }
    }
}