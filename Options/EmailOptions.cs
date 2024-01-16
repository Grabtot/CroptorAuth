namespace CroptorAuth.Options
{
    public class EmailOptions
    {
        public const string SectionName = "EmailOptions";

        public string EmailAddress { get; set; }
        public string EmailName { get; set; }
        public string EmailPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
    }
}
