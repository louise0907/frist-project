
using Ecomwed.Models.custermor;
using Ecomwed.Models.product;

namespace Ecomwed.viewmodel
{
    public class cate_pro
    {
        public products products {  get; set; }
        public List<categories> categories { get; set; }

        public int? categoriesid {  get; set; }
    }
}

