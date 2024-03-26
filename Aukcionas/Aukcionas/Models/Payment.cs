using Microsoft.AspNetCore.Routing.Constraints;

namespace Aukcionas.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Payment_Id { get; set; }
        public DateTime Payment_Time { get; set; }
        public double Payment_Amount { get; set; }
        public string Payment_Currency { get; set; }
        public Boolean Payment_Successful { get; set; }
        public string Address_Line1 { get; set; }
        public string Address_Line2 { get; set; }
        public string Country { get; set; }
        public string Postal_Code { get; set;}
        public string Buyer_Id { get; set; }
        public string Auction_Id { get; set; }
        public string Buyer_Email { get; set; }
       public string Auction_Owner_Email { get; set; }
        public double? Payout_Amount { get; set; }
        public DateTime? Payout_Time {  get; set; } 
        public string? Payout_Currency { get; set; }
        public Boolean? Payout_Successful { get; set; }
    }
}
