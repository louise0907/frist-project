using Ecomwed.Models.seller;
using Stripe;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecomwed.Models.product
{
    public class products
    {
        public int Id { get; set; }
        public string? image {  get; set; }
        public string productname { get; set; }
        public string? description  { get; set;}
        public decimal normalprice { get; set;}
        public int discount { get; set;}
        public decimal afterdiscountprice { get; set;}
        public int stock {  get; set;}
      

        
        public productstatus status { get; set; }
      
        public int sellersID {  get; set;}
        public sellers? sellers { get; set;}
       
        public categories? categories { get; set; }
        [ForeignKey(nameof(categories))]
        public int? categoriesId { get; set; }
        public products()
        {
            status = productstatus.Active;
        }
    }
}

public enum productstatus
{
    PendingReview,Active,Inactive,AdminDelete,SellerDelete
}