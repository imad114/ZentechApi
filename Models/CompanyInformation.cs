namespace ZentechAPI.Models
{
    public class CompanyInformation
    {
        public int Id { get; set; }                         // Identifiant unique
        public string CompanyName { get; set; }             // Nom de l'entreprise
        public string? CompanyLogoUrl { get; set; }          // URL du logo de l'entreprise
        public string CompanyAddress { get; set; }          // Adresse de l'entreprise
        public string? PostalCode { get; set; }              // Code postal
        public string? City { get; set; }                    // Ville
        public string? Country { get; set; }                 // Pays
        public string? ContactPersonName { get; set; }       // Nom de la personne de contact
        public string? ContactPersonPosition { get; set; }   // Poste de la personne de contact
        public string? PhoneNumber { get; set; }             // Numéro de téléphone mobile
        public string? FaxNumber { get; set; }               // Numéro de fax s
        public string Email { get; set; }                   // Adresse e-mail principale
        public string? BusinessLicenseCode { get; set; }     // Code de licence commerciale
        public string? FacebookUrl { get; set; }             // Lien vers la page Facebook
        public string? TwitterUrl { get; set; }              // Lien vers la page Twitter
        public string? LinkedInUrl { get; set; }             // Lien vers la page LinkedIn
        public string? InstagramUrl { get; set; }            // Lien vers la page Instagram
        public string? YoutubeUrl { get; set; }              // Lien vers la chaîne YouTube
        public DateTime? CreatedAt { get; set; }             // Date de création
        public DateTime? UpdatedAt { get; set; }             // Date de mise à jour
    }
}
