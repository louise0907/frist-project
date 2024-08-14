using Ecomwed.Models.custermor;
using Ecomwed.Models.product;

namespace Ecomwed.viewmodel
{
    public class productdetailcs
    {
        public categories categories { get; set; }
        public products products { get; set; }
        public List<review> review { get; set; }
        public List<customers> customers { get; set; }
        public List<customerprofile>? profile { get; set; }

        public float AverageRating { get; set; }
    }
}
