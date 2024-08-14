namespace Ecomwed.Models.custermor
{
    public class tanshhistory
    {
        public int id { get; set; }
        public string subtotal { get; set; }
        public string sst {  get; set; }
        public string grandtotal { get; set; }

        public DateTime time { get; set; }

        public int? customersID { get; set; }
        public customers? customers { get; set; }
        public historystatus status { get; set; }
      
    }
    public enum historystatus
    {
        Pending, Paid, Cancel
    }
}
