using HouseholdPlanner.Contracts.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Services.Common
{
    public class FileService : IFileService
    {
        public async Task<string> GetFileContentAsync(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            return content;
        }
    }
}
