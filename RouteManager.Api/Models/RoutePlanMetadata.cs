using System.Text.Json.Serialization;

namespace RouteManager.Models
{
    internal class RoutePlanMetadata : IRouteMetadata
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        [JsonIgnore]
        public Guid RouteId { get; set; }

        [JsonIgnore]
        public RoutePlan? Route { get; set; }

        public RoutePlanMetadata()
        {
            Id = Guid.NewGuid();
            Key = string.Empty;
            Value = string.Empty;
            RouteId = Guid.Empty;
        }

        public RoutePlanMetadata(string key, string value, Guid routeId) : this()
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Key = key;
            Value = value;
            RouteId = routeId;
        }

        public override string ToString() => $"{Key}: {Value}";
    }
}
