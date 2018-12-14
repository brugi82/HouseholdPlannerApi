using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Data.Contracts
{
    public interface IUnitOfWork: IDisposable
    {
        IMemberRepository MemberRepository { get; }
        IFamilyRepository FamilyRepository { get; }
        IInvitationRepository InvitationRepository { get; }

        Task SaveAsync();
    }
}
