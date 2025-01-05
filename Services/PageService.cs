using System;
using System.Collections.Generic;
using ZentechAPI.Dto;
using ZentechAPI.Models;
using Zentech.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Zentech.Services
{
    public class PageService
    {
        private readonly PageRepository _pageRepository;

        public PageService(PageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        // Method to retrieve all pages
        public List<Pages> GetAllPages()
        {
            return _pageRepository.GetAllPages();
        }

        // Method to retrieve a page by its ID
        public Pages GetPageById(int id)
        {
            return _pageRepository.GetPageById(id);
        }

        // Method to retrieve a page by its Slug
        public Pages GetPageBySlug(string slug)
        {
            return _pageRepository.GetPageBySlug(slug);
        }

        // Method to add a new page
        public int AddPage(PageDto pageDto, string createdBy)
        {
           
            return _pageRepository.AddPage(pageDto, createdBy);
        }

        // Method to update a page
        public bool UpdatePage(PageDto pageDto, string updatedBy)
        {
          
            return _pageRepository.UpdatePage(pageDto, updatedBy);
        }
        // Increment Visitor Count
        public void IncrementVisitorCount(int pageId)
        {
            
            _pageRepository.IncrementVisitorCount(pageId);
        }


        // Method to delete a page
        public void DeletePage(int pageId)
        {
        
            _pageRepository.DeletePage(pageId);
        }
    }
}