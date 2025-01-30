using System.Collections.Concurrent;
using Zentech.Repositories;
using ZentechAPI.Models;
using ZentechAPI.Repositories;

public class TechnicalDocService
{
    private readonly TechincalDocRepository _repository;

    public TechnicalDocService(TechincalDocRepository repository)
    {
        _repository = repository;
    }

    #region Technical Documentation Methods

    public List<TechincalDoc> GetAllTechnicalDocs(int limit)
    {
        return _repository.GetAllTechnicalDocs(limit);
    }

    public TechincalDoc GetTechnicalDocByID(string id)
    {
        return _repository.GetTechnicalDocByID(id);
    }
    public List<TechincalDoc> GetAllTechnicalDocsWithCategories(int limit)
    {
        return _repository.GetAllTechnicalDocsWithCategories(limit);
    }
    public int AddTechnicalDoc(TechincalDoc technicalDoc, string createdBy)
    {
        return _repository.AddTechnicalDoc(technicalDoc, createdBy);
    }

    public bool UpdateTechnicalDoc(TechincalDoc technicalDoc, string updateBy)
    {
        return _repository.UpdateTechnicalDoc(technicalDoc, updateBy);
    }

    public void DeleteTechnicalDoc(int id)
    {
        _repository.DeleteTechnicalDoc(id);
    }

    #endregion

    #region TD_Category Methods

    public async Task<ConcurrentBag<Other_Category>> GetAllCategories()
    {
        return await _repository.GetTDCategories();
    }

    public async Task<Other_Category> AddTechnicalDocCategory(Other_Category category)
    {
        return await _repository.AddTDCategory(category);
    }

    public async Task<Other_Category>  UpdateTechnicalDocCategory(Other_Category category)
    {
        return await  _repository.UpdateTDCategory(category);
    }

    

    public async Task<int> DeleteTechnicalDocCategory(int categoryId)
    {
        return await _repository.DeleteTDCategory(categoryId.ToString());
    }



    #endregion
}

