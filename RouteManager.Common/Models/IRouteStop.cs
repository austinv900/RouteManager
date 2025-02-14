using System;

namespace RouteManager.Models
{
    public interface IRouteStop
    {
        Guid Id { get; }

        int Sequence { get; }

        string Name { get; }

        string Address { get; }

        DateTimeOffset TimeWindowBegin { get; }

        DateTimeOffset? TimeWindowEnd { get; }

        TimeSpan? DwellTime { get; }
    }
}
