using System;
using System.Collections.Generic;

namespace RouteManager.Models
{
    public interface IRoute
    {
        Guid Id { get; }

        string Name { get; }

        DateTimeOffset DispatchTime { get; }

        IEnumerable<IRouteMetadata> Metadata { get; }

        IEnumerable<IRouteStop> Stops { get; }
    }
}
