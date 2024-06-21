namespace WebRegistry.Models
{
    //заказчики
    public partial class Customer
    {
        public int Idcustomer { get; set; }
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
    }
}
