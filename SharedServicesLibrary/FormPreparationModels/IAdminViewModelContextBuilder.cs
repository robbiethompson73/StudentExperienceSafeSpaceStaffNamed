using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedServicesLibrary.FormPreparationModels
{
    public interface IAdminViewModelContextBuilder
    {
        Task<AdminViewModelMappingContext> BuildAsync(int submissionId);
    }
}
