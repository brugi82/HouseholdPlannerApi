using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Data.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepositoryFactory _repositoryFactory;

        public UnitOfWork(IRepositoryFactory repositoryFactory, ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));

            MemberRepository = _repositoryFactory.CreateRepository<IMemberRepository>(new object[] { _dbContext });
            FamilyRepository = _repositoryFactory.CreateRepository<IFamilyRepository>(new object[] { _dbContext });
        }

        public IMemberRepository MemberRepository { get; private set; }

        public IFamilyRepository FamilyRepository { get; private set; }


        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
