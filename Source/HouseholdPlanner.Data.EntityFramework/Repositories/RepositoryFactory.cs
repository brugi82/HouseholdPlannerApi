using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Data.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public TRepository CreateRepository<TRepository>(object[] param) where TRepository: class
        {
            var repository = Activator.CreateInstance(typeof(TRepository), param) as TRepository;

            return repository;
        }
    }
}
