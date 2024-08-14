using Ecomwed.Models.custermor;

namespace Ecomwed.Models.seller
{
    public class sellers
    {
        public int ID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime laslogintime { get; set; }
        public DateTime logintimenow { get; set; }
        public sellersstatus status { get; set; }
        public int? sellerprofileID {  get; set; } //classname+ID
        public sellerprofile? profile { get; set; }
        public sellers()
        {
            laslogintime = DateTime.UtcNow;
            logintimenow = DateTime.UtcNow;
            
        }
    }
    public enum sellersstatus
    {
         Active, Inactive
    }
}
