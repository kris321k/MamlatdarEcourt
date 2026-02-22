using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Models;
using MamlatdarEcourt.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;


namespace MamlatdarEcourt.Services
{

    public class CaseService
    {

        private readonly CaseRepository _caseRepo;

        public CaseService(CaseRepository caseRepo)
        {
            _caseRepo = caseRepo;
        }


        public async Task<Case> CreateCaseAsync(CaseRegisterDto c, string ApplicantId)
        {
            c.CaseNumber = await _caseRepo.GetLastSerialAsync(ApplicantId);

            return await _caseRepo.CreateCaseAsync(c, ApplicantId);
        }


        

    }
}