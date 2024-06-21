using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models;

public partial class Detail
{ 
    [Key]
    public int IdLink { get; set; }

    public int IdParent { get; set; }
   
    public int IdPart { get; set; }

    public string? Count { get; set; }

    public string? Note { get; set; }

    public virtual Izdelie IdParentNavigation { get; set; } = null!;

    public virtual Izdelie IdPartNavigation { get; set; } = null!;
}
