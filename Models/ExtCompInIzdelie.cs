using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models;

public partial class ExtCompInIzdelie
{
    public int IdExtComponent { get; set; }

    [DisplayName("Шифр изделия, где используется")]
    public string ParentShifr { get; set; } = null!;

    [DisplayName("Наименование")]
    public string Name { get; set; } = null!;

    [DisplayName("Обозначение")]
    public string? Shifr { get; set; }
    [DisplayName("Кол-во")]
    public string? Count { get; set; }

    [DisplayName("Масса 1 п.м., кг")]
    public string? Weight { get; set; }

    [DisplayName("Примечание")]
    public string? Note { get; set; }

    public int IzdelieId { get; set; }
    public int ExtCompId { get; set; }
    public virtual Izdelie IzdelieIdNavigation { get; set; } = null!;
    public virtual ExternalComponent ExternalComponentIdNavigation { get; set; } = null!;
}
