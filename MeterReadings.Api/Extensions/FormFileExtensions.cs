using System.Globalization;
using CsvHelper;

namespace MeterReadings.Api.Extensions
{
    /// <summary>
    /// Extends the <see cref="IFormFile"/> interface.
    /// </summary>
    public static class FormFileExtensions
    {
        /// <summary>
        /// Converts a CSV <see cref="IFormFile"/> to a list of records.
        /// </summary>
        /// <typeparam name="T">The type of record to convert to.</typeparam>
        /// <param name="formFile">The <see cref="IFormFile"/> containing the CSV data.</param>
        /// <returns>The converted records.</returns>
        public static IEnumerable<T> ToCsvList<T>(this IFormFile formFile)
        {
            using var streamReader = new StreamReader(formFile.OpenReadStream());
            using var csv = new CsvReader(streamReader, CultureInfo.CurrentCulture);
            return csv.GetRecords<T>().ToList();
        }
    }
}
