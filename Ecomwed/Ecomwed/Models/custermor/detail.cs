using Ecomwed.Models.product;

namespace Ecomwed.Models.custermor
{
    public class detail
    {
        public int Id { get; set; }
        public int QTY { get; set; }
        public decimal price { get; set;}
        public int? productsId { get; set; } 
        public products? products { get; set; }
        public int? tanshhistoryid { get; set; } 
        public tanshhistory? tanshhistory { get; set; }
    }
}
