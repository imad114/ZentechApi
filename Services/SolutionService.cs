using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zentech.Repositories;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Services
{
    public class SolutionService
    {
        private readonly SolutionRepository _repository;

        public SolutionService(SolutionRepository repository)
        {
            _repository = repository;
        }

        // Method to retrieve all solution
        public async Task<List<Solution>> GetAllSolutionsAsync()
        {
            return await Task.Run(() => _repository.GetAllSolutions());
        }
        // Method to retrieve a solution by its ID, including photos and associated products
        public Solution GetSolutionById(int solutionId)
        {
            return _repository.GetSolutionById(solutionId);
        }


        public void AddProductsToSolution(int solutionId, List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                throw new ArgumentException("Product list cannot be empty.");

            foreach (var productId in productIds)
            {
                _repository.AddProductToSolution(solutionId, productId);
            }
        }

        public int AddSolution(SolutionDto solutionDto, string createdBy)
        {
            if (solutionDto == null)
                throw new ArgumentNullException(nameof(solutionDto));

            return _repository.AddSolution(solutionDto, createdBy);
        }


        public void AddPhotoToSolution(int SolutionId, string photoUrl)
        {
            if (SolutionId <= 0 || string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid product ID or photo URL.");
            Console.WriteLine($"Adding photo: SolutionId={SolutionId}, PhotoUrl={photoUrl}");

            _repository.AddPhoto(SolutionId, "Solutions", photoUrl);
        }

        public void UpdateSolution(SolutionDto solutionDto, string updatedBy)
        {
            // Additional validation or business logic can be added here
            if (solutionDto == null || solutionDto.SolutionID <= 0)
            {
                throw new ArgumentException("Invalid solution data.");
            }

            _repository.UpdateSolution(solutionDto, updatedBy);
        }

        public void DeletePhotoFromSolution(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid photo URL.");

            _repository.DeletePhoto(photoUrl);
        }


        // Supprimer une solution
        public async Task DeleteSolutionAsync(int solutionId)
        {
            await Task.Run(() => _repository.DeleteSolution(solutionId));
        }
    }
}
