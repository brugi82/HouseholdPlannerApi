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
        void Add(Member member);
        void Add(string id, string firstName, string lastName);
        Task<Member> Get(string id);
    }
}
