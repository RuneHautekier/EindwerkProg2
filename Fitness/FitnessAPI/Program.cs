using FitnessBL.Interfaces;
using FitnessBL.Services;
using FitnessEF.Repositories;

namespace FitnessAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connectionString =
                @"Data Source=rune\SQLEXPRESS;Initial Catalog=GymTest;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder
                .Services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                        .Json
                        .ReferenceLoopHandling
                        .Ignore
                );

            builder.Services.AddSingleton<IMemberRepo>(r => new MemberRepo(connectionString));
            builder.Services.AddSingleton<MemberService>();

            builder.Services.AddSingleton<IEquipmentRepo>(r => new EquipmentRepo(connectionString));
            builder.Services.AddSingleton<EquipmentService>();

            builder.Services.AddSingleton<ITime_slotRepo>(r => new Time_slotRepo(connectionString));
            builder.Services.AddSingleton<Time_slotService>();

            builder.Services.AddSingleton<IReservationRepo>(r => new ReservationRepo(
                connectionString
            ));
            builder.Services.AddSingleton<ReservationService>();

            builder.Services.AddSingleton<IProgramRepo>(r => new ProgramRepo(connectionString));
            builder.Services.AddSingleton<ProgramService>();

            builder.Services.AddSingleton<MaintenanceService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
