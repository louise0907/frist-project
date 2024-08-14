using Ecomwed.Models.product;

namespace Ecomwed.viewmodel
{
    public class categoriesview
    {
        public List<products> products {  get; set; }
        public List<categories> categories { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
