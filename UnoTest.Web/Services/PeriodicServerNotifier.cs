namespace UTT
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Hosting;
    using UnoWeb.Test;

   //  https://github.com/davidfowl/UT3/blob/fb12e182d42d2a5a902c1979ea0e91b66fe60607/UTT/Scavenger.cs
   // https://github.com/davidfowl/UT3/blob/fb12e182d42d2a5a902c1979ea0e91b66fe60607/UTT/Startup.cs#L46

    public class PeriodicServerNotifier : BackgroundService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public PeriodicServerNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", Guid.NewGuid());
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}