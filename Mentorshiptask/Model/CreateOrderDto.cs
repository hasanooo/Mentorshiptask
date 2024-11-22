namespace Mentorshiptask.Model
{
    public class CreateOrderDto
    {
        public Guid ProductId { get; set; }
        public string CustomerName { get; set; }
        public decimal Quantity { get; set; }
    }
    public class UpdateOrderDto
    {
        public decimal Quantity { get; set; }
    }
}
