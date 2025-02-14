using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RouteManager.Database;
using RouteManager.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RouteManager.Managers
{
    public class RouteManager : IModelManager<Guid, IRoute, RouteManager.CreateOptions, RouteManager.UpdateOptions, RouteManager.SearchOptions>
    {
        protected RoutingContext Database { get; }

        protected ILogger Logger { get; }

        public RouteManager(RoutingContext db, ILogger<RouteManager> logger)
        {
            Database = db;
            Logger = logger;
        }

        #region Options

        public class CreateOptions
        {
            public string Name { get; set; } = Guid.NewGuid().ToString("D").Substring(8);

            public DateTimeOffset DispatchTime { get; set; } = DateTimeOffset.UtcNow;

            public IDictionary<string, string>? Metadata { get; set; } = new Dictionary<string, string>();

            public ICollection<StopCreateOptions>? Stops { get; set; } = new List<StopCreateOptions>();
        }

        public class UpdateOptions
        {
            public string? Name { get; set; }

            public DateTimeOffset? DispatchTime { get; set; }

            public ICollection<string>? RemoveMetadata { get; set; } = new List<string>();

            public IDictionary<string, string>? AddMetadata { get; set; } = new Dictionary<string, string>();

            public ICollection<Guid>? RemoveStops { get; set; } = new List<Guid>();

            public ICollection<StopUpdateOptions>? AddStops { get; set; } = new List<StopUpdateOptions>();
        }

        public class SearchOptions
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

        public class StopCreateOptions
        {
            public string? Name { get; set; }

            public int? Sequence { get; set; }

            public string? Address { get; set; }

            public double? Latitude { get; set; }

            public double? Longitude { get; set; }

            public DateTimeOffset? TimeWindowBegin { get; set; }

            public DateTimeOffset? TimeWindowEnd { get; set; }

            public TimeSpan? DwellTime { get; set; }
        }

        public class StopUpdateOptions
        {
            public Guid Id { get; set; }

            public int? Sequence { get; set; }

            public string? Address { get; set; }

            public double? Latitude { get; set; }

            public double? Longitude { get; set; }

            public DateTimeOffset? TimeWindowBegin { get; set; }

            public DateTimeOffset? TimeWindowEnd { get; set; }

            public TimeSpan? DwellTime { get; set; }
        }

        #endregion

        #region Interface Methods

        public async Task<IRoute> Create(CreateOptions options, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(options.Name))
            {
                Logger.LogDebug("Route creation failed: Missing name");
                throw new ArgumentNullException(nameof(options.Name), "Missing route name");
            }

            if (options.DispatchTime == default)
            {
                Logger.LogDebug("Route creation failed: Missing dispatch time");
                throw new ArgumentNullException(nameof(options.DispatchTime), "Missing dispatch time");
            }

            await using IDbContextTransaction transaction = await Database.Database.BeginTransactionAsync(cancellationToken);
            RoutePlan plan = new RoutePlan();

            try
            {
                plan.Name = options.Name;
                plan.DispatchTime = options.DispatchTime;
                await Database.Routes.AddAsync(plan, cancellationToken);
                Logger.LogDebug("Route {ID} ({NAME}) added to entity tracking", plan.Id, plan.Name);
                
                if (options.Metadata != null && options.Metadata.Count > 0)
                {
                    IEnumerable<RoutePlanMetadata> metadata = options.Metadata.Select(x => new RoutePlanMetadata(x.Key, x.Value, plan.Id));
                    await Database.RouteMetadata.AddRangeAsync(metadata, cancellationToken);
                }

                if (options.Stops != null && options.Stops.Count > 0)
                {
                    // TODO: Add Stop Processing
                }

                await Database.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return plan;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> Delete(IRoute route, CancellationToken cancellationToken = default)
        {
            if (route == null)
            {
                Logger.LogDebug("Route deletion failed: Missing route instance");
                return false;
            }

            return await Delete([route], cancellationToken) > 0;
        }

        public async Task<int> Delete(IEnumerable<IRoute> routes, CancellationToken cancellationToken = default)
        {
            if (routes == null || routes.Count() == 0)
            {
                Logger.LogDebug("Route deletion failed: Missing route list");
                return 0;
            }

            RoutePlan[] routePlans = routes.Where(x => x is RoutePlan)
                .Select(x => (RoutePlan)x)
                .ToArray();

            if (routePlans.Length == 0)
            {
                Logger.LogDebug("Route deletion failed: Items arn't RoutePlans");
                return 0;
            }

            Database.Routes.RemoveRange(routePlans);
            int exe = await Database.SaveChangesAsync(cancellationToken);
            Logger.LogWarning("Successfully removed {COUNT} route objects from data source", exe);
            return exe;
        }

        public async Task<IRoute> Update(Guid routeId, UpdateOptions options, CancellationToken cancellationToken = default)
        {
            IQueryable<RoutePlan> query = Database.Routes.Where(x => x.Id == routeId);

            if (options.RemoveMetadata != null || options.AddMetadata != null)
            {
                query = query.Include(x => x.RouteMetadata);
            }

            RoutePlan? plan = await query.FirstOrDefaultAsync(cancellationToken);

            if (plan == null)
            {
                Logger.LogError("Failed to update route with {ID}: Route doesn't exist", routeId);
                throw new KeyNotFoundException($"Failed to update route with {routeId:D}: Route doesn't exist");
            }

            if (!string.IsNullOrWhiteSpace(options.Name) && options.Name != plan.Name)
            {
                plan.Name = options.Name;
            }

            if (options.DispatchTime.HasValue)
            {
                plan.DispatchTime = options.DispatchTime.Value;
            }

            if (options.RemoveMetadata != null && options.RemoveMetadata.Count > 0)
            {
                foreach (string key in options.RemoveMetadata)
                {
                    IRouteMetadata? meta = plan.Metadata.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                    if (meta != null)
                    {
                        Database.Remove(meta);
                        Logger.LogDebug("Removing metadata key {KEY}", key);
                    }
                }
            }

            if (options.AddMetadata != null && options.AddMetadata.Count > 0)
            {
                IEnumerable<RoutePlanMetadata> meta = options.AddMetadata.Select(x => new RoutePlanMetadata(x.Key, x.Value, plan.Id));
                await Database.RouteMetadata.AddRangeAsync(meta, cancellationToken);
            }

            await Database.SaveChangesAsync(cancellationToken);
            return plan;
        }

        public async IAsyncEnumerable<IRoute> Find(SearchOptions? options, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IQueryable<RoutePlan> query = Database.Routes
                .Include(x => x.RouteStops.OrderBy(n => n.Sequence))
                .Include(x => x.RouteMetadata)
                .OrderByDescending(x => x.DispatchTime);

            if (options != null)
            {
                if (options.Id.HasValue)
                {
                    query = query.Where(x => x.Id == options.Id.Value);
                }

                if (!string.IsNullOrEmpty(options.Name))
                {
                    query = query.Where(x => x.Name.Contains(options.Name, StringComparison.InvariantCultureIgnoreCase));
                }

                if (options.DispatchStartTime.HasValue)
                {
                    query = query.Where(x => x.DispatchTime >= options.DispatchStartTime.Value);
                }

                if (options.DispatchEndTime.HasValue)
                {
                    query = query.Where(x => x.DispatchTime <= options.DispatchEndTime.Value);
                }

                if (options.StopMinimum.HasValue)
                {
                    query = query.Where(x => x.Stops.Count() >= options.StopMinimum.Value);
                }

                if (options.StopMaximum.HasValue)
                {
                    query = query.Where(x => x.Stops.Count() <= options.StopMaximum.Value);
                }

                if (options.Limit.HasValue)
                {
                    query = query.Take(options.Limit.Value);
                }
                else
                {
                    query = query.Take(10);
                }
            }
            else
            {
                query = query.Take(10);
            }

            await foreach (var route in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                if (route != null)
                {
                    yield return route;
                }
            }
        }

        #endregion
    }
}
