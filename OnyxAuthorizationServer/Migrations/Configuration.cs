namespace Onyx.Authorization.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Onyx.Authorization.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using Onyx.Authorization;
    using System.Data.Entity.Core;
    using TwoFactorAuthentication.API.Services;
    using System.Data.Entity.Validation;
    using System.IdentityModel.Claims;
    using System.Web;

    /// <summary>
    /// Class handling configuration of migrations
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<AuthorizationDb>
    {
        /// <summary>
        /// Migrations configuration constructor
        /// </summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        /// <summary>
        /// Adds a default "admin" user to the database with a password of "password"
        /// </summary>
        /// <param name="context">A datastore access context</param>
        /// <returns>Whether adding the user exists in the database</returns>
        private bool AddUser(AuthorizationDb context)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string roleName = "Admin";

            // Identity result objects handle results from non-select operations
            // on the user datastore
            IdentityResult identityResult = null;

            // UserManager handles the management of a certain type of User, and it
            // requires a UserStore to handle the actual access to the datastore
            //UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            try
            {

                //Create Role Admin if it does not exist
                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    role = new IdentityRole(roleName);
                    var roleresult = roleManager.Create(role);
                }

                // Create a new user as a POCO
                var user = userManager.FindByName("admin");// new ApplicationUser()
                
                // Check if the user already exists before creating it
                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = "admin",
                        Email = "m@darocha.com",
                        TwoFactorEnabled = true,
                        EmailConfirmed = true,
                        PSK = TimeSensitivePassCode.GeneratePresharedKey(),
                        Profile = new Profile()
                        {
                            FirstName = "Marcelo",
                            LastName = "Darocha"
                        }
                    };

                    // Attempt to create the user
                    identityResult = userManager.Create(user, "Password1!");
                    
                }

                // Add user admin to Role Admin if not already added
                var rolesForUser = userManager.GetRoles(user.Id);
                if (!rolesForUser.Contains(role.Name))
                {
                    var result = userManager.AddToRole(user.Id, role.Name);
                }

            }
            catch (DbEntityValidationException e)
            {
                var message = e.InnerException.Message;
            }

            // Pass back the result of attempting to create the user
            return identityResult.Succeeded;

        }

        /// <summary>
        /// Add some default sales data to the databse
        /// </summary>
        /// <param name="context">A datastore access context</param>
        private void AddSales(AuthorizationDb context)
        {
            // Grab some default regions
            Region northAmerica = context.Regions.Single(r => r.Name == "North America");
            Region europe = context.Regions.Single(r => r.Name == "Europe");
            // Create some default sales (amounts need to be unique)
            List<Sale> sales = new List<Sale> {
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 500m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 200.58m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 300.75m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 1300.45m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 5000.75m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 800.46m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 400.10m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 1070.67m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 657.25m
                },
                new Sale {
                    RegionId = northAmerica.Id,
                    Amount = 142.25m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 1000.46m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 102.15m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 500.46m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 400.00m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 360.00m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 780.00m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 605.67m
                },
                new Sale {
                    RegionId = europe.Id,
                    Amount = 455.42m
                }
            };
            // Add the sales to the database (unless the amount exists)
            sales.ForEach(sale => context.Sales.AddOrUpdate(s => s.Amount, sale));
        }

        /// <summary>
        /// Function that will be run on upwards migrations of the database
        /// </summary>
        /// <param name="context">A datastpre access context</param>
        protected override void Seed(AuthorizationDb context)
        {
            // Add a default user to the database
            AddUser(context);

            // Define some default regions
            List<Region> regions = new List<Region> {
                new Region {
                    Name = "North America",
                    SalesTarget = 9000m
                },
                new Region
                {
                    Name = "Europe",
                    SalesTarget = 6000m
                }
            };
            // Add them to the context, unless they already exist
            regions.ForEach(region => context.Regions.AddOrUpdate(r => r.Name, region));
            // Then save them to the database
            context.SaveChanges();
            // Now grab the database versions
            Region northAmerica = context.Regions.Single(r => r.Name == "North America");
            Region europe = context.Regions.Single(r => r.Name == "Europe");
            // Define some default employees
            List<Employee> employees = new List<Employee> {
                new Employee
                {
                    FirstName = "Sarah",
                    LastName = "Doe",
                    RegionId = northAmerica.Id
                },
                new Employee
                {
                    FirstName = "John Q.",
                    LastName = "Public",
                    RegionId = europe.Id
                }
            };
            // Add them to the context, unless they already exist
            employees.ForEach(employee => context.Employees.AddOrUpdate(e => e.LastName, employee));
            // Then save them to the database
            context.SaveChanges();
            // Now grab the database versions
            Employee sarahDoe = context.Employees.Single(e => e.FirstName == "Sarah" && e.LastName == "Doe");
            Employee johnPublic = context.Employees.Single(e => e.FirstName == "John Q." && e.LastName == "Public");
            // And make them sales directors
            northAmerica.SalesDirector = sarahDoe;
            europe.SalesDirector = johnPublic;
            // Save their "promotions" to the database
            context.SaveChanges();
            // if there aren't any sales in the database currently, add some
            if (context.Sales.Count() == 0)
            {
                AddSales(context);
            }
            // save any sales that were added
            context.SaveChanges();

            if (context.Clients.Count() == 0)
            {
                context.Clients.AddRange(BuildClientsList());
                context.SaveChanges();
            }
        }



     

        private static List<Client> BuildClientsList()
        {

            List<Client> ClientsList = new List<Client> 
            {
                new Client
                { Id = "ngAuthApp", 
                    Secret= Helper.GetHash("abc@123"), 
                    Name="AngularJS front-end Application", 
                    ApplicationType =  Models.ApplicationTypes.JavaScript, 
                    Active = true, 
                    RefreshTokenLifeTime = 7200, 
                    AllowedOrigin = "http://onyximports.com.br"
                },
                new Client
                { Id = "consoleApp", 
                    Secret=Helper.GetHash("123@abc"), 
                    Name="Console Application", 
                    ApplicationType =Models.ApplicationTypes.NativeConfidential, 
                    Active = true, 
                    RefreshTokenLifeTime = 14400, 
                    AllowedOrigin = "*"
                }
            };

            return ClientsList;
        }
    }
}