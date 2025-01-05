namespace ZentechAPI.Models
{
    public class Specification
    {
        public int SpecificationID { get; set; }
        public int ProductID { get; set; } // Clé étrangère vers Product
        public string Key { get; set; }    // Clé de la spécification (ex : "Weight", "Dimensions")
        public string Value { get; set; }  // Valeur de la spécification (ex : "2kg", "10x10x10 cm")

        
        public Product? Product { get; set; }
    }
}
