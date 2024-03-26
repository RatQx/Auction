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
using Aukcionas.Utils;

namespace Aukcionas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ForumRestUser> _userManager;
        private readonly ILogger<AuctionController> _logger;
        private readonly IHubContext<BiddingHub> _hubContext;
        private readonly IConfiguration _configuration;

        //private readonly ICloudStorageService _cloudStorageService;
        public AuctionController(UserManager<ForumRestUser> userManager,DataContext context, ILogger<AuctionController> logger, IConfiguration configuration, IHubContext<BiddingHub> hubContext /**ICloudStorageService cloudStorageService**/)
        {
            _userManager = userManager;
            _dataContext = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext;
            _configuration = configuration;
            //_cloudStorageService = cloudStorageService;
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Auction))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Auction>> UpdateAuction([FromBody] Auction auction)
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
                dbAuction.username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //dbAuction.picture = auction.picture;

                await _dataContext.SaveChangesAsync();

                return Ok(dbAuction);
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if(user.Bank == null || user.Paypal == null)
                {
                    return BadRequest();
                }
                auction.username = userId;
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
                }
                if(!user.Liked_Auctions.Contains(id))
                {
                    user.Liked_Auctions.Add(id);
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
                }
                if (user.Liked_Auctions.Contains(id))
                {
                    user.Liked_Auctions.Remove(id);
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
    }
}