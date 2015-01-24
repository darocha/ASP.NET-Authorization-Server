using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Onyx.Authorization.Models;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;

namespace Onyx.Authorization.Models
{
    /// <summary>
    /// Context for accessing the datastore
    /// </summary>
    /// <remarks>
    /// This class inherits from IdentityDbContext which provides the
    /// DbSet<> for IdentityUser and IdentityRole. We could supply a
    /// custom User type which is a descendant of IdentityUser, and
    /// IdentityDbContext would use that to create the DbSet<>, but
    /// for the purposes of this application it isn't needed, so we
    /// will provide the default IdentityUser
    /// </remarks>
    public class AuthorizationDb : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Creates a new instance of the context
        /// </summary>
        /// <remarks>
        /// This constructor assumes that the context will be named
        /// "AuthorizationDb" in your Web.config file.
        /// </remarks>
        public AuthorizationDb()
            : base("AuthorizationDb")
        {
        }

       

        /// <summary>
        /// Geographical regions for sales tracking purposes
        /// </summary>
        public virtual DbSet<Region> Regions { get; set; }
        /// <summary>
        /// Corporate employees
        /// </summary>
        public virtual DbSet<Employee> Employees { get; set; }
        /// <summary>
        /// Sales tracked at a regional level
        /// </summary>
        public virtual DbSet<Sale> Sales { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// Uses the fluent API to explicitly define some relations in the schema
        /// </summary>
        /// <param name="modelBuilder">Supplied by Entity Framework</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // allow the base we are overriding to do its work
            base.OnModelCreating(modelBuilder);
            // Each Region has 0-to-many sales, but each sale MUST have a region
            // If a region is deleted, it will cascade delete sales
            modelBuilder.Entity<Region>()
                .HasMany<Sale>(r => r.Sales)
                .WithRequired(s => s.Region)
                .WillCascadeOnDelete(true);
            // Each Employee belongs to a region, but Regions are unaware
            // of dependent employees
            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Region)
                .WithMany()
                .HasForeignKey(r => r.RegionId)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            
            
        }

        public static AuthorizationDb Create()
        {
            return new AuthorizationDb();
        }
    }
}