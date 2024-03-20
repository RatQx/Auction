using Aukcionas.Data;
using Aukcionas.Models;
using Aukcionas.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aukcionas.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;

namespace Aukcionas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ForumRestUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IHubContext<BiddingHub> _hubContext;

        //private readonly ICloudStorageService _cloudStorageService;
        public AuctionController(UserManager<ForumRestUser> userManager,DataContext context, ILogger<AuthController> logger, IHubContext<BiddingHub> hubContext /**ICloudStorageService cloudStorageService**/)
        {
            _userManager = userManager;
            _dataContext = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext;
            //_cloudStorageService = cloudStorageService;
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Auction>>> UpdateAuction([FromBody] Auction auction)
        {
            try
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

                //await _hubContext.Clients.All.SendAsync("AuctionUpdated", auction);

                return Ok(await _dataContext.Auctions.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating auction information: {exception}", ex);
                return StatusCode(500, "Internal Server Error");
            }


        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Auction>> GetAuction(int id)
        {
            try
            {
                var auction = await _dataContext.Auctions.FindAsync(id);
                if (auction == null)
                {
                    return NoContent();
                }
                Ok(auction);
                return auction;
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Auction>>> GetAuctions([FromQuery] GetAucReq req)
        {
            return Ok(await _dataContext.Auctions.Where(req.GetPredicates()).ToListAsync());
        }


        [HttpPost]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<List<Auction>>> CreateAuction([FromBody] Auction auction)
        {
            if (ModelState.IsValid && auction != null)
            {
                _dataContext.Auctions.Add(auction);
                await _dataContext.SaveChangesAsync();
                return Ok(await _dataContext.Auctions.ToListAsync());
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            if (dbAuction.buy_now_price <= last_bid)
            {
                dbAuction.auction_end_time = DateTime.Now;
                StopAuction(auction);
                dbAuction.auction_ended = true;
                dbAuction.auction_won = true; // create seperate function to check if auction just ended or some dude won it(set winned name)
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

        [HttpPost("{id}/like")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Auction>>> LikeAuction(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var auction = await _dataContext.Auctions.FindAsync(id);

                if (auction == null)
                    return NotFound("Auction not found");

                if (!auction.auction_likes_list.Contains(userId))
                {
                    auction.auction_likes_list.Add(userId);
                    auction.auction_likes++;
                    await _dataContext.SaveChangesAsync();

                    user.Auctions_Won.Add(id);
                    await _userManager.UpdateAsync(user).ConfigureAwait(false);
                }

                return Ok(await _dataContext.Auctions.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while liking the auction: {exception}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("{id}/unlike")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Auction>>> UnlikeAuction(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var auction = await _dataContext.Auctions.FindAsync(id);

                if (auction == null)
                    return NotFound("Auction not found");

                if (auction.auction_likes_list.Contains(userId))
                {
                    auction.auction_likes_list.Remove(userId);
                    auction.auction_likes--;
                    await _dataContext.SaveChangesAsync();

                    user.Auctions_Won.Remove(id);
                    await _userManager.UpdateAsync(user).ConfigureAwait(false);
                }

                return Ok(await _dataContext.Auctions.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while unliking the auction: {exception}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Comment>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Comment>>> GetCommentsForAuction(int id)
        {
            try
            {
                var auction = await _dataContext.Auctions
                    .Include(a => a.Comments)
                    .FirstOrDefaultAsync(a => a.id == id);

                if (auction == null)
                {
                    return NotFound("Auction not found");
                }

                return Ok(auction.Comments);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while fetching comments: {exception}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("{id}/comments")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Comment))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Comment>> AddCommentToAuction(int id, [FromBody] Comment comment)
        {
            try
            {
                if (comment == null || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var auction = await _dataContext.Auctions
                    .Include(a => a.Comments)
                    .FirstOrDefaultAsync(a => a.id == id);

                if (auction == null)
                {
                    return NotFound("Auction not found");
                }

                var claims = User.Claims.Select(c => new { c.Type, c.Value });
                Console.WriteLine($"User Claims: {JsonConvert.SerializeObject(claims)}");

                comment.Date = DateTime.Now;
                comment.Username = comment.Username;
                comment.AuctionId = auction.id;

                auction.Comments.Add(comment);
                await _dataContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCommentsForAuction), new { id = auction.id }, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while adding a comment: {exception}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("PlaceBid")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PlaceBid(int auctionId, double bidAmount)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("PlaceBid", auctionId, bidAmount);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while placing bid: {exception}", ex);
                return BadRequest("Error placing bid");
            }
        }
    }
}