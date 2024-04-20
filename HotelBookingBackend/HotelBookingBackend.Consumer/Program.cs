using HotelBookingBackend.DataAccess;

namespace HotelBookingBackend.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddSingleton<BookingDataService>();
            builder.Services.AddHostedService<ConsumerService>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}