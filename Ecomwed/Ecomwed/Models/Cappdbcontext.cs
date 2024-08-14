using Ecomwed.Models.admin;
using Ecomwed.Models.custermor;
using Ecomwed.Models.product;
using Ecomwed.Models.seller;
using Microsoft.EntityFrameworkCore;

namespace Ecomwed.Models
{
    public class Cappdbcontext : DbContext
    {
       public Cappdbcontext() 
        { 
        }
            public DbSet<sellers> sellers { get; set; }
            public DbSet<sellerprofile> sellerprofile { get; set; }
            public DbSet<customers> customers { get; set; }
            public DbSet<customerprofile> customerprofile { get; set; }
            public DbSet<cart> cart { get; set; }
            public DbSet<products> products { get; set; }
        public DbSet<categories> categories { get; set; }
        public DbSet<admins> admins { get; set; }
        public DbSet<tanshhistory> tanshhistory { get; set; }
        public DbSet<detail> detail { get; set; }
        public DbSet<review> review {  get; set; }
        public Cappdbcontext(DbContextOptions options) : base(options)
            {

            }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)

        {    
            //optionBuilder.UseSqlServer(@"Data Source=SQL8006.site4now.net;Initial Catalog=db_aaa1ec_ecco;User Id=db_aaa1ec_ecco_admin;Password=UwW#8sCgrzjf8wT;MultipleActiveResultSets=true;");
            //optionBuilder.UseSqlServer(@"Server=LAPTOP-IJ6O0403\SQLEXPRESS;Database=Econwed;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true;");

            // 三个nuget packgage 6.0.11 version entityframework.core/tools/sqlserver
            //然后tools > manage nuget package
            //-- Add-migration intial
            //-- update-database
        }
    }
}

