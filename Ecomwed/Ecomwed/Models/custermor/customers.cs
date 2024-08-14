using Ecomwed.Models.product;
using Ecomwed.Models.seller;

namespace Ecomwed.Models.custermor
{
    public class customers
    {
        public int ID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email {  get; set; }
        public DateTime laslogintime { get; set; }
        public DateTime logintimenow { get; set; }
        public customerstatus status { get; set; }

        public int? customerprofileID { get; set; } //classname+ID
        public customerprofile? profile { get; set; }
        public customers()
        {
            laslogintime = DateTime.UtcNow;
            logintimenow = DateTime.UtcNow;
            
        }
    }
    public enum customerstatus
    {
         Active, Inactive
    }
}
