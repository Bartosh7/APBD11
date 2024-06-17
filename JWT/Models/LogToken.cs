using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JWT.Controllers;

namespace JWT.Models;

[Table("LogTokens")]
public class LogToken
{
    [Key]
    [Column("id")]
    [Required]
    public int Id { get; set; }

    [Column("token")]
    [Required]
    public string Token { get; set; }

    [Column("toData")]
    [Required]
    public DateTime ToDate { get; set; }

    [ForeignKey("UserId")]
    [Column("FK_userId")]
    [Required]
    public int UserId { get; set; }
    public AppUser User { get; set; }
}