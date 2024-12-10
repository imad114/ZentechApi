namespace Zentech.Models
{
    public class Role
    {
        public int RoleID { get; set; } 
        public string RoleName { get; set; } // Nom du rôle (e.g., Admin, User)

        public List<User> Users { get; set; } // Liste des utilisateurs associés à ce rôle
    }
}
