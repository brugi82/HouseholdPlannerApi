using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Data.EntityFramework.Infrastructure;
using HouseholdPlanner.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Repositories
{
    public class FamilyRepository: EfRepositoryBase<Family>, IFamilyRepository
    {
        public FamilyRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
