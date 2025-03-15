namespace RouteManager.Managers.Options.Route
{
    public class RouteUpdateOptions
    {
        public string? Name { get; set; }

        public DateTimeOffset? DispatchTime { get; set; }

        public ICollection<string>? RemoveMetadata { get; set; } = new List<string>();

        public IDictionary<string, string>? AddMetadata { get; set; } = new Dictionary<string, string>();
    }
}
