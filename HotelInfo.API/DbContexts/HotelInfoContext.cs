using Microsoft.EntityFrameworkCore;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using Microsoft.Extensions.Hosting;

namespace HotelInfo.API.DbContexts
{
    public class HotelInfoContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<City> Cities { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<HotelAmenity> HotelAmenities { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<RoomClass> RoomClasses { get; set; }


        public HotelInfoContext(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:SSMSConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>()
                .HasMany(hotel => hotel.HotelAmenities)
                .WithMany();
            modelBuilder.Entity<Room>()
                .HasMany(hotel => hotel.RoomAmenities)
                .WithMany();
            modelBuilder.Entity<RoomClass>()
                .HasMany(hotel => hotel.RoomAmenities)
                .WithMany();
        }

    }
}
