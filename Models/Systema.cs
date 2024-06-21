using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models
{
    /// <summary>
    /// Класс систем
    /// </summary>
    public partial class Systema
    {
        public int SystemId { get; set; }

        [Display(Name = "Сокращенное наименование")]
        public required string SystemName { get; set; }

        [Display(Name = "Описание")]
        public string? Descrition { get; set; }

        [Display(Name = "Полное наименование")]
        public string? Fullname { get; set; }
        
        [Display(Name = "Изделия")]
        public virtual ICollection<IzdelieSystema> IzdelieNavigaion { get; set; } = new List<IzdelieSystema>();
    }
}
