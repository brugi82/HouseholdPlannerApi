using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Data.EntityFramework.Infrastructure;
using HouseholdPlanner.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.EntityFramework.Repositories
{
    public class HouseholdTaskRepository: EfRepositoryBase<HouseholdTask>, IHouseholdTaskRepository
    {
		public HouseholdTaskRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
		{

		}

		public Task<IEnumerable<HouseholdTask>> GetFamilyTasks(string familyId)
		{
			if (string.IsNullOrEmpty(familyId))
				throw new ArgumentNullException(nameof(familyId));

			return GetFamilyTasksAsync(familyId);
		}

		public Task<IEnumerable<HouseholdTask>> GetUsersFamilyTasks(string userId)
		{
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));

			return GetUsersFamilyTasksAsync(userId);
		}

		private async Task<IEnumerable<HouseholdTask>> GetUsersFamilyTasksAsync(string userId)
		{
			var member = await _dbContext.Members.FirstOrDefaultAsync(u => u.Id == userId);
			if (member != null)
				return await GetFamilyTasksAsync(member.FamilyId);
			else
				return new List<HouseholdTask>();
		}

		private async Task<IEnumerable<HouseholdTask>> GetFamilyTasksAsync(string familyId)
		{
			var familyTasks = await _dbContext.HouseholdTasks.Where(t => t.FamilyId == familyId).ToListAsync();
			return familyTasks;
		}
	}
}
