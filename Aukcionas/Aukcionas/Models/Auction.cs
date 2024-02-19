using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Aukcionas.Models;
public class Auction
{
    public int id { get; set; }
    [Required]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Maximum 200 characters")]
    public string name { get; set; }
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Maximum 30 characters")]
    public string country { get; set; }
    [Required]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Maximum 200 characters")]
    public string city { get; set; }
    [Required]
    [Range(1, 10, ErrorMessage = "Value must be between 1 and 10")]
    public double starting_price { get; set; }
    [Required]
    [Range(1, 10, ErrorMessage = "Value must be between 1 and 10")]
    public int bid_ammount { get; set; }
    [Required]
    [Range(1, 10, ErrorMessage = "Value must be between 1 and 10")]
    public double min_buy_price { get; set; }
    [Required]
    public DateTime auction_start_time { get; set; }
    [Required]
    public DateTime auction_end_time { get; set; }
    public DateTime? auction_time { get; set; }
    public Boolean? auction_stopped { get; set; }
    public Boolean? auction_ended { get; set; }
    public Boolean? auction_won { get; set; }
    [Required]
    [Range(1, 10, ErrorMessage = "Value must be between 1 and 10")]
    public double buy_now_price { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Maximum 50 characters")]
    public string category { get; set; }
    [Required]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Maximum 300 characters")]
    public string description { get; set; }
    [Required]
    [StringLength(4, MinimumLength = 1, ErrorMessage = "Maximum 4 characters")]
    public string item_build_year { get; set; }
    [Required]
    [Range(1, 10, ErrorMessage = "Value must be between 1 and 10")]
    public double item_mass { get; set; } // in kg
    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Maximum 20 characters")]
    public string condition { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Maximum 50 characters")]
    public string material { get; set; }
    public int? auction_likes { get; set; }
    public string? username { get; set; }
    public string? auction_winner { get; set; }
    public List<string> auction_biders_list { get; set; } = new();
    public List<double> bidding_amount_history { get; set; } = new();
    public List<DateTime> bidding_times_history { get; set; } = new();

    [NotMapped]
    public string? picture { get; set; } //change to IFormFile type
    [NotMapped]
    public string? SingedUrl { get; set; }
    public string? SavedUrl { get; set; }
    public string? SavedFileName { get; set; }
    //add comments and commented users list.




}


