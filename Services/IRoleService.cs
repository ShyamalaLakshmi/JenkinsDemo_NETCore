using Entities;
using System.Threading.Tasks;

namespace Services
{
    public interface IRoleService
    {
        Task<(bool Succeeded, string ErrorMessage)> AddRoleAsync(UserEntity user, string password);
    }
}
