using ZentechAPI.Models;

public class Product
{
    public int ProductID { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; } 
    public string? UpdatedBy { get; set; }
   
    public int CategoryID { get; set; }
    public int? MainCategoryID { get; set; }

    // public Category? Category { get; set; } 

    public List<string>? Photos { get; set; }
    public List<Specification>? Specifications { get; set; }
    public string? MainCategoryName { get; set; }
    public string? CategoryName { get;  set; }
}
