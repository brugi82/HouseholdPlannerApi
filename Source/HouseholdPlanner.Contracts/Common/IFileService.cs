using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Contracts.Common
{
    public interface IFileService
    {
        Task<string> GetFileContentAsync(string path);
    }
}
