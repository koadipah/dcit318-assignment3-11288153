using HealthcareSystem.App;

var app = new HealthSystemApp();
app.SeedData();
app.BuildPrescriptionMap();

Console.WriteLine("All Patients:");
app.PrintAllPatients();

Console.WriteLine("\nPrescriptions for Patient ID 2:");
app.PrintPrescriptionsForPatient(2);

