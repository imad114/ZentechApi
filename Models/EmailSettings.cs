namespace ZentechAPI.Models
{
    public class EmailSettings
    {
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string SenderEmail { get; set; }   // Nom correcte de la propriété
        public string SenderName { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
    }
}

