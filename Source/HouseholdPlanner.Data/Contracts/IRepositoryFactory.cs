using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Contracts
{
    public interface IRepositoryFactory
    {
        TRepository CreateRepository<TRepository>(object[] param) where TRepository: class;
    }
}
