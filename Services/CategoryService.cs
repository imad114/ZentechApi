using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using Zentech.Repositories;

namespace Zentech.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _repository;

        public CategoryService(CategoryRepository repository)
        {
            _repository = repository;
        }

        
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await Task.Run(() => _repository.GetAllCategories());
        }

       
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await Task.Run(() => _repository.GetCategoryById(id));
        }

        
        public async Task<Category> AddCategoryAsync(Category category, string createdBy)
        {
            return await Task.Run(() => _repository.AddCategory(category, createdBy));
        }

        
        public async Task<bool> UpdateCategoryAsync(Category category, string updatedBy)
        {
            return await Task.Run(() => _repository.UpdateCategory(category, updatedBy));
        }

        
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                _repository.DeleteCategory(categoryId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
