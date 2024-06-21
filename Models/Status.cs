using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models
{
    public partial class Status
    {
        //состояние изделия. Значения жестко зафиксированные в справочнике
        public int IdStatus { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(450)]
        public string Note { get; set; }

        [Display(Name = "Изделия в статусе")]
        public virtual ICollection<RedBook> Books { get; set; } = new List<RedBook>();

    }
}
