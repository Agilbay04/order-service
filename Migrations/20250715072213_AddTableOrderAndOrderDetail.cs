using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class AddTableOrderAndOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_notifications_users_user_id",
                table: "notifications");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "ix_notifications_user_id",
                table: "notifications");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    order_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    customer_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    customer_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    customer_phone_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ex_product_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ex_product_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    category_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ex_category_key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ex_category_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    ex_unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ex_total_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_details_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_details_id",
                table: "order_details",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_order_details_order_id_product_id_category_id_ex_product_code_ex_category_key",
                table: "order_details",
                columns: new[] { "order_id", "product_id", "category_id", "ex_product_code", "ex_category_key" });

            migrationBuilder.CreateIndex(
                name: "ix_orders_id",
                table: "orders",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_order_number",
                table: "orders",
                column: "order_number",
                unique: true,
                filter: "[order_number] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    key = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    key = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    password = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_publish = table.Column<bool>(type: "bit", nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    stock = table.Column<int>(type: "int", maxLength: 6, nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_Products_Categories",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    permission_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_key",
                table: "categories",
                column: "key",
                unique: true,
                filter: "[key] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_id",
                table: "permissions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_key",
                table: "permissions",
                column: "key",
                unique: true,
                filter: "[key] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_code",
                table: "products",
                column: "code",
                unique: true,
                filter: "[code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_products_id",
                table: "products",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_id",
                table: "role_permissions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_id",
                table: "roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_key",
                table: "roles",
                column: "key",
                unique: true,
                filter: "[key] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_id",
                table: "user_roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_id",
                table: "users",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_notifications_users_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
