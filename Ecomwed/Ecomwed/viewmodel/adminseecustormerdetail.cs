using Ecomwed.Models.custermor;

namespace Ecomwed.viewmodel
{
    public class adminseecustormerdetail
    {
        public customers customers { get; set; }
        public customerprofile? profile { get; set; }
        public List<tanshhistory>? tanshhistory { get; set; }
        
    }
}
