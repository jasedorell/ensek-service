namespace MeterReadings.Data.Models
{
    public class MeterReading
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public DateTime MeterReadingDateTime { get; set; }

        public string MeterReadValue { get; set; }

        public Account Account { get; set; }
    }
}
