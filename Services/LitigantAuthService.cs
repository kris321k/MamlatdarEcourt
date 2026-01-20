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

        private readonly TokenService _tokenService;


        public LitigantAuthService(LitigantRepository litigantRepository, TokenService tokenService)
        {
            _litigantRepository = litigantRepository;
            _tokenService = tokenService;

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

        public async Task<string?> LoginAsync(LoginDto _loginDto)
        {
            var user = await _litigantRepository.GetByEmailAsync(_loginDto.Email);

            if (user == null) return null;

            var PasswordIsvalid = await _litigantRepository.CheckPasswordAsync(user, _loginDto.Password);

            if (!PasswordIsvalid)
            {
                return null;
            }
            
            return await _tokenService.CreateTokenAsync(user);

        }
    }
}

