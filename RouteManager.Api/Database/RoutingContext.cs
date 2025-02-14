using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RouteManager.Models;

namespace RouteManager.Database
{
    public class RoutingContext : DbContext
    {
        internal DbSet<RoutePlan> Routes { get; set; }

        internal DbSet<RoutePlanMetadata> RouteMetadata { get; set; }

        internal DbSet<RouteStop> RouteStops { get; set; }

        internal DbSet<Location> Locations { get; set; }

        public RoutingContext(DbContextOptions<RoutingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dtvc = new ValueConverter<DateTimeOffset, long>(
                dto => dto.UtcTicks,
                ticks => new DateTimeOffset(ticks, TimeSpan.Zero));

            var tsvc = new ValueConverter<TimeSpan, long>(
                ts => ts.Ticks,
                ticks => TimeSpan.FromTicks(ticks));

            modelBuilder.Entity<RoutePlan>(entity =>
            {
                entity.ToTable("Routes", "Routing");
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.RouteStops)
                .WithOne(x => x.Route)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.RouteMetadata)
                .WithOne(x => x.Route)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(x => x.DispatchTime)
                .HasConversion(dtvc);
            });

            modelBuilder.Entity<RoutePlanMetadata>(entity =>
            {
                entity.ToTable("Metadata", "Routing");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Key)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(x => x.Value)
                .HasMaxLength(200)
                .IsRequired();
            });

            modelBuilder.Entity<RouteStop>(entity =>
            {
                entity.ToTable("Stops", "Routing");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                .HasMaxLength(50);

                entity.Property(x => x.Address)
                .HasMaxLength(200);

                entity.Property(x => x.TimeWindowBegin)
                .HasConversion(dtvc);

                entity.Property(x => x.TimeWindowEnd)
                .HasConversion(dtvc);

                entity.Property(x => x.DwellTime)
                .HasConversion(tsvc);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations", "Routing");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Address)
                    .HasMaxLength(200)
                    .IsRequired();
            });
        }
    }
}
