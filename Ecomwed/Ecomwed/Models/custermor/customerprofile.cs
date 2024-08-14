using Humanizer;

namespace Ecomwed.Models.custermor
{
    public class customerprofile
    {
        public int ID { get; set; }
        public string fullname { get; set; }
        public string NRIC { get; set; }
        public string phonenumber { get; set; }
        public string? image {  get; set; }
        public string? age {  get; set; }
        public string birthday {  get; set; }
        public string address {  get; set; }
        public string gender { get; set; }
      
        public CountryStatus Status { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode {  get; set; }
    }
    public enum CountryStatus
    {
        Malaysia
    }
}
