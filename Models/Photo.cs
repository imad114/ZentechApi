public class Photo
{
    public int PhotoID { get; set; } 
    public int EntityID { get; set; } 
    public string EntityType { get; set; } // Type de l'entité ("News" ou "Product")
    public string Url { get; set; } 
}
