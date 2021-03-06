﻿using HouseholdPlanner.Contracts.Services;
using HouseholdPlanner.Data.Contracts;
using HouseholdPlanner.Models.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Services
{
	public class HouseholdTaskService : IHouseholdTaskService
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ILogger<HouseholdTaskService> _logger;

		public HouseholdTaskService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<HouseholdTaskService> logger)
		{
			_unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task Add(string userId, string title, string description)
		{
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId));
			if (string.IsNullOrEmpty(title))
				throw new ArgumentNullException(nameof(title));

			return AddAsync(userId, title, description);
		}

		public async Task<IEnumerable<HouseholdTask>> GetUsersFamilyTasks(string userId)
		{
			try
			{
				using (var unitOfWork = _unitOfWorkFactory.Create())
				{
					var dataTasks = await unitOfWork.HouseholdTaskRepository.GetUsersFamilyTasks(userId);
					var tasks = dataTasks.Select(t => new HouseholdTask()
					{
						Id = t.Id,
						Title = t.Title,
						Description = t.Description,
						Created = t.Created,
						CreatedById = t.CreatedById,
						Updated = t.Updated,
						UpdatedById = t.UpdatedById,
						AssignedToId = t.AssignedToId,
						FamilyId = t.FamilyId
					});

					return tasks;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
		}

		private async Task AddAsync(string userId, string title, string description)
		{
			try
			{
				using (var unitOfWork = _unitOfWorkFactory.Create())
				{
					var user = await unitOfWork.MemberRepository.GetAsync(userId) ?? throw new ArgumentException($"User with id: {userId} not found.");
					var family = await unitOfWork.FamilyRepository.GetAsync(user.FamilyId) ?? throw new ArgumentException($"Family with id: {user.FamilyId} not found.");

					var newTask = new Data.Models.HouseholdTask()
					{
						Title = title,
						Description = description,
						CreatedBy = user,
						UpdatedBy = user,
						Created = DateTime.Now,
						Updated = DateTime.Now,
						Family = family
					};
					unitOfWork.HouseholdTaskRepository.Add(newTask);

					await unitOfWork.SaveAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
		}
	}
}
