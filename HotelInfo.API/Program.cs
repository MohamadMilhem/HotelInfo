using HotelInfo.API.DbContexts;
using HotelInfo.API.Services;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace HotelInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setup =>
            {
                var xmlCommentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName);

                setup.IncludeXmlComments(xmlCommentsFullPath);
            });

            var storageConnection = builder.Configuration["ConnectionStrings:HotelReservationWebApi:Storage"];

            builder.Services.AddDbContext<HotelInfoContext>();
            builder.Services.AddScoped<IHotelInfoRepository, HotelInfoRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddControllersWithViews()
                            .AddNewtonsoftJson();
            builder.Services.AddAzureClients(azureBuilder =>
            {
                azureBuilder.AddBlobServiceClient(storageConnection);
            });

            var app = builder.Build();


            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}