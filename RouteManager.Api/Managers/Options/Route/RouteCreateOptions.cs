using NuGet.Packaging;

namespace RouteManager.Managers.Options.Route
{
    public class RouteCreateOptions
    {
        public string? Name { get; set; }

        public DateTimeOffset? DispatchTime { get; set; }

        public IDictionary<string, string>? Metadata { get; set; }

        public RouteCreateOptions()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            Name = $"{now:yyyyMMddHHmmss}";
            DispatchTime = now;
            Metadata = new Dictionary<string, string>();
        }

        public RouteCreateOptions(string name) : this()
        {
            Name = name;
        }

        public RouteCreateOptions(string name, DateTimeOffset dispatchTime) : this(name)
        {
            DispatchTime = dispatchTime;
        }

        public RouteCreateOptions(string name, DateTimeOffset dispatchTime, IReadOnlyDictionary<string, string> metadata) : this(name, dispatchTime)
        {
            Metadata.AddRange(metadata);
        }
    }
}
