using System;
using System.Collections.Generic;
using Zentech.Repositories;
using Zentech.Models;
using ZentechAPI.Models;
using ZentechAPI.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Concurrent;

namespace Zentech.Services
{
    public class NewsService
    {
        private readonly NewsRepository _newsRepository;

        public NewsService(NewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

       
        public List<News> GetAllNews()
        {
            return _newsRepository.GetAllNews();
        }

       
        public News GetNewsById(int id)
        {
            return _newsRepository.GetNewsById(id);
        }

        
        public NewsDto AddNews(NewsDto news, string createdBy)
        {
            if (news == null) throw new ArgumentNullException(nameof(news));
            if (string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Content))
                throw new ArgumentException("Title and content are required for the news.");

            return _newsRepository.AddNews(news, createdBy);
        }

      
        public bool UpdateNews(NewsDto news, string updatedBy)
        {
            if (news == null) throw new ArgumentNullException(nameof(news));

            return _newsRepository.UpdateNews(news, updatedBy);
        }

        
        public void DeleteNews(int newsId)
        {
            if (newsId <= 0) throw new ArgumentException("Invalid news ID.");

            _newsRepository.DeleteNews(newsId);
        }

       
        public void AddPhotoToNews(int newsId, string photoUrl)
        {
            if (newsId <= 0 || string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid news ID or photo URL.");

            _newsRepository.AddPhoto(newsId, "News", photoUrl); 
        }

        
        public void DeletePhotoFromNews(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid photo URL.");

            _newsRepository.DeletePhoto(photoUrl); 
        }

        public List<News> GetNewsByCategoryId(int category_id)
        {
            return _newsRepository.GetNewsByCategoryId(category_id);
        }



        #region TD_Category Methods
       
        // Get all news categories
      public async Task<ConcurrentBag<Other_Category>> GetNewsCategoriesAsync()
        {
            return await _newsRepository.GetNewsCategoriesAsync();
        }
       

        // Add a news category
        public async Task<Other_Category> AddNewsCategoryAsync(Other_Category category)
        {
            return await _newsRepository.AddNewsCategoryAsync(category);
        }

        // Update a news category
        public async Task<Other_Category> UpdateNewsCategoryAsync(Other_Category category)
        {
            return await _newsRepository.UpdateNewsCategoryAsync(category);
        }

        // Delete a news category
        public async Task<bool> DeleteNewsCategoryAsync(int categoryId)
        {
            return await _newsRepository.DeleteNewsCategoryAsync(categoryId.ToString());
        }



        #endregion
    }
}
