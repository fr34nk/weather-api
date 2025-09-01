using System.Diagnostics.CodeAnalysis;
using HappyCode.NetCoreBoilerplate.Core.Models;
using Microsoft.EntityFrameworkCore;
using HappyCode.NetCoreBoilerplate.Core.Dtos;

public class WeatherCollection : List<Weather>
{
    public WeatherCollection(): base()
    {}
}

namespace HappyCode.NetCoreBoilerplate.Core
{
    [ExcludeFromCodeCoverage]
    public partial class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options)
            : base(options)
        {}

        public virtual DbSet<Weather> Weather { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Weather>(entity =>
             {
                 entity.HasIndex(e => e.Id);

                 entity.Property(e => e.Id).ValueGeneratedOnAdd();

                 entity.Property(e => e.Temperature);

                 entity.Property(e => e.Humidity);
             });
        }
    }
}
