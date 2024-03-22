using Aukcionas.Data;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using Aukcionas.Auth.Model;

namespace Aukcionas.Services
{
    public class BiddingHub : Hub
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<BiddingHub> _logger;
        public BiddingHub(DataContext dataContext, ILogger<BiddingHub> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AuctionBid");
            await Clients.Caller.SendAsync("UserConnected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AuctionBid");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task PlaceBid(int auctionId, double bidAmount, string username)
        {
            try
            {
                var auction = _dataContext.Auctions.Find(auctionId);
                if (auction == null)
                {
                    return;
                }
                if (auction.auction_ended == true || auction.auction_end_time <= DateTime.Now)
                {
                    return;
                }
                await Clients.All.SendAsync("UpdateBid", auctionId, bidAmount);
                double currentBid = (auction.bidding_amount_history.Count > 0)
                ? auction.bidding_amount_history.Last()
                : auction.starting_price;
                if (bidAmount >= currentBid)
                {
                    auction.auction_biders_list.Add(username);
                    auction.bidding_amount_history.Add(bidAmount);
                    auction.bidding_times_history.Add(DateTime.Now);

                    await _dataContext.SaveChangesAsync();

                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing bid.");
                throw;
            }
        }
    }
}
