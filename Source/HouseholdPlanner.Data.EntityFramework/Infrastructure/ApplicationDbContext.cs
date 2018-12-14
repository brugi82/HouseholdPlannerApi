using HouseholdPlanner.Data.EntityFramework.Models;
using HouseholdPlanner.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
              : base(options)
        {
        }

        public DbSet<Family> Families { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
    }
}
