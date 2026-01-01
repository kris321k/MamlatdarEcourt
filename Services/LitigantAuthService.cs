using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.Repositories;
using Microsoft.AspNetCore.Identity;


namespace MamlatdarEcourt.Services
{
    public class LitigantAuthService
    {
        private readonly LitigantRepository _litigantRepository;
        private string rolename = "litigant";


        public LitigantAuthService(LitigantRepository litigantRepository)
        {
            _litigantRepository = litigantRepository;
        }

        public async Task<IdentityResult> RegisterLigitantAsync(UserRegister userDto)
        {
            var result = await _litigantRepository.CreateAsync(userDto);

            return result;
        }


        public async Task<User?> FindLigitantByEmail(string email)
        {
            return await _litigantRepository.GetByEmailAsync(email);
        }


        public async Task<IdentityResult> AddToRoleAsync(User user)
        {
            return await _litigantRepository.AddToRoleAsync(user, rolename);
        }


    }
}