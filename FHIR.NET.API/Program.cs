using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Validation;

namespace FHIR.NET.API
{
	class MainClass
	{
		/*
		 * Config
		 */
		const string host = "http://spark-dstu2.furore.com/fhir";

		public static void Main(string[] args)
		{
			Console.WriteLine("Let's create a patient");

			/*
			 * The guys from furore where so nice to setup a test server for us.
			 * 
			 * Connect to FHIR server
			 */
			var client = new FhirClient(host);

			/*
			 * Create patient
			 */
			var pCreator = new PatientCreator();

			const string p1GivenName = "John";
			const string p1FamilyName = "Doe";
			var p1 = pCreator.CreatePatient(client, p1GivenName, p1FamilyName);

			const string p2GivenName = "Jane";
			const string p2FamilyName = "Doe";
			pCreator.CreatePatient(client, p2GivenName, p2FamilyName);

			/*
			 * Find patient by id
			 */
			{
				var res = client.Read<Patient>("Patient/"+p1.Id);
				Console.WriteLine("Patient by id: " + res.Name[0]);
				var serialized = FhirSerializer.SerializeToJson(res);
				Console.WriteLine("Serialized patient: " + serialized);
				var backToObject = new FhirJsonParser().Parse<Patient>(serialized);
				Console.WriteLine("Parsed patient: " + backToObject);

				//res.Deceased = new FhirString("not a valid element"); 
				//DotNetAttributeValidation.Validate(res);
			}
			Console.WriteLine();

			/*
			 * Find patients by name
			 */
			{
				var toSearchFor = new SearchParams();
				toSearchFor.Add("given", p1GivenName);
				toSearchFor.Add("family", p1FamilyName);
				toSearchFor.Add("_count", "100");
				var res = client.Search(toSearchFor, "Patient");
				Console.WriteLine("Search by name: Found " + res.Entry.Count + " with the name " + p1GivenName + " " + p1FamilyName);
			}
			Console.WriteLine();

			/*
			 * Find newest patient by name
			 */
			{

			}
			Console.WriteLine();

			/*
			 * Find patient with the name John Doe which has been updated 5 minutes ago
			 */
			{

			}
			Console.WriteLine();

			/*
			 * Find all observations for patients with the name John
			 */
			{

			}
			Console.WriteLine();
		}
	}

	class PatientCreator
	{
		public Patient CreatePatient(FhirClient client, string p1GivenName, string p1FamilyName)
		{
			var p = new Patient();
			p.Name.Add(HumanName.ForFamily(p1FamilyName).WithGiven(p1GivenName));
			return client.Create(p);
		}
	}
}
