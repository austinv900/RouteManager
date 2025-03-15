using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RouteManager.Database;
using RouteManager.Managers.Options.Route;
using RouteManager.Models;
using System.Data;
using System.Runtime.CompilerServices;

namespace RouteManager.Managers
{
    public class RouteManager : IModelManager<Guid, IRoute, RouteCreateOptions, RouteUpdateOptions, RouteQueryOptions>
    {
        protected RoutingContext Database { get; }

        protected ILogger Logger { get; }

        public RouteManager(RoutingContext db, ILogger<RouteManager> logger)
        {
            Database = db;
            Logger = logger;
        }

        #region Interface Methods

        public async Task<IRoute> CreateModel(RouteCreateOptions createOptions, CancellationToken cancellationToken = default)
        {
            await using IDbContextTransaction transaction = await Database.Database.BeginTransactionAsync(cancellationToken);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            RoutePlan plan = new RoutePlan();

            try
            {
                // Name Creation
                if (string.IsNullOrEmpty(createOptions.Name))
                {
                    plan.Name = $"{now:yyyyMMddHHmmss}";
                    Logger.LogDebug("No name provided in createOptions, using {NAME}", plan.Name);
                }
                else
                {
                    plan.Name = createOptions.Name;
                    Logger.LogDebug("Using route name {NAME}", createOptions.Name);
                }

                // Dispatch Time
                if (!createOptions.DispatchTime.HasValue)
                {
                    plan.DispatchTime = now;
                    Logger.LogDebug("No dispatch time provided for {NAME}, using {TIME}", plan.Name, now);
                }
                else
                {
                    plan.DispatchTime = createOptions.DispatchTime.Value;
                    Logger.LogDebug("Using dispatch time {TIME} for route {NAME}", plan.DispatchTime, plan.Name);
                }

                await Database.Routes.AddAsync(plan, cancellationToken);
                Logger.LogDebug("Route {ID} ({NAME}) added to entity tracking", plan.Id, plan.Name);

                // Add Metadata
                if (createOptions.Metadata != null && createOptions.Metadata.Count > 0)
                {
                    foreach (var x in createOptions.Metadata)
                    {
                        RoutePlanMetadata meta = new RoutePlanMetadata(x.Key, x.Value, plan.Id);
                        await Database.RouteMetadata.AddAsync(meta, cancellationToken);
                        Logger.LogDebug("Added metadata for route {RID}: {KEY} => {VALUE}", plan.Id, x.Key, x.Value);
                    }
                }

                await Database.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Logger.LogInformation("Created route {NAME} ({ID})", plan.Name, plan.Id);
                return plan;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                Logger.LogError(e, "Failed to create route {NAME}", plan.Name);
                throw;
            }
        }

        public async Task<int> DeleteModel(Guid[] modelIds, CancellationToken cancellationToken = default)
        {
            if (modelIds == null || modelIds.Length == 0)
            {
                Logger.LogDebug("No model IDs provided for deletion");
                return 0;
            }

            Logger.LogDebug("Preparing to delete routes with IDs {IDS}", string.Join(',', modelIds));
            RoutePlan[] plans = Database.Routes.Where(x => modelIds.Contains(x.Id)).ToArray();

            if (plans.Length == 0)
            {
                Logger.LogDebug("No routes found with provided IDs");
                return 0;
            }

            Database.Routes.RemoveRange(plans);
            int exe = await Database.SaveChangesAsync(cancellationToken);
            Logger.LogWarning("Successfully removed {COUNT} route objects from data source", exe);
            return exe;
        }

        public async Task<IRoute> UpdateModel(Guid modelId, RouteUpdateOptions updateOptions, CancellationToken cancellationToken = default)
        {
            IQueryable<RoutePlan> query = Database.Routes.Where(x => x.Id == modelId);
            Logger.LogDebug("Searching for route {ID}", modelId);

            if (updateOptions.RemoveMetadata != null || updateOptions.AddMetadata != null)
            {
                Logger.LogDebug("Including metadata");
                query = query.Include(x => x.RouteMetadata);
            }

            RoutePlan? plan = await query.FirstOrDefaultAsync(cancellationToken);

            if (plan == null)
            {
                Logger.LogError("Failed to update route with {ID}: Route doesn't exist", modelId);
                throw new KeyNotFoundException($"Failed to update route with {modelId:D}: Route doesn't exist");
            }

            using IDbContextTransaction transaction = await Database.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (!string.IsNullOrWhiteSpace(updateOptions.Name) && updateOptions.Name != plan.Name)
                {
                    plan.Name = updateOptions.Name;
                }

                if (updateOptions.DispatchTime.HasValue)
                {
                    plan.DispatchTime = updateOptions.DispatchTime.Value;
                }

                if (updateOptions.RemoveMetadata != null && updateOptions.RemoveMetadata.Count > 0)
                {
                    foreach (string key in updateOptions.RemoveMetadata)
                    {
                        IRouteMetadata? meta = plan.Metadata.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                        if (meta != null)
                        {
                            Database.Remove(meta);
                            Logger.LogDebug("Removing metadata key {KEY}", key);
                        }
                    }
                }

                if (updateOptions.AddMetadata != null && updateOptions.AddMetadata.Count > 0)
                {
                    foreach (var x in updateOptions.AddMetadata)
                    {
                        RoutePlanMetadata metaData = new RoutePlanMetadata(x.Key, x.Value, plan.Id);
                        await Database.RouteMetadata.AddAsync(metaData, cancellationToken);
                        Logger.LogDebug("Added metadata for route {ID}: {NAME} => {VALUE}", plan.Id, metaData.Key, metaData.Value);
                    }
                }

                await Database.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                Logger.LogInformation("Updated route {NAME} ({ID})", plan.Name, plan.Id);
                return plan;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                Logger.LogError(e, "Failed to update Route: {ID}", modelId);
                throw;
            }
        }

        public async IAsyncEnumerable<IRoute> LookupModels(RouteQueryOptions? lookupOptions, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IQueryable<RoutePlan> query = Database.Routes
                .Include(x => x.RouteStops.OrderBy(n => n.Sequence))
                .Include(x => x.RouteMetadata)
                .OrderByDescending(x => x.DispatchTime);

            if (lookupOptions != null)
            {
                if (lookupOptions.Id.HasValue)
                {
                    query = query.Where(x => x.Id == lookupOptions.Id.Value);
                }

                if (!string.IsNullOrEmpty(lookupOptions.Name))
                {
                    query = query.Where(x => x.Name.Contains(lookupOptions.Name, StringComparison.InvariantCultureIgnoreCase));
                }

                if (lookupOptions.DispatchStartTime.HasValue)
                {
                    query = query.Where(x => x.DispatchTime >= lookupOptions.DispatchStartTime.Value);
                }

                if (lookupOptions.DispatchEndTime.HasValue)
                {
                    query = query.Where(x => x.DispatchTime <= lookupOptions.DispatchEndTime.Value);
                }

                if (lookupOptions.StopMinimum.HasValue)
                {
                    query = query.Where(x => x.Stops.Count() >= lookupOptions.StopMinimum.Value);
                }

                if (lookupOptions.StopMaximum.HasValue)
                {
                    query = query.Where(x => x.Stops.Count() <= lookupOptions.StopMaximum.Value);
                }

                if (lookupOptions.Limit.HasValue)
                {
                    query = query.Take(lookupOptions.Limit.Value);
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
