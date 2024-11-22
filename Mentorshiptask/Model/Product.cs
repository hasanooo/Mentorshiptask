using Mentorshiptask.Model;
using Microsoft.EntityFrameworkCore;

public class Product
{
    public Guid Id { get; set; }
    public string StrProductName { get; set; }
    [Precision(18, 2)]
    public decimal NumUnitPrice { get; set; }
    [Precision(18, 2)]
    public decimal NumStock { get; set; }

   
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
