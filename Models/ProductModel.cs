namespace ZentechAPI.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public int? ModelId { get; set; }

        public string? Model { get; set; }
        public string? Displacement { get; set; }
        public string? CoolingType { get; set; }
        public string? MotorType { get; set; }
        public string? VoltageFrequency { get; set; }
        public double? CoolingCapacityW { get; set; }
        public double? CoolingCapacityBTUPerHour { get; set; }
        public double? CoolingCapacityKcal { get; set; }
        public double? COPWW { get; set; }
        public double? COPBTUPerWH { get; set; }
        public string?  CreateDate { get; set; }
        public string? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? CreatedBy { get; set; }
    }
}
