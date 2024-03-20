namespace Aukcionas.Models
{
    public class EmailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string SenderName { get; set; }
        public EmailModel(string to, string subject, string content, string senderName)
        {
            To = to;
            Subject = subject;
            Content = content;
            SenderName = senderName;
        }
    }
}
