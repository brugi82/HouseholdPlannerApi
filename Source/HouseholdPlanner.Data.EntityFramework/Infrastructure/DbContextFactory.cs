using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Infrastructure
{
    public class DbContextFactory: IDbContextFactory<ApplicationDbContext>
	{
		private readonly string _connectionString;

		public DbContextFactory(string connectionString)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		public ApplicationDbContext Create()
		{
			var options = new DbContextOptionsBuilder();
			options.UseSqlServer(_connectionString, b => b.MigrationsAssembly("HouseholdPlanner.Data.EntityFramework"));

			var newContext = new ApplicationDbContext(options.Options);

			return newContext;
		}
    }
}
