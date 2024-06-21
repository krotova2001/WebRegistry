using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models;

public partial class User_roles
{
    [Key]
    public int idUserRole { get; set; }

    [Display(Name = "Логин")]
    public required string Login { get; set; }
    
    [Display(Name = "Идентификатор роли")]
    public int? roleId {  get; set; }

    [Display(Name = "ФИО"), MaxLength(450)]
    public string? Fio { get; set; }

    [Display(Name = "Роли")]
    public virtual Roles? UserRoleNavigation { get; set; }
}
