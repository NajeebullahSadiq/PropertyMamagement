namespace WebAPIBackend.Models.RequestData
{
    public class CompanyDetailData
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public bool? Status { get; set; }

        public string? DocPath { get; set; }

        public string? Tin { get; set; }

        /// <summary>
        /// Calendar type for date parsing: "gregorian", "hijriShamsi", or "hijriQamari"
        /// Defaults to "hijriShamsi" if not provided
        /// </summary>
        public string? CalendarType { get; set; }
    }
}
