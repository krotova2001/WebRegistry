namespace WebRegistry.Models
{
    //промежуточная таблица для регистрации связи система-изделие
    public partial class IzdelieSystema
    {
        public int izdelieId { get; set; }
        public int systemId { get; set; }
        public virtual Systema IdSystemaNavigation { get; set; } = null!;

        public virtual Izdelie IdIzdelieNavigation { get; set; } = null!;
    }
}
