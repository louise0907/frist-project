using Ecomwed.Models.custermor;

namespace Ecomwed.viewmodel
{
    public class inv
    {
        public customers customers { get; set; }
        public customerprofile? profile { get; set; }
        public List<detail> detail { get; set; }
       
        public tanshhistory tanshhistory { get; set; }
    }
}
