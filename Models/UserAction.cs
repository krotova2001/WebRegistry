using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models
{
    //писание значимых дейтсвий пользователя для запси в журнал
    public partial class UserAction
    {
       public int idUserAction { get; set; }

       [Required, MaxLength(150)]
       public string userLogin { get; set; }

       [MaxLength(450)]
       public string actionText { get; set; }

       [Required]
       public DateTime date { get; set; }
    }
}
