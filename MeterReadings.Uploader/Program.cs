using System.Net.Http.Headers;
using System.Net.Http.Json;
using MeterReadings.Core.Models;

const string meterReadingsUrl = "https://localhost:7013/meter-readings-uploads";
var meterReadingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"inputs\Meter_Reading.csv") ;

Console.WriteLine($"Processing csv file '{meterReadingsPath}'");

var httpClient = new HttpClient();
using var multipartFormContent = new MultipartFormDataContent();
await using var csvStream = File.OpenRead(meterReadingsPath);
using var streamContent = new StreamContent(csvStream);

streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
multipartFormContent.Add(streamContent, "file", Path.GetFileName(meterReadingsPath));

Console.WriteLine($"Sending meter readings to '{meterReadingsUrl}'");
var httpResponse = await httpClient.PostAsync(meterReadingsUrl, multipartFormContent);

httpResponse.EnsureSuccessStatusCode();

var response = await httpResponse.Content.ReadFromJsonAsync<MeterReadingResponse>();

if (response == null)
{
    Console.WriteLine("Unexpected error processing meter readings. Response is empty.");
}
else
{
    foreach (var (meterReadingEntry, validationErrors) in response.Errors)
    {
        Console.WriteLine($"FAILURE: {meterReadingEntry}. ERRORS: {string.Join("|", validationErrors)}");
    }

    Console.WriteLine($"\nFinished processing meter readings. SuccessCount: {response.SuccessCount}, FailureCount: {response.Errors.Count}.");
}

Console.ReadKey();