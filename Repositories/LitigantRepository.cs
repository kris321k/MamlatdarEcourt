using Microsoft.AspNetCore.Identity;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.DTOS;
using StackExchange.Redis;

namespace MamlatdarEcourt.Repositories
{
    public class LitigantRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LitigantRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

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
            if (string.IsNullOrWhiteSpace(email))
            {

                return null;

            }
            return await _userManager.FindByEmailAsync(email);
        }


        public async Task<IdentityResult> AddToRoleAsync(User user, string rolename)
        {
            return await _userManager.AddToRoleAsync(user, rolename);
        }
    }
}
