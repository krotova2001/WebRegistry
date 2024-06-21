using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models;

/// <summary>
/// Категории изделий
/// </summary>
public partial class Category
{
    public int IdCategory { get; set; }

    [Display(Name = "Наименование категории")]
    public string Name { get; set; } = null!;

    [Display(Name = "Примечание")]
    public string? Description { get; set; }

    [Display(Name = "Изделия в этой категории")]
    public virtual ICollection<Izdelie> Izdelies { get; set; } = new List<Izdelie>();
}
