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
        public List<AboutUs> GetAllPages()
        {
            return _pageRepository.GetAllPages();
        }

        // Method to retrieve a page by its ID
        public AboutUs GetPageById(int id)
        {
            return _pageRepository.GetPageById(id);
        }


        // Method to add a new page
        public int AddPage(AboutUs aboutAs, string createdBy)
        {
           
            return _pageRepository.AddPage(aboutAs, createdBy);
        }

        // Method to update a page
        public bool UpdatePage(AboutUs aboutAs, string updatedBy)
        {
          
            return _pageRepository.UpdatePage(aboutAs, updatedBy);
        }
       
        // Method to delete a page
        public void DeletePage(int pageId)
        {
        
            _pageRepository.DeletePage(pageId);
        }
    }
}