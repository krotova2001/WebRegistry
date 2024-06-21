using System;
using System.Collections.Generic;

namespace WebRegistry.Models;

public partial class TempDetail
{
    public int IdLink { get; set; }

    /// <summary>
    /// Шифр родительской детали
    /// </summary>
    public string ParentShifr { get; set; } = null!;

    public string Shifr { get; set; } = null!;

    /// <summary>
    /// Наименование
    /// 
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// ГОСТ
    /// </summary>
    public string? Gost { get; set; }

    public int? Count { get; set; }

    /// <summary>
    /// Масса 1 п.м., кг
    /// </summary>
    public string? Weight { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// Ссылка на составной чертеж
    /// </summary>
    public int IdParent { get; set; }

    public virtual Izdelie IdParentNavigation { get; set; } = null!;
}
