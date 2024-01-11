namespace CroptorAuth.Options
{
    public class EmailOptions
    {
        public const string SectionName = "EmailOptions";

        public string EmailAddress { get; private set; }
        public string EmailName { get; private set; }
        public string EmailPassword { get; private set; }
        public string SmtpHost { get; private set; }
        public int SmtpPort { get; private set; }
    }
}
