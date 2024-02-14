using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maurice.Domain.Entities;

namespace Maurice.Application.Interfaces.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateAsync(EventEntity eventEntity, CancellationToken cancellationToken);
    }
}
