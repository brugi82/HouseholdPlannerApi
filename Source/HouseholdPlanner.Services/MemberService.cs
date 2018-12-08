using HouseholdPlanner.Contracts.Services;
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
	public class MemberService : IMemberService
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ILogger<MemberService> _logger;

		public MemberService(IUnitOfWorkFactory unitOfWorkFactory, ILogger<MemberService> logger)
		{
			_unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(_unitOfWorkFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Add(Member member)
		{
			try
			{
				await Add(member.Id, member.FirstName, member.LastName);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
		}

		public async Task Add(string id, string firstName, string lastName)
		{
			try
			{
				using (var unitOfWork = _unitOfWorkFactory.Create())
				{
					unitOfWork.MemberRepository.Add(new Data.Models.Member()
					{
						Id = id,
						FirstName = firstName,
						LastName = lastName
					});

					await unitOfWork.SaveAsync();
				}
			}
			catch(Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}
		}

		public Task<Member> Get(string id)
		{
			throw new NotImplementedException();
		}
	}
}
