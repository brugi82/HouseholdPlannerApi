using HouseholdPlanner.Contracts.Services;
using HouseholdPlanner.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Services
{
	public class FamilyService : IFamilyService
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ILogger<FamilyService> _logger;

		public FamilyService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<FamilyService> logger)
		{
			_unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Add(string ownerId, string name, string description)
		{
			try
			{
				using (var unitOfWork = _unitOfWorkFactory.Create())
				{
					var member = await unitOfWork.MemberRepository.GetAsync(ownerId);
					if (member == null)
						throw new ArgumentException($"Member with given id {ownerId} not found");

					unitOfWork.FamilyRepository.Add(new Data.Models.Family()
					{
						Name = name,
						Description = description,
						Members = new List<Data.Models.Member>() { member }
					});

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
