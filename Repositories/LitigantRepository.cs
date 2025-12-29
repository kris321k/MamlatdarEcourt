using Microsoft.AspNetCore.Identity;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.DTOS;

namespace MamlatdarEcourt.Repositories
{
    public class LitigantRepository
    {
        private readonly UserManager<User> _userManager;

        public LitigantRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(UserRegister userDto)
        {
            var user = new User
            {
                UserName = userDto.Email,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                DOB = userDto.DOB,
                Taluka = userDto.Taluka,
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            return result;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
