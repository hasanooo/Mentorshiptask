

using Microsoft.EntityFrameworkCore;

namespace Mentorshiptask.Model;


public class Order
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; } 
    public string StrCustomerName { get; set; }
    [Precision(18, 2)]
    public decimal NumQuantity { get; set; }
    public DateTime DtOrderDate { get; set; }

    // Navigation property
    public Product Product { get; set; }
}
