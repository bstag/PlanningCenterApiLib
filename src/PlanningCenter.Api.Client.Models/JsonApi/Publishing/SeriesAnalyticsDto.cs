namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing
{
    public class SeriesAnalyticsDto
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public SeriesAnalyticsAttributesDto Attributes { get; set; }
        public object Links { get; set; }
    }

    public class SeriesAnalyticsAttributesDto
    {
        public int TotalViews { get; set; }
        public int TotalDownloads { get; set; }
        public int TotalPlays { get; set; }
    }

    public class SeriesAnalyticsDataDto
    {
        public int Views { get; set; }
        public int Downloads { get; set; }
    }
}
