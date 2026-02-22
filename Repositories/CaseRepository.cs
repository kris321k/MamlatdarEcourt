using Microsoft.AspNetCore.Identity;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.DTOS;
using StackExchange.Redis;
using MamlatdarEcourt.Services;
using MamlatdarEcourt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace MamlatdarEcourt.Repositories
{

    public class CaseRepository
    {
        private readonly AppDbContext _context;
        private readonly LitigantRepository _litigant;

        public CaseRepository(AppDbContext context, LitigantRepository litigant)
        {
            _context = context;
            _litigant = litigant;

        }

        public async Task<Case> CreateCaseAsync(CaseRegisterDto c, string applicantId)
        {
            var caseEntity = new Case
            {
                CaseNumber = c.CaseNumber,
                Title = c.Title,
                DisputeCategory = c.DisputeCategory,
                ApplicantId = applicantId,
                Status = CaseStatus.Filed,        
                FiledDate = DateTime.UtcNow       
            };

            _context.Case.Add(caseEntity);  
            await _context.SaveChangesAsync();

            return caseEntity;              
        }

        public async Task<Case?> FindByCaseNumber(string CaseNumber)
        {
            return await _context.Case.FirstOrDefaultAsync(c => c.CaseNumber == CaseNumber);
        }

        public async Task<IEnumerable<Case>> FindCaseByApplicantId(string UserId)
        {
            return await _context.Case.Where(c => c.ApplicantId == UserId).ToListAsync();
        }


        public async Task<string> GetLastSerialAsync(string ApplicantId)
        {
            var user = await _litigant.FindUserById(ApplicantId);

            if (user == null)
                throw new Exception("User not found");

            if (string.IsNullOrEmpty(user.Taluka))
                throw new Exception("User taluka not defined");

            var taluka = user.Taluka.ToUpper();
            var year = DateTime.UtcNow.Year;

            var lastCase = await _context.Case
                .Where(c => c.CaseNumber.StartsWith($"MAM-{taluka}-{year}"))
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            int newSerial = 1;

            if (lastCase != null)
            {
                var parts = lastCase.CaseNumber.Split('-');
                var lastSerial = int.Parse(parts.Last());
                newSerial = lastSerial + 1;
            }
            return $"MAM-{taluka}-{year}-{newSerial:D6}";
        }
    }
}