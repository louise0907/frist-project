using Ecomwed.Models.product;

namespace Ecomwed.Models.custermor
{
    public class cart
    {
        public int Id { get; set; }
        public int quantity { get; set; }
        public bool isselected { get; set; }
        public int? productsId { get; set; }
        public products? products { get; set; }
        public int? customersID { get; set; }
        public customers? customers { get; set; }
        public cart()
        {
            isselected = false;
        }
    }
}
