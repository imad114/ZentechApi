namespace ZentechAPI.Models
{
    public class Other_Category
    {
        // update by imad 26/01/2025 8:37 change CategoryID to int and change date to DateTime
        public int CategoryID { get; set; }
        public string?  CategoryType{ get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
