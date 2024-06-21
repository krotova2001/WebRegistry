using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models
{
    public partial class RedBook
    {
        public RedBook()
        {
            publishingDate = DateTime.Now;
        }

        public int IdRedBook { get; set; }

        [Display(Name = "Шифр")]
        [Required]
        [StringLength(100)]
        [MinLength(3)]
        public string ShifrRedBook { get; set; } = null!;

        [Display(Name = "Инвернтарный номер")]
        [Required]
        public int Inventory { get; set; }

        //дата публикации, которую ставит юзер
        [Display(Name = "Дата выдачи в производство")]
        public DateTime userPublishingDate { get; set; }

        //реальная дата публикации
        public DateTime publishingDate { get; set; }

        [Display(Name = "Примечание")]
        public string? Note { get; set; }

        [Display(Name = "Снято")]
        public bool? IsArchived { get; set; }

        [Required]
        public string? AuthorLogin { get; set; }

        public int? StatusId { get; set; }

        [Display(Name = "Состояние")]
        public virtual Status? StatusNavigation { get; set; }
    }
}
