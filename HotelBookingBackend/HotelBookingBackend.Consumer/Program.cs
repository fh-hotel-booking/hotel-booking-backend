namespace HotelBookingBackend.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<ConsumerService>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}