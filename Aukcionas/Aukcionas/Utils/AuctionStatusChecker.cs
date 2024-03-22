using Aukcionas.Data;
using Aukcionas.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aukcionas.Utils
{
    public class AuctionStatusChecker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public AuctionStatusChecker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var auctionStatusService = scope.ServiceProvider.GetRequiredService<AuctionStatusService>();

                    await auctionStatusService.CheckAuctionStatus(dbContext);
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
