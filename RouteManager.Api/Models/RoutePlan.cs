using System.Text.Json.Serialization;

namespace RouteManager.Models
{
    internal class RoutePlan : IRoute
    {
        [JsonIgnore]
        public ICollection<RouteStop> RouteStops { get; set; }

        [JsonIgnore]
        public ICollection<RoutePlanMetadata> RouteMetadata { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset DispatchTime { get; set; }

        public IEnumerable<IRouteMetadata> Metadata => RouteMetadata;

        public IEnumerable<IRouteStop> Stops => RouteStops;

        public RoutePlan()
        {
            Id = Guid.NewGuid();
            RouteStops = new List<RouteStop>();
            RouteMetadata = new List<RoutePlanMetadata>();
            Name = string.Empty;
            DispatchTime = DateTimeOffset.UtcNow;
        }
    }
}
