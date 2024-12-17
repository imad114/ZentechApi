using System;
using System.Collections.Generic;
using Zentech.Repositories;
using Zentech.Models;
using ZentechAPI.Models;
using ZentechAPI.Dto;

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

        
        public NewsDto AddNews(NewsDto news)
        {
            if (news == null) throw new ArgumentNullException(nameof(news));
            if (string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Content))
                throw new ArgumentException("Title and content are required for the news.");

            return _newsRepository.AddNews(news);
        }

      
        public bool UpdateNews(NewsDto news)
        {
            if (news == null) throw new ArgumentNullException(nameof(news));

            return _newsRepository.UpdateNews(news);
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
    }
}
