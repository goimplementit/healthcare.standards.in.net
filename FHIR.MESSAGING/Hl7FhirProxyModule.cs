using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using Nancy;
using Nancy.Extensions;
using Hl7.Fhir.Serialization;

namespace Messaging
{
	public class Hl7FhirProxyModule : NancyModule
	{
		/*
		 * Config
		 */
		private const string HOST_FHIR_SERVER = "http://spark-dstu2.furore.com";
		private const string CONTENT_TYPE = "application/json";

		static HttpClient fhirServerProxyClient = new HttpClient();

		public Hl7FhirProxyModule()
		{
			//debug mode
			StaticConfiguration.DisableErrorTraces = false;

			fhirServerProxyClient.BaseAddress = new Uri(HOST_FHIR_SERVER);
			fhirServerProxyClient.DefaultRequestHeaders.Accept.Clear();
			fhirServerProxyClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(CONTENT_TYPE));

			Get["/{uri*}", true] = async (parameters, ctx) =>
			{
				var path = parameters.uri + this.Request.Url.Query;
				var response = await GetAsync(path);
				return await response.Content.ReadAsStringAsync();
			};

			Post["/{uri*}", true] = async (parameters, ctx) =>
			{
				var path = parameters.uri + this.Request.Url.Query;
				var body = Request.Body.AsString();
				var response = await PostAsync(path, body);

				notify(path, body);

				return await response.Content.ReadAsStringAsync();
			};
		}

		private async Task<HttpResponseMessage> GetAsync(string path)
		{
			return await fhirServerProxyClient.GetAsync(path);
		}

		private async Task<HttpResponseMessage> PostAsync(string path, string body)
		{
			var content = new StringContent(body, Encoding.UTF8, "application/json");
			return await fhirServerProxyClient.PostAsync(path, content);
		}

		private async Task<HttpResponseMessage> PutAsync(string path, string body)
		{
			var content = new StringContent(body, Encoding.UTF8, "application/json");
			return await fhirServerProxyClient.PutAsync(path, content);
		}

		private void notify(string path, string str)
		{
			var bundle = new Bundle();
			bundle.Id = Uuid.Generate().ToString();
			bundle.Type = Bundle.BundleType.Message;

			var headerId = Uuid.Generate().ToString();
			var header = new MessageHeader();
			header.Id = headerId;
			bundle.AddResourceEntry(header, "MessageHeader/" + headerId);

			var resource = new FhirJsonParser().Parse<Resource>(str);
			bundle.AddResourceEntry(resource, path);

			var bundleAsJson = FhirSerializer.SerializeResourceToJson(bundle);
			Console.WriteLine(bundleAsJson);
		}
	}
}