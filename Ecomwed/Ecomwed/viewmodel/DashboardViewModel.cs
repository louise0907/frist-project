namespace Ecomwed.viewmodel
{
    public class DashboardViewModel
    {
        public int TotalSales { get; set; }
        public int TotalComments { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSellers { get; set; }
        public decimal EarningsToday { get; set; }
        public decimal EarningsYesterday { get; set; }
      
        public string DateRange { get; set; }
    }
}
