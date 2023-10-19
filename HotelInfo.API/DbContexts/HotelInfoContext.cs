using Microsoft.EntityFrameworkCore;
using HotelInfo.API.Entites;

namespace HotelInfo.API.DbContexts
{
    public class HotelInfoContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<City> Cities { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Photo> Photos { get; set; }


        public HotelInfoContext(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:SSMSConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

    }
}
