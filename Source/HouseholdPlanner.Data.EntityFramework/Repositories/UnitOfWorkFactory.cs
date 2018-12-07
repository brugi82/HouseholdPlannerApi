using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Data.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Repositories
{
	public class UnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
		private readonly IRepositoryFactory _repositoryFactory;

		public UnitOfWorkFactory(IRepositoryFactory repositoryFactory, IDbContextFactory<ApplicationDbContext> dbContextFactory)
		{
			_dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
			_repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
		}

		public IUnitOfWork Create()
		{
			var unitOfWork = new UnitOfWork(_repositoryFactory, _dbContextFactory);

			return unitOfWork;
		}
	}
}
