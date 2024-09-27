using EcoMart.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace EcoMart.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                // --> Category
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Category>()
                    {
                        new Category()
                        {
                            CategoryName = "Food & Beverages",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Fashion & Accessories",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Electronics",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Health & Beauty",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Home & Kitchen",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Sports & Outdoors",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Automotive",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Toys, Hobbies, & Collectibles",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Office & School Supplies",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Baby & Kids",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Jewelry & Accessories",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Digital Products & Services",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Pets Supplies",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        },
                        new Category()
                        {
                            CategoryName = "Art Supplies & Stationery",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        }
                    });
                    context.SaveChanges();
                }

                // --> Product
                if (!context.Products.Any())
                {
                    context.Products.AddRange(new List<Product>()
                    {
                        new Product()
                        {
                            ProductName = "Kanzler Crispy Chicken Nugget 450gr",
                            Description = "Kanzler Crispy Chicken Nugget adalah nugget ayam premium yang dibuat dengan daging pilihan, tanpa pengawet dan MSG.",
                            Price = 37900,
                            StockQuantity = 10,
                            CategoryId = 1,
                            ImageUrl = "https://down-id.img.susercontent.com/file/id-11134207-7r991-lwvm9nfrm3ka4e",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        },
                        new Product()
                        {
                            ProductName = "Basreng Pedas Daun Jeruk 1kg",
                            Description = "Basreng pedas daun jeruk terlaris ternyata dari ikan tenggiri dan bahan-bahan pilihan lainnya sehingga membuat cita rasa basreng yang membuat ketagihan saat dimakan.",
                            Price = 54999,
                            StockQuantity = 10,
                            CategoryId = 1,
                            ImageUrl = "https://down-id.img.susercontent.com/file/id-11134207-7r991-lw104jooy7mm4f",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        },
                        new Product()
                        {
                            ProductName = "Oli Mpx2 All Matic Honda",
                            Description = "1pcs oli mpx2 800ml all matic honda.",
                            Price = 33700,
                            StockQuantity = 10,
                            CategoryId = 7,
                            ImageUrl = "https://down-id.img.susercontent.com/file/sg-11134201-22120-zzmxo5oo0hlv4b",
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        }
                    });
                    context.SaveChanges();
                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                // Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                // Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "admin@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        UserName = "admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "Admin123.");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }
            }
        }
    }
}
