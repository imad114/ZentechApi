using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Models;
using Zentech.Services;

namespace ZentechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyInformationController : ControllerBase
    {
        private readonly CompanyInformationService _companyInformationService;

        public CompanyInformationController(CompanyInformationService companyInformationService)
        {
            _companyInformationService = companyInformationService;
        }

        /// <summary>
        /// Get all company information.
        /// </summary>
        /// <response code="200">Returns the list of all companies</response>
        /// <response code="404">No companies found</response>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all company information", Description = "Retrieves all the company information available.")]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _companyInformationService.GetAllCompaniesAsync();
                if (companies == null || companies.Count == 0)
                {
                    return NotFound("No companies found.");
                }
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get company information by ID.
        /// </summary>
        /// <param name="id">ID of the company</param>
        /// <response code="200">Returns the company with the specified ID</response>
        /// <response code="404">Company not found</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get company information by ID", Description = "Retrieves the company information based on the provided ID.")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            try
            {
                var company = await _companyInformationService.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    return NotFound($"Company with ID {id} not found.");
                }
                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a new company information.
        /// </summary>
        /// <param name="company">The company to add</param>
        /// <response code="201">Company successfully created</response>
        /// <response code="400">Invalid company data</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Add a new company", Description = "Creates a new company in the system.")]
        public async Task<IActionResult> AddCompany([FromBody] CompanyInformation company)
        {
            try
            {
                if (company == null)
                {
                    return BadRequest("Company data is required.");
                }

                await _companyInformationService.AddCompanyAsync(company);
                return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update existing company information.
        /// </summary>
        /// <param name="id">ID of the company</param>
        /// <param name="company">The company data to update</param>
        /// <response code="204">Company successfully updated</response>
        /// <response code="400">Invalid company data</response>
        /// <response code="404">Company not found</response>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update existing company information", Description = "Updates the information of an existing company.")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyInformation company)
        {
            try
            {
                if (company == null)
                {
                    return BadRequest("Company data is required.");
                }

                company.Id = id; // Ensure the ID from the URL is used for the update
                await _companyInformationService.UpdateCompanyAsync(company);
                return NoContent(); // 204 No Content, successful update
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
