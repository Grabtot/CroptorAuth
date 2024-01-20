namespace CroptorAuth.Models
{
    public record Order
    {
        private Order() { }

        private Order(Guid id, Guid userId, int amount)
        {
            Id = id;
            UserId = userId;
            Amount = amount;
        }

        public static Order Create(Guid userId, int amount)
        {
            return new(Guid.NewGuid(), userId, amount);
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public int Amount { get; private set; }
    }   
}
