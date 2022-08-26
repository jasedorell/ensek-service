using MeterReadings.Api.Extensions;
using MeterReadings.Core.Models;
using MeterReadings.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadings.Api.Controllers
{
    /// <summary>
    /// The Meter Readings API Controller.
    /// </summary>
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly MeterReadingProcessor _meterReadingProcessor;

        public MeterReadingController(MeterReadingProcessor meterReadingProcessor)
        {
            _meterReadingProcessor = meterReadingProcessor;
        }

        /// <summary>
        /// Processes a CSV file of Meter Readings.
        /// </summary>
        /// <param name="file">The CSV file to process.</param>
        /// <returns>The <see cref="MeterReadingResponse"/> for the CSV file.</returns>
        [Route("meter-readings-uploads")]
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            return this.Ok(await _meterReadingProcessor.Process(file.ToCsvList<MeterReadingEntry>()));
        }
    }
}
