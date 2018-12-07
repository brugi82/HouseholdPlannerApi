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
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IRepositoryFactory _repositoryFactory;
		private readonly ApplicationDbContext _dbContext;

		public UnitOfWork(IRepositoryFactory repositoryFactory, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));

			_dbContext = _dbContextFactory.Create();

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
