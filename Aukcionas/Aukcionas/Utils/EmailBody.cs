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
    }
}
