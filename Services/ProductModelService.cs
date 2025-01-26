using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using ZentechAPI.Models;
using ZentechAPI.Repositories;

public class ProductModelService
{
    private readonly ProductModelRepository _repository;

    public ProductModelService(ProductModelRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductModel>> GetAllModels(int offset, int limit)
    {
        return await Task<List<ProductModel>>.Run(() => {
            return _repository.GetAllModels(offset, limit);
        });
    }

    public async Task<ProductModel> GetModelById(int id)
    {
        return await Task<ProductModel>.Run(() => {
            return _repository.GetModelById(id);
        });
    }

    public async Task<List<ProductModel>> GetModelsByProductId(int prodId, int offset, int limit)
    {
        return await Task<List<ProductModel>>.Run(() => {
            return _repository.GetModelsByProductId(prodId, offset, limit);
        });
    }

    public async Task<int> AddModel(ProductModel model, string createdBy)
    {
        return await Task<int>.Run(() => {
            return _repository.AddModel(model, createdBy);
        });
    }

    public async Task<bool> UpdateModel(ProductModel model, string updatedBy)
    {
       return await Task<bool>.Run(() => {
            return _repository.UpdateModel(model, updatedBy);
        });
    }

    public async Task<bool> RemoveModel(int modelId)
    {
        return await Task<bool>.Run(()=> { return _repository.RemoveModel(modelId); });
    }
}
