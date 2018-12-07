using HouseholdPlanner.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Services
{
    public interface IMemberService
    {
        Task Add(Member member);
        Task Add(string id, string firstName, string lastName);
        Task<Member> Get(string id);
    }
}
