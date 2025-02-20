using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using Zentech.Repositories;
using ZentechAPI.Dto;
using MySql.Data.MySqlClient;

namespace Zentech.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        // get all Products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await Task.Run(() => _repository.GetAllProducts());
        }

        public async Task<List<Product>> GetProductsWithCategories(int limit)
        {
            return await Task.Run(() => _repository.GetProductsWithCategories(limit));
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await Task.Run(async () =>  await _repository.GetProductById(id));
        }

        
        public async Task<int> AddProductAsync(ProductDto product, string createdBy)
        {
            if (product == null || string.IsNullOrEmpty(createdBy))
                throw new ArgumentException("Invalid product or createdBy");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be empty.");

            return await Task.Run(() => _repository.AddProduct(product, createdBy));
        }


        public async Task<bool> UpdateProductAsync(ProductDto product, string updatedBy)
        {
            return await Task.Run(() => _repository.UpdateProduct(product, updatedBy));
        }


        public async Task<string> DeleteProductAsync(int id)
        {
            try
            {
                _repository.DeleteProduct(id);
                return "success";
            }
            catch (MySqlException ex) when (ex.Number == 1451) // Erreur clé étrangère
            {
                return "Cannot delete product because it is referenced in another table.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
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
