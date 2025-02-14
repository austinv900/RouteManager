using System.Text.Json.Serialization;

namespace RouteManager.Models
{
    internal class RouteStop : IRouteStop
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Sequence { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTimeOffset TimeWindowBegin { get; set; }

        public DateTimeOffset? TimeWindowEnd { get; set; }

        public TimeSpan? DwellTime { get; set; }

        public Guid RouteId { get; set; }

        [JsonIgnore]
        public RoutePlan? Route { get; set; }

        public RouteStop()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Sequence = 0;
            Address = string.Empty;
            Latitude = double.NaN;
            Longitude = double.NaN;
            TimeWindowBegin = DateTimeOffset.UtcNow;
            TimeWindowEnd = null;
            DwellTime = null;
            RouteId = Guid.Empty;
        }
    }
}
