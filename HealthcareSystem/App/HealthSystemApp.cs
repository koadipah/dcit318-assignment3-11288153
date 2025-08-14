using System;
using System.Collections.Generic;
using System.Linq;
using HealthcareSystem.Models;
using HealthcareSystem.Repositories;

namespace HealthcareSystem.App
{
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Owusu", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Timothy Ninsin", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Kukua Ama", 28, "Male"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Aspirin", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Cough Syrup", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            var prescriptions = _prescriptionRepo.GetAll();
            _prescriptionMap.Clear();

            foreach (var p in prescriptions)
            {
                if (!_prescriptionMap.ContainsKey(p.PatientId))
                {
                    _prescriptionMap[p.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[p.PatientId].Add(p);
            }
        }

        public void PrintAllPatients()
        {
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId)
                ? _prescriptionMap[patientId]
                : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var prescriptions = GetPrescriptionsByPatientId(patientId);
            if (!prescriptions.Any())
            {
                Console.WriteLine("No prescriptions found for this patient.");
                return;
            }

            foreach (var p in prescriptions)
            {
                Console.WriteLine($"Prescription ID: {p.Id}, Medication: {p.MedicationName}, Date Issued: {p.DateIssued:d}");
            }
        }
    }
}
