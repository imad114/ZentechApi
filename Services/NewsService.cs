using System;
using System.Collections.Generic;
using Zentech.Repositories;
using Zentech.Models;
using ZentechAPI.Models;

namespace Zentech.Services
{
    public class NewsService
    {
        private readonly NewsRepository _newsRepository;

        public NewsService(NewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        // Récupérer toutes les actualités
        public List<News> GetAllNews()
        {
            return _newsRepository.GetAllNews();
        }

        // Récupérer une actualité spécifique par ID
        public News GetNewsById(int id)
        {
            return _newsRepository.GetNewsById(id);
        }

        // Ajouter une nouvelle actualité
        public News AddNews(News news)
        {
            if (news == null) throw new ArgumentNullException(nameof(news));
            if (string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Content))
                throw new ArgumentException("Title and content are required for the news.");

            return _newsRepository.AddNews(news);
        }

        // Mettre à jour une actualité
        public bool UpdateNews(News news)
        {
            if (news == null) throw new ArgumentNullException(nameof(news));

            return _newsRepository.UpdateNews(news);
        }

        // Supprimer une actualité et ses photos associées
        public void DeleteNews(int newsId)
        {
            if (newsId <= 0) throw new ArgumentException("Invalid news ID.");

            _newsRepository.DeleteNews(newsId);
        }

        // Ajouter une photo à une actualité
        public void AddPhotoToNews(int newsId, string photoUrl)
        {
            if (newsId <= 0 || string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid news ID or photo URL.");

            _newsRepository.AddPhoto(newsId, "News", photoUrl); // Utilise la table Photos générique
        }

        // Supprimer une photo d'une actualité
        public void DeletePhotoFromNews(string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
                throw new ArgumentException("Invalid photo URL.");

            _newsRepository.DeletePhoto(photoUrl); // Supprimer la photo via la méthode du repository
        }
    }
}
