using Aukcionas.Data;
using Aukcionas.Models;
using Aukcionas.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Aukcionas.Utils;

namespace Aukcionas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly DataContext _dataContext;
        //private readonly ICloudStorageService _cloudStorageService;
        public AuctionController(DataContext context /**ICloudStorageService cloudStorageService**/)
        {
            _dataContext = context;
            //_cloudStorageService = cloudStorageService;
        }
        [HttpPut("Update")]
        public async Task<ActionResult<List<Auction>>> UpdateAuction([FromBody]Auction auction)
        {
            var dbAuction = await _dataContext.Auctions.FindAsync(auction.id);
            if (dbAuction == null)
                return BadRequest("Auction not found");
            dbAuction.name = auction.name;
            dbAuction.country = auction.country;
            dbAuction.city = auction.city;
            dbAuction.bid_ammount = auction.bid_ammount;
            dbAuction.min_buy_price = auction.min_buy_price;
            if (auction.auction_end_time >= DateTime.Now.AddHours(1))
                dbAuction.auction_end_time = auction.auction_end_time;
            dbAuction.buy_now_price = auction.buy_now_price;
            dbAuction.category = auction.category;
            dbAuction.description = auction.description;
            dbAuction.item_build_year = auction.item_build_year;
            dbAuction.item_mass = auction.item_mass;
            dbAuction.condition = auction.condition;
            dbAuction.material = auction.material;
            //dbAuction.picture = auction.picture;

            await _dataContext.SaveChangesAsync();
            CheckIfItemIsBought(auction);

            return Ok(await _dataContext.Auctions.ToListAsync());

        }

        [HttpGet]
        public async Task<ActionResult<List<Auction>>> GetAuctions([FromQuery] GetAucReq req)
        {
            return Ok(await _dataContext.Auctions.Where(req.GetPredicates()).ToListAsync());
        }


        [HttpPost]
        public async Task<ActionResult<List<Auction>>> CreateAuction([FromBody]Auction auction)
        {
            //auction.username = "Pakeistas"; 
            _dataContext.Auctions.Add(auction);
            Console.WriteLine(auction);
            await _dataContext.SaveChangesAsync();
            return Ok(await _dataContext.Auctions.ToListAsync());
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<List<Auction>>> DeleteAuction([FromRoute] int id)
        {
            var dbAuction = await _dataContext.Auctions.FindAsync(id);
            if (dbAuction == null)
                return BadRequest("Auction not found");

            _dataContext.Remove(dbAuction);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.Auctions.ToListAsync());
        }

        [HttpPut("CheckIfItemIsBought")]
        public async Task<ActionResult<List<Auction>>> CheckIfItemIsBought(Auction auction)// check if new buy now price is larger then last bid
        {
            var dbAuction = await _dataContext.Auctions.FindAsync(auction.id);
            if (dbAuction == null)
                return BadRequest("Auction not found");
            var last_bid = dbAuction.bidding_amount_history[^1];
            if(dbAuction.buy_now_price<=last_bid)
            {
                dbAuction.auction_end_time = DateTime.Now;
                StopAuction(auction);
                dbAuction.auction_ended = true;
                dbAuction.auction_won =true; // create seperate function to check if auction just ended or some dude won it(set winned name)
            }

            return Ok(await _dataContext.Auctions.ToListAsync());
        }
        [HttpPut("CheckNewEndTime")]
        public async Task<ActionResult<List<Auction>>> CheckNewEndTime(Auction auction)//check if new end time is at least set to one hour from now 
        {
            var dbAuction = await _dataContext.Auctions.FindAsync(auction.id);
            if (dbAuction == null)
                return BadRequest("Auction not found");
            if (dbAuction.auction_end_time >= DateTime.Now.AddHours(1))
                return Ok(await _dataContext.Auctions.ToListAsync());
            else
                return BadRequest();
        }
        [HttpPut("StopAuction")]
        public async Task<ActionResult<List<Auction>>> StopAuction(Auction auction)// stop auction from live bidding
        {
            var dbAuction = await _dataContext.Auctions.FindAsync(auction.id);
            if (dbAuction == null)
                return BadRequest("Auction not found");
            dbAuction.auction_stopped = true;
            return Ok(await _dataContext.Auctions.ToListAsync());
        }

        //add function - extend aution time on high amount of bids in last minutes.
        //add function - if auction had 0 bids post it again.
        //add function - get 1 auction by id 
    }
}
