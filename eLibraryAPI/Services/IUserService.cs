using eLibraryAPI.Models.Dtos;
using eLibraryAPI.Models.Models;

namespace eLibraryAPI.Services
{
    public interface IUserService
    {
        public Task<bool> createUser(UserModel user);
        public Task<bool> deleteUser(string userGuid);
        public Task<List<UserDto>> getUsers(string adminId);
    }
}
