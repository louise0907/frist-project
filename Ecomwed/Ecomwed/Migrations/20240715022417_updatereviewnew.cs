using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecomwed.Migrations
{
    /// <inheritdoc />
    public partial class updatereviewnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoryname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customerprofile",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NRIC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    birthday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    zipcode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerprofile", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "sellerprofile",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NRIC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phonenumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    birthday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellerprofile", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    laslogintime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    logintimenow = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    customerprofileID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_customers_customerprofile_customerprofileID",
                        column: x => x.customerprofileID,
                        principalTable: "customerprofile",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "sellers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    laslogintime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    logintimenow = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    sellerprofileID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_sellers_sellerprofile_sellerprofileID",
                        column: x => x.sellerprofileID,
                        principalTable: "sellerprofile",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "tanshhistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subtotal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    grandtotal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    customersID = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tanshhistory", x => x.id);
                    table.ForeignKey(
                        name: "FK_tanshhistory_customers_customersID",
                        column: x => x.customersID,
                        principalTable: "customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    productname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    normalprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount = table.Column<int>(type: "int", nullable: false),
                    afterdiscountprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    sellersID = table.Column<int>(type: "int", nullable: false),
                    categoriesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_categories_categoriesId",
                        column: x => x.categoriesId,
                        principalTable: "categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_products_sellers_sellersID",
                        column: x => x.sellersID,
                        principalTable: "sellers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    isselected = table.Column<bool>(type: "bit", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: true),
                    customersID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cart_customers_customersID",
                        column: x => x.customersID,
                        principalTable: "customers",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_cart_products_productsId",
                        column: x => x.productsId,
                        principalTable: "products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "detail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QTY = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: true),
                    tanshhistoryid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detail_products_productsId",
                        column: x => x.productsId,
                        principalTable: "products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_detail_tanshhistory_tanshhistoryid",
                        column: x => x.tanshhistoryid,
                        principalTable: "tanshhistory",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StarRating = table.Column<float>(type: "real", nullable: false),
                    customersID = table.Column<int>(type: "int", nullable: false),
                    productsId = table.Column<int>(type: "int", nullable: false),
                    reviewdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_review_customers_customersID",
                        column: x => x.customersID,
                        principalTable: "customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_review_products_productsId",
                        column: x => x.productsId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cart_customersID",
                table: "cart",
                column: "customersID");

            migrationBuilder.CreateIndex(
                name: "IX_cart_productsId",
                table: "cart",
                column: "productsId");

            migrationBuilder.CreateIndex(
                name: "IX_customers_customerprofileID",
                table: "customers",
                column: "customerprofileID");

            migrationBuilder.CreateIndex(
                name: "IX_detail_productsId",
                table: "detail",
                column: "productsId");

            migrationBuilder.CreateIndex(
                name: "IX_detail_tanshhistoryid",
                table: "detail",
                column: "tanshhistoryid");

            migrationBuilder.CreateIndex(
                name: "IX_products_categoriesId",
                table: "products",
                column: "categoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_products_sellersID",
                table: "products",
                column: "sellersID");

            migrationBuilder.CreateIndex(
                name: "IX_review_customersID",
                table: "review",
                column: "customersID");

            migrationBuilder.CreateIndex(
                name: "IX_review_productsId",
                table: "review",
                column: "productsId");

            migrationBuilder.CreateIndex(
                name: "IX_sellers_sellerprofileID",
                table: "sellers",
                column: "sellerprofileID");

            migrationBuilder.CreateIndex(
                name: "IX_tanshhistory_customersID",
                table: "tanshhistory",
                column: "customersID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "detail");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "tanshhistory");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "sellers");

            migrationBuilder.DropTable(
                name: "customerprofile");

            migrationBuilder.DropTable(
                name: "sellerprofile");
        }
    }
}
