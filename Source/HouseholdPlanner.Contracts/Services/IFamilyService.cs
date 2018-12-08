using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Services
{
    public interface IFamilyService
    {
		Task Add(string ownerId, string name, string description);
    }
}
