namespace Core.Models
{
    public class EmailCredentialsModel
    {
        public string EmailCredentialsUsername { get; set; }
        public string EmailCredentialsPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}