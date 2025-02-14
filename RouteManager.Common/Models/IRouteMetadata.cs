using System;

namespace RouteManager.Models
{
    public interface IRouteMetadata
    {
        Guid Id { get; }

        string Key { get; }

        string Value { get; }
    }
}
