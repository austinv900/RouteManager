using System.ComponentModel;

namespace RouteManager.Managers.Options.Route
{
    public class RouteQueryOptions
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public DateTimeOffset? DispatchStartTime { get; set; }

        public DateTimeOffset? DispatchEndTime { get; set; }

        public int? StopMinimum { get; set; }

        public int? StopMaximum { get; set; }

        [DefaultValue(10)]
        public int? Limit { get; set; } = 10;
    }
}
