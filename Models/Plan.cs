namespace CroptorAuth.Models
{
    public record Plan
    {
        private Plan(PlanType planType, DateOnly? expireDate)
        {
            Type = planType;
            ExpireDate = expireDate;
        }

        private Plan() { }

        public PlanType Type { get; protected set; } = PlanType.Free;
        public DateOnly? ExpireDate { get; protected set; }

        public static Plan Create(PlanType type, DateOnly? expireDate = null)
        {
            return new Plan(type, expireDate);
        }

    }

    public enum PlanType
    {
        Free,
        Pro,
        Admin
    }
}
