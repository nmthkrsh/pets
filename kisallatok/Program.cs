using System;
using System.Collections.Generic;
using System.Linq;

namespace kisallatok
{
    public abstract class Person
    {
        public string PersonalID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        protected Person(string personalID, string name, string phoneNumber, string emailAddress)
        {
            PersonalID = personalID;
            Name = name;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
        }

        public override string ToString()
        {
            return $"ID: {PersonalID}, Name: {Name}, Phone: {PhoneNumber}, Email: {EmailAddress}";
        }
    }
    public class Pet
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public int AgeInYears { get; set; }
        public string OwnerPersonalID { get; set; }
        public string ChipNumber { get; set; }

        public Pet(string name, string species, int ageInYears, string ownerPersonalID, string chipNumber)
        {
            Name = name;
            Species = species;
            AgeInYears = ageInYears;
            OwnerPersonalID = ownerPersonalID;
            ChipNumber = chipNumber ?? throw new ArgumentNullException(nameof(chipNumber));
        }

        public void CheckClinic(VetClinic clinic)
        {
            var vetsTreating = clinic.ListOfVets.Where(vet => vet.PatientList.Contains(this)).ToList();
            if (vetsTreating.Count > 0)
            {
                Console.WriteLine($"{Name} is treated at {clinic.ClinicName} by: ");
                foreach (var vet in vetsTreating)
                {
                    Console.WriteLine(vet.Name);
                }
            }
            else Console.WriteLine($"{Name} is not being treated at {clinic.ClinicName}");
        }
        public override string ToString()
        {
            return $"Name: {Name}, Species: {Species}, Age: {AgeInYears}, Owner ID: {OwnerPersonalID}, Chip: {ChipNumber}";
        }
    }

    public class Owner : Person
    {
        public string BillingAddress { get; set; }
        public HashSet<Pet> GuardedPets { get; set; }

        public Owner(string personalID, string name, string phoneNumber, string emailAddress, string billingAddress)
            : base(personalID, name, phoneNumber, emailAddress)
        {
            BillingAddress = billingAddress;
            GuardedPets = new HashSet<Pet>();
        }

        public override string ToString()
        {
            return base.ToString() + $", Billing Address: {BillingAddress}, Pets: {GuardedPets.Count}";
        }
    }

    public class Vet : Person
    {
        public string CertificateNumber { get; set; }
        public List<string> TreatedSpecies { get; set; }
        public HashSet<Pet> PatientList { get; set; }

        public Vet(string personalID, string name, string phoneNumber, string emailAddress, string certificateNumber, List<string> treatedSpecies)
            : base(personalID, name, phoneNumber, emailAddress)
        {
            CertificateNumber = certificateNumber;
            TreatedSpecies = treatedSpecies;
            PatientList = new HashSet<Pet>();
        }

        public Vet(string personalID, string name, string phoneNumber, string emailAddress, string certificateNumber, List<string> treatedSpecies, IEnumerable<Owner> owners)
            : this(personalID, name, phoneNumber, emailAddress, certificateNumber, treatedSpecies)
        {
            FilterAndAddPatients(owners);
        }

        public void FilterAndAddPatients(IEnumerable<Owner> owners)
        {
            foreach (var owner in owners)
            {
                foreach (var pet in owner.GuardedPets)
                {
                    if (TreatedSpecies.Contains(pet.Species))
                    {
                        PatientList.Add(pet);
                    }
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + $", Certificate: {CertificateNumber}, Treated Species: {string.Join(", ", TreatedSpecies)}, Patients: {PatientList.Count}";
        }
    }
    public class VetClinic
    {
        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicPhone { get; set; }
        public string ClinicEmail { get; set; }
        public HashSet<Vet> ListOfVets { get; set; }

        public VetClinic(string clinicName, string clinicAddress, string clinicPhone, string clinicEmail)
        {
            ClinicName = clinicName;
            ClinicAddress = clinicAddress;
            ClinicPhone = clinicPhone;
            ClinicEmail = clinicEmail;
            ListOfVets = new HashSet<Vet>();
        }

        public override string ToString()
        {
            return $"Clinic: {ClinicName}, Address: {ClinicAddress}, Phone: {ClinicPhone}, Vets: {ListOfVets.Count}";
        }
    }
    internal class Program
    {
        static void Main()
        {
            var owner1 = new Owner("O1", "Alice", "123456789", "alice@mail.com", "123 Main St");
            var owner2 = new Owner("O2", "Bob", "987654321", "bob@mail.com", "456 Oak St");
            var owner3 = new Owner("O3", "Charlie", "567890123", "charlie@mail.com", "789 Pine St");

            var pet1 = new Pet("Fluffy", "Dog", 3, "O1", "CHIP001");
            var pet2 = new Pet("Whiskers", "Cat", 2, "O1", "CHIP002");
            var pet3 = new Pet("Buddy", "Dog", 5, "O2", "CHIP003");
            var pet4 = new Pet("Snowball", "Rabbit", 1, "O2", "CHIP004");
            var pet5 = new Pet("Mittens", "Cat", 4, "O3", "CHIP005");

            owner1.GuardedPets.Add(pet1);
            owner1.GuardedPets.Add(pet2);
            owner2.GuardedPets.Add(pet3);
            owner2.GuardedPets.Add(pet4);
            owner3.GuardedPets.Add(pet5);

            
            var vet1 = new Vet("V1", "Dr. Smith", "111222333", "smith@clinic.com", "CERT001", new List<string> { "Dog", "Cat" });
            var vet2 = new Vet("V2", "Dr. Johnson", "444555666", "johnson@clinic.com", "CERT002", new List<string> { "Rabbit" }, new List<Owner> { owner1, owner2 });

            var clinic1 = new VetClinic("Happy Pets Clinic", "123 Vet St", "555-CLINIC1", "contact@happypets.com");
            var clinic2 = new VetClinic("Healthy Animals Center", "456 Health Rd", "555-CLINIC2", "info@healthyanimals.com");

            clinic1.ListOfVets.Add(vet1);
            clinic2.ListOfVets.Add(vet2);

            pet1.CheckClinic(clinic1);
            pet4.CheckClinic(clinic2);

            Console.WriteLine(owner1);
            Console.WriteLine(owner2);
            Console.WriteLine(vet1);
            Console.WriteLine(vet2);
            Console.WriteLine(clinic1);
            Console.WriteLine(clinic2);
        }
    }
}
