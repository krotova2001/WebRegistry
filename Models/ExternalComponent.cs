using System.ComponentModel;

namespace WebRegistry.Models
{
    //покупные изделия
    public partial class ExternalComponent
    {
        public int idExternalComponents { get; set; }
        [DisplayName("Наименование")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Код/Обозначение")]
        public string? Shifr { get; set; }

        [DisplayName("Вес")]
        public double? Weight { get; set; }

        [DisplayName("Примечание")]
        public string? Note { get; set; }

        [DisplayName("Использование в изделиях")]
        public virtual ICollection<ExtCompInIzdelie> IzdelieNavigation { get; set; } = new List<ExtCompInIzdelie>();
    }
}
