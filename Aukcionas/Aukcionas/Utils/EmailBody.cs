namespace Aukcionas.Utils
{
    public static class EmailBody
    {
        public static string EmailStringReset(string email, string emailToken) // for password reset
        {
            return $@"<html>
            <body>
                <div>
                    <p>Reset your password.</p>
                    <P>Click link below to reset your password. Link will expire in 15 minutes.</p><br>
                    <a href=http://localhost:4200/reset?email={email}&code={emailToken}>Reset password</a><br>
                </div>
            </body> 
            </html>
            ";
        }
        public static string EmailStringConfirm(string email, string emailToken)// for new user email confirmation
        {
            return $@"<html>
        <body>
            <div>
                <p>Welcome.</p>
                <P>Click the link below to confirm your email address:</p><br>
                <a href=http://localhost:4200/confirm-email?email={email}&code={emailToken}>Confirm Email</a><br>
            </div>
        </body> 
        </html>
        ";
        }
        public static string EmailPaymentForUser(string paymentToken, string auctionName, double price)
        {
            return $@"<html>
        <body>
            <div>
                <p>Congratulations!</p>
                <P>You won online auction: {auctionName} </p><br>
                <P>You outbidded other contestants and won auction. Your bidded price was: {price} </p><br>
                <p> Click the link below to make payment for auction. The link will be valid for 1 week.</p><br>
                <p> After the link expires you woun't be able to pay for auction and you woun't recive item.</p><br>
                <a href=http://localhost:4200/payment?token={paymentToken}>Payment link</a><br>
            </div>
        </body> 
        </html>
        ";
        }

        public static string EmailAuctionWonForOwner(string auctionName, double price, string auction_winner, int auction_id)
        {
            return $@"<html>
        <body>
            <div>
                <p>Congratulations!</p>
                <P>You auction: {auctionName}, has finished</p><br>
                <P>The bids were enough to meet minimum buy amount. The winning bid was: {price} </p><br>
                <P> User that won the auction: {auction_winner}</p><br>
                <p> This user now has1 week to make payment to auction.</p><br>
                <p> Once the payment is complete you will recive users shipping information. </p><br>
                <p> For more information check auction page. </p><br>
                <a href=http://localhost:4200/auction-bid/{auction_id}>Auction page </a><br>
            </div>
        </body> 
        </html>
        ";
        }

        public static string EmailAuctionEndedForOwner(string auctionName, int auction_id)
        {
            return $@"<html>
        <body>
            <div>
                <P>You auction: {auctionName}, has finished</p><br>
                <P>The bids were not enough to meet minimum buy amount. </p><br>
                <p> For more information check you'r auction page. </p><br>
                <a href=http://localhost:4200/auction-bid/{auction_id}>Auction page </a><br>
            </div>
        </body> 
        </html>
        ";
        }
    }
}
