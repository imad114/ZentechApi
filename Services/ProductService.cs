using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using Zentech.Repositories;
using ZentechAPI.Dto;

namespace Zentech.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

      
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await Task.Run(() => _repository.GetAllProducts());
        }

        
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await Task.Run(() => _repository.GetProductById(id));
        }

        
        public async Task<int> AddProductAsync(ProductDto product, string createdBy)
        {
            return await Task.Run(() => _repository.AddProduct(product, createdBy));
        }


        public async Task<bool> UpdateProductAsync(ProductDto product, string updatedBy)
        {
            return await Task.Run(() => _repository.UpdateProduct(product, updatedBy));
        }

      
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                _repository.DeleteProduct(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

      
        public void AddPhotoToProduct(int productId, string photoUrl)
        {
            if (productId <= 0 || string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid product ID or photo URL.");
            Console.WriteLine($"Adding photo: ProductId={productId}, PhotoUrl={photoUrl}");

            _repository.AddPhoto(productId, "Products", photoUrl); 
        }

        public void DeletePhotoFromProduct(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid photo URL.");

            _repository.DeletePhoto(photoUrl); 
        }
    }
}
