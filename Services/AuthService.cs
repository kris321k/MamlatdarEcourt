
using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Repositories;
using Microsoft.AspNetCore.Identity;

namespace MamlatdarEcourt.Services
{
    public class AuthService
    {
        private readonly LitigantRepository _repo;

        public AuthService(LitigantRepository repo)
        {
            _repo = repo;
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegister dto)
        {

            var existingUser = await _repo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "A user with this email already exists."
                });
            }

            // Create user
            var result = await _repo.CreateAsync(dto);
            return result;
        }
    }
}
