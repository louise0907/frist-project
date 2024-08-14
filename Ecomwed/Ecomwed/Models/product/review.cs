using Ecomwed.Models.custermor;
using Ecomwed.Models.seller;

namespace Ecomwed.Models.product
{
    public class review
    {
        public int Id { get; set; }
        public string Review { get; set; }
        public float StarRating { get; set; }
        public int customersID { get; set; }
        public customers? custermor { get; set; }
        public int productsId { get; set; }
        public products? products { get; set; }
        public DateTime reviewdate { get; set; }
    }
}
