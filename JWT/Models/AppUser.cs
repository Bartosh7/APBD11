using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWT.Models;
[Table("Users")]
public class AppUser
{
    [Key]
    [Column("id")]
    [Required]
    public int IdUser { get; set; }
    
    [Column("email")]
    [Required]
    public string Email { get; set; }
    [Column("password_hashed")]
    [Required]
    public string PasswordHashed { get; set; }
    [Column("salt")]
    [Required]
    public string Salt { get; set; }
   
}