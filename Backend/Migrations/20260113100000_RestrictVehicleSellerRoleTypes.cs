using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIBackend.Migrations
{
    /// <inheritdoc />
    public partial class RestrictVehicleSellerRoleTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Map deprecated seller role types to approved values
            // Approved values: 'Seller', 'Sellers', 'Sales Agent', 'Heirs'
            
            // Map 'Lessor' -> 'Seller' (single lessor becomes single seller)
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Seller' 
                WHERE ""RoleType"" = 'Lessor';
            ");
            
            // Map 'Lessors' -> 'Sellers' (multiple lessors become multiple sellers)
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Sellers' 
                WHERE ""RoleType"" = 'Lessors';
            ");
            
            // Map 'Seller in a revocable sale' -> 'Seller'
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Seller' 
                WHERE ""RoleType"" = 'Seller in a revocable sale';
            ");
            
            // Map 'Sellers in a revocable sale' -> 'Sellers'
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Sellers' 
                WHERE ""RoleType"" = 'Sellers in a revocable sale';
            ");
            
            // Map 'Lease Agent' -> 'Sales Agent'
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Sales Agent' 
                WHERE ""RoleType"" = 'Lease Agent';
            ");
            
            // Map 'Agent for a revocable sale' -> 'Sales Agent'
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Sales Agent' 
                WHERE ""RoleType"" = 'Agent for a revocable sale';
            ");
            
            // Set NULL or empty values to 'Seller' (default)
            migrationBuilder.Sql(@"
                UPDATE tr.""VehiclesSellerDetails"" 
                SET ""RoleType"" = 'Seller' 
                WHERE ""RoleType"" IS NULL OR ""RoleType"" = '';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reversible migration - no action needed as we cannot restore original values
            // The mapping is one-way but data is preserved (just categorized differently)
        }
    }
}
