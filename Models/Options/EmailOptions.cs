namespace WebApplication1.Models.Options
{
    public class EmailOptions
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string Login {  get; set; } = string.Empty;
    }
}

