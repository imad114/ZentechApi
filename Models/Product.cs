using ZentechAPI.Models;

public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } 
    public string UpdatedBy { get; set; } 

    // Liste des photos associées
    public List<string> Photos { get; set; }
}
