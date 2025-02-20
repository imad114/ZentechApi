using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using Zentech.Repositories;
using System.Collections.Concurrent;
using MySql.Data.MySqlClient;

namespace Zentech.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _repository;

        public CategoryService(CategoryRepository repository)
        {
            _repository = repository;
        }

        #region Sub Category 
        /// <summary>
        /// Récupère toutes les catégories.
        /// </summary>
        /// <returns>Liste des catégories.</returns>
        public async Task<ConcurrentBag<Category>> GetAllCategoriesAsync()
        {
            return await Task.Run(() => _repository.GetAllCategories());
        }

        /// <summary>
        /// Récupère une catégorie par ID.
        /// </summary>
        /// <param name="id">ID de la catégorie.</param>
        /// <returns>Catégorie correspondante ou null si non trouvée.</returns>
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await Task.Run(() => _repository.GetCategoryById(id));
        }

        /// <summary>
        /// Récupère une catégorie par ID.
        /// </summary>
        /// <param name="id">ID de la catégorie.</param>
        /// <returns>Catégorie correspondante ou null si non trouvée.</returns>
        public async Task<ConcurrentBag<Category>> GetCategoryByMainCategoryAsync(int id)
        {
            return await Task.Run(() => _repository.GetCategoryByMainCategory(id));
        }
        

        /// <summary>
        /// Ajoute une nouvelle catégorie.
        /// </summary>
        /// <param name="category">Catégorie à ajouter.</param>
        /// <param name="createdBy">Utilisateur qui a créé la catégorie.</param>
        /// <returns>Catégorie ajoutée.</returns>
        public async Task<Category> AddCategoryAsync(Category category, string createdBy)
        {
            return await Task.Run(() => _repository.AddCategory(category, createdBy));
        }

        /// <summary>
        /// Met à jour une catégorie existante.
        /// </summary>
        /// <param name="category">Catégorie avec les nouvelles données.</param>
        /// <param name="updatedBy">Utilisateur qui a modifié la catégorie.</param>
        /// <returns>True si la mise à jour a réussi, False sinon.</returns>
        public async Task<bool> UpdateCategoryAsync(Category category, string updatedBy)
        {
            return await Task.Run(() => _repository.UpdateCategory(category, updatedBy));
        }

        /// <summary>
        /// Supprime une catégorie par ID.
        /// </summary>
        /// <param name="categoryId">ID de la catégorie à supprimer.</param>
        /// <returns>True si la suppression a réussi, False sinon.</returns>
        public async Task<string> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                _repository.DeleteCategory(categoryId);
                return "success"; // Indique que la suppression a réussi
            }
            catch (MySqlException ex) when (ex.Number == 1451) // Erreur FOREIGN KEY
            {
                return "Cannot delete category because it is referenced in another table.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
        #endregion



        #region MAin Category 
        // Récupérer toutes les catégories principales
        public async Task<ConcurrentBag<MainProdCategory>> GetAllMainProdCategoriesAsync()
        {
            return await Task.Run(() => _repository.GetAllMainProdCategories());
        }

        // Récupérer une catégorie principale par ID
        public async Task<MainProdCategory?> GetMainProdCategoryByIdAsync(int id)
        {
            return await Task.Run(() => _repository.GetMainProdCategoryById(id));
        }

        // Ajouter une nouvelle catégorie principale
        public async Task<MainProdCategory> AddMainProdCategoryAsync(MainProdCategory category, string createdBy)
        {
            return await Task.Run(() => _repository.AddMainProdCategory(category, createdBy));
        }

        // Mettre à jour une catégorie principale
        public async Task<bool> UpdateMainProdCategoryAsync(MainProdCategory category, string updatedBy)
        {
            return await Task.Run(() => _repository.UpdateMainProdCategory(category, updatedBy));
        }

        // Supprimer une catégorie principale
        public async Task<string> DeleteMainProdCategoryAsync(int categoryId)
        {
            try
            {
                await Task.Run(() => _repository.DeleteMainProdCategory(categoryId));
                return "success"; // Suppression réussie
            }
            catch (MySqlException ex) when (ex.Number == 1451) // Contrainte FOREIGN KEY
            {
                return "Cannot delete this category because it is referenced in another table.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
        #endregion
    }
}
