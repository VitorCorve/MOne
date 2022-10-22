using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOneDbContext.Models
{
    [Table("Infos")]
    public class UserInfo
    {
        [Key]
        public int UserInfoId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime DateRegister { get; set; }
        public string Gender { get; set; } = string.Empty;
    }
}
