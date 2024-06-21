using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models;

public partial class Roles
{
    public int iduser_roles { get; set; }

    [Display(Name = "Роль")]
    public string? role { get; set; }

    [Display(Name = "Описание")]
    public string? description { get; set; }

    [Display(Name = "Пользователи в этой роли")]
    public virtual ICollection<User_roles> User_Roles { get; set; } = new List<User_roles>();

}
