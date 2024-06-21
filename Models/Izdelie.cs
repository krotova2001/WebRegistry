using System.ComponentModel.DataAnnotations;

namespace WebRegistry.Models;

public partial class Izdelie
{
    public int Id { get; set; }

    [Display(Name = "Шифр"), Required, StringLength(100), MinLength(3, ErrorMessage = "Слишком короткий путь")]
    public string Shifr { get; set; } = null!;

    [Display(Name = "Сетевой путь к файлу чертежа"), Required, MinLength(3, ErrorMessage = "Слишком маленькая длина пути")]
    public string FilePath { get; set; } = null!;

    public string? JX { get; set; }

    public string? WX { get; set; }

    public string? JY { get; set; }

    public string? WY { get; set; }

    [Display(Name = "Площадь сечения, см2")]
    public string? Square { get; set; }

    [Display(Name = "Масса 1 м длины, кг (Масса 1 п.м. общая)")]
    public string? Weight { get; set; }

    [Display(Name = "Периметр внешний, мм")]
    public string? Perimeter { get; set; }

    [Display(Name = "Диаметр описанной окружности, мм")]
    public string? CircleDiametr { get; set; }

    [Display(Name = "Группа сложности")]
    public string? DifficultyGroup { get; set; }

    [Display(Name = "ГОСТ")]
    public string? Gost { get; set; }

    [Display(Name = "Разраб.")]
    public string? Razrab { get; set; }

    [Display(Name = "Провер.")]
    public string? Prov { get; set; }

    public string? Tkontr { get; set; }
   
    public string? Nkontr { get; set; }

    [Display(Name = "Мастер")]
    public string? Master { get; set; }

    [Display(Name = "Утвер.")]
    public string? Utverd { get; set; }

    [Display(Name = "Шифр заказчика")]
    public string? CustomerShifr { get; set; }

    [Display(Name = "Категория")]
    public int? IdCategory { get; set; }

    [Display(Name = "Масса AL 1 п.м., кг")]
    public string? WeightAl { get; set; }

    [Display(Name = " Артикул 1С")]
    public string? Articul { get; set; }

    [Display(Name = "Содержит ли покупные изделия")]
    public sbyte? ContainExtComponent { get; set; }

    [Display(Name = "Составное изделие")]
    public sbyte? ContainDetails { get; set; }


    [Display(Name = "Примечание")]
    public string? Note { get; set; }

    [Display(Name = "Снято")]
    public sbyte? IsArchived { get; set; }

    [Display(Name = "Шифр заказчика")]
    public string? CustomerName { get; set; }

    [Display(Name = "Зарезервировано перед запуском")]
    public sbyte? IsReserved { get; set; } 


    [Display(Name = "Категория")]
    public virtual Category? IdCategoryNavigation { get; set; }


    [Display(Name = "Изделия в составе")]
    public virtual ICollection<Detail> DetailIdParentNavigations { get; set; } = new List<Detail>();

    [Display(Name = "Входит в изделия")]
    public virtual ICollection<Detail> DetailIdPartNavigations { get; set; } = new List<Detail>();

    [Display(Name = "Покупные изделия в составе")]
    public virtual ICollection<ExtCompInIzdelie> ExtcomponentsNavigations { get; set; } = new List<ExtCompInIzdelie>();
  
    [Display(Name = "Входит в системы")]
    public virtual ICollection<IzdelieSystema> SystemasNavigation { get; set; } = new List<IzdelieSystema>();
}
