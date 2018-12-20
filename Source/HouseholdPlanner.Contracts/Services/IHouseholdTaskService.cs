using HouseholdPlanner.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Services
{
    public interface IHouseholdTaskService
    {
		Task Add(string userId, string title, string description);
		Task<IEnumerable<HouseholdTask>> GetUsersFamilyTasks(string userId);
    }
}
