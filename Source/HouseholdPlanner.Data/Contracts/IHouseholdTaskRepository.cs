using HouseholdPlanner.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Contracts
{
    public interface IHouseholdTaskRepository: IRepository<HouseholdTask>
    {
		Task<IEnumerable<HouseholdTask>> GetFamilyTasks(string familyId);
		Task<IEnumerable<HouseholdTask>> GetUsersFamilyTasks(string userId);
    }
}
