
    var httpClient = new HttpClient();
    var patientClient = new PatientApiClient(httpClient);
    var patientGenerator = new PatientGenerator();
    
    var patients = patientGenerator.Generate(100);
    
     foreach (var patient in patients)
     {
         await patientClient.AddPatientAsync(patient);
     }