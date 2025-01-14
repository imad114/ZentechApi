namespace ZentechAPI.Dto
{
    public class AddProductsToSolutionDto
    {
        public int SolutionID { get; set; }
        public List<int> ProductIDs { get; set; }
    }
}
