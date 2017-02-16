﻿using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

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
			}

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

			/*
			 * Find newest patient by name
			 */
			{
				var toSearchFor = new SearchParams();
				toSearchFor.Add("given", p1GivenName);
				toSearchFor.Add("family", p1FamilyName);
				toSearchFor.Add("_count", "1");
				toSearchFor.Add("_sort:desc", "_lastUpdated");
				var res = client.Search<Patient>(toSearchFor);
				Console.WriteLine("Search by name, sort by lastupdated and limit to 1 result: " + (res.Entry[0].Resource as Patient).Name[0]);
			}

			/*
			 * Find patient with the name John Doe which has been updated 5 minutes ago
			 */
			{
				var toSearchFor = new SearchParams();
				toSearchFor.Add("given", p1GivenName);
				toSearchFor.Add("family", p1FamilyName);
				toSearchFor.Add("_lastUpdated", ">=" + DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-ddTdd:HH:mm"));
				var res = client.Search<Patient>(toSearchFor);
				Console.WriteLine("Search by name, get the ones lastupdated at most 5 minutes ago: " + res.Entry.Count);
			}

			/*
			 * Find all observations for patients with the name John
			 * 
			 * http://spark.furore.com/fhir/Observation?subject.name=John
			 */
			{
				var toSearchFor = new SearchParams();
				toSearchFor.Add("subject.name", p1GivenName);
				var res = client.Search<Observation>(toSearchFor);
				Console.WriteLine("Find all observations for patients with the name John: " + res.Entry.Count);
			}
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
