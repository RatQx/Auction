using System.ComponentModel.DataAnnotations;

namespace Aukcionas.Models
{
    public class Report
    {
        public int id { get; set; }
        [Required]
        public string auction_Id { get; set; }
        public string? userName { get; set; }
        [Required]
        public string report_Message { get; set; }
        public DateTime? report_Time { get; set; }
    }
}
