using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using Zentech.Repositories;

namespace Zentech.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }

        // Obtenir tous les produits
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await Task.Run(() => _repository.GetAllProducts());
        }

        // Obtenir un produit spécifique par ID
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await Task.Run(() => _repository.GetProductById(id));
        }

        // Ajouter un nouveau produit
        public async Task<Product> AddProductAsync(Product product, string createdBy)
        {
            return await Task.Run(() => _repository.AddProduct(product, createdBy));
        }

        // Mettre à jour un produit existant
        public async Task<bool> UpdateProductAsync(Product product, string updatedBy)
        {
            return await Task.Run(() => _repository.UpdateProduct(product, updatedBy));
        }

        // Supprimer un produit
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

        // Ajouter une photo à un product
        public void AddPhotoToProduct(int productId, string photoUrl)
        {
            if (productId <= 0 || string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid product ID or photo URL.");

            _repository.AddPhoto(productId, "Products", photoUrl); // Utilise la table Photos générique
        }

        // Supprimer une photo d'un product
        public void DeletePhotoFromProduct(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid photo URL.");

            _repository.DeletePhoto(photoUrl); // Supprimer la photo via la méthode du repository
        }
    }
}
