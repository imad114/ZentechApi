using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using ZentechAPI.Repositories;

namespace Zentech.Services
{
    public class CompanyInformationService
    {
        private readonly CompanyInformationRepository _repository;

        public CompanyInformationService(CompanyInformationRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieve all company information.
        /// </summary>
        public async Task<List<CompanyInformation>> GetAllCompaniesAsync()
        {
            return await Task.Run(() => _repository.GetAll());
        }

        /// <summary>
        /// Retrieve a company by its ID.
        /// </summary>
        public async Task<CompanyInformation?> GetCompanyByIdAsync(int id)
        {
            return await Task.Run(() => _repository.GetById(id));
        }

        /// <summary>
        /// Add a new company information.
        /// </summary>
        public async Task AddCompanyAsync(CompanyInformation company)
        {
            if (company == null)
                throw new ArgumentException("Company cannot be null");

            await Task.Run(() => _repository.Add(company));
        }

      
        /// Update an existing company information.
        public async Task UpdateCompanyAsync(CompanyInformation company)
        {
            if (company == null)
                throw new ArgumentException("Company cannot be null");

            await Task.Run(() => _repository.Update(company));
        }
    }
}
