using System.Diagnostics.CodeAnalysis;
using HappyCode.NetCoreBoilerplate.Core.Models;
using Microsoft.EntityFrameworkCore;
using HappyCode.NetCoreBoilerplate.Core.Dtos;

public class GoogleCalendarCollection : List<GoogleCalendarModel>
{
    public GoogleCalendarCollection (): base()
    {}
}


namespace HappyCode.NetCoreBoilerplate.Core
{
    [ExcludeFromCodeCoverage]
    public partial class GoogleCalendarContext : DbContext
    {

        public GoogleCalendarContext (DbContextOptions<GoogleCalendarContext> options)
            : base(options)
        {}

        public virtual DbSet<GoogleCalendarModel> GoogleCalendarModel { get; set; }
        // public virtual DbSet<List<GoogleCalendarModel>> GoogleCalendarModelList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");
            modelBuilder.Entity<GoogleCalendarModel>(entity =>
             {
                 entity.HasIndex(e => e.id);
                 entity.Property(e => e.data);
                 entity.Property(e => e.eventId);
             });
        }
    }
}
