using System;

namespace RouteManager.Models
{
    public interface ILocation
    {
        Guid Id { get; }

        string Name { get; }

        string Address { get; }
    }
}
