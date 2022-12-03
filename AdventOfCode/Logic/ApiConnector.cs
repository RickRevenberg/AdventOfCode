namespace AdventOfCode.Logic
{
	using System;
	using System.IO;
	using System.Net.Http;
	using System.Threading.Tasks;

	internal static class ApiConnector
	{
		private const string BaseEndpoint = "https://adventofcode.com/";

		internal static async Task<string> RetrieveInput(int year, int day)
		{
			var authenticationCookie = GetAuthenticationCookie();
			var combinedEndpoint = $"{BaseEndpoint}{year}/day/{day}/input";

		    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, combinedEndpoint);
			requestMessage.Headers.Add("Cookie", authenticationCookie);

			using var httpclient = new HttpClient();
			var response = await httpclient.SendAsync(requestMessage);
			
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException(
					$"Unable to retrieve input from year '{year}' day '{day}'. Make sure that these numbers are available yet and that the authentication cookie is correct.");
			}

			return await response.Content.ReadAsStringAsync();
	    }

		internal static async Task SubmitAnswer(int year, int day, int part, string answer)
		{
			var authenticationCookie = GetAuthenticationCookie();
			var combinedEndpoint = $"{BaseEndpoint}{year}/day/{day}/answer";

			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, combinedEndpoint);
			requestMessage.Headers.Add("Cookie", authenticationCookie);
		}

		private static string GetAuthenticationCookie()
		{
			var authenticationFile = Path.Combine(Directory.GetCurrentDirectory(), "../../../AuthenticationCookie.txt");
			if (!File.Exists(authenticationFile))
			{
				File.Create(authenticationFile);
			}

			var authenticationCookie = File.ReadAllText(authenticationFile);
			if (string.IsNullOrEmpty(authenticationCookie))
			{
				throw new InvalidOperationException(
					$"No authentication cookie was set. Please enter the authentication cookie in the generated file at '{authenticationFile}'");
			}

			return authenticationCookie;
		}
    }
}
