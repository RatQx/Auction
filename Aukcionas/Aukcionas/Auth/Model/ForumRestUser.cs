using Microsoft.AspNetCore.Identity;

namespace Aukcionas.Auth.Model
{
    public class ForumRestUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }
        [PersonalData]
        public string? Surname { get; set; }
        [PersonalData]
        public string? Phone_Number { get; set; }
        [PersonalData]
        public List<int> Auctions_Won { get; set; } = new ();
        [PersonalData]
        public List<int> Liked_Auctions { get; set; } = new();
        [PersonalData]
        public Boolean Can_Bid { get; set; } = true;
        [PersonalData]
        public string? ResetPasswordToken { get; set; }
        public DateTime ResetPasswordExpiry { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public DateTime EmailConfirmationTokenExpiry { get; set; }
        [PersonalData]
        public Boolean?  Paypal { get; set; }
        [PersonalData]
        public Boolean? Bank { get; set;}
        [PersonalData]
        public string? Paypal_Email { get; set; }
        [PersonalData]
        public string? Account_Holder_Name { get; set; }
        [PersonalData]
        public string? Account_Number { get; set; }
        [PersonalData]
        public string? Bank_Name { get; set; }
        [PersonalData]
        public string? Bic_Swift_Code { get; set; }

    }
}
