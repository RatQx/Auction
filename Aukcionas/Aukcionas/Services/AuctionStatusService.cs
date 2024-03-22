using Aukcionas.Auth.Model;
using Aukcionas.Data;
using Aukcionas.Models;
using Aukcionas.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aukcionas.Services
{
    public class AuctionStatusService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuctionStatusService(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task CheckAuctionStatus(DataContext dbContext)
        {
            var endedAuctions = await dbContext.Auctions
                .Where(a => a.auction_end_time <= DateTime.Now && (a.auction_ended == null || !a.auction_ended.Value))
                .ToListAsync();

            foreach (var auction in endedAuctions)
            {
                auction.auction_ended = true;
                var userManager = dbContext.GetService<UserManager<ForumRestUser>>();
                if (auction.bidding_amount_history.Any())
                {
                    double highestBid = auction.bidding_amount_history.Max();
                    if (highestBid >= auction.min_buy_price)
                    {
                        auction.auction_won = true;
                        var user = await userManager.FindByNameAsync(auction.auction_biders_list.Last());
                        auction.auction_winner = user.UserName;

                        var auctionOwner = await userManager.FindByIdAsync(auction.username);
                            
                        var payment = new Payment(_configuration);
                        var emailToken = payment.GeneratePaymentToken(user.Id,auction.id);

                        string from = _configuration["EmailConfiguration:From"];
                        var emailModelBuyer = new EmailModel(user.Email, "Auction win", EmailBody.EmailPaymentForUser(emailToken, auction.name, auction.bidding_amount_history.Last()), "Auction winner confirmation");
                        _emailService.SendEmail(emailModelBuyer);

                        var emailModelOwner = new EmailModel(auctionOwner.Email, "Your auction has a winner", EmailBody.EmailAuctionWonForOwner(auction.name, auction.bidding_amount_history.Last(), user.UserName, auction.id), "Auction ended");
                        _emailService.SendEmail(emailModelOwner);
                    }
                    else
                    {
                        var auctionOwner = await userManager.FindByIdAsync(auction.username);
                        var emailModelOwner = new EmailModel(auctionOwner.Email, "Your auction has ended", EmailBody.EmailAuctionEndedForOwner(auction.name, auction.id), "Auction ended");
                        _emailService.SendEmail(emailModelOwner);
                    }
                }
                else
                {
                    var auctionOwner = await userManager.FindByIdAsync(auction.username);
                    var emailModelOwner = new EmailModel(auctionOwner.Email, "Your auction has ended", EmailBody.EmailAuctionEndedForOwner(auction.name, auction.id), "Auction ended");
                    _emailService.SendEmail(emailModelOwner);
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
