using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOneDbContext.Models
{
    [Table("Users")]
    public class User
    {

        [Key]
        public int UserId { get; set; }
        public int UserInfoId { get; set; }
        public virtual UserInfo Info { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
