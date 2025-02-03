using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using Zentech.Repositories;
using ZentechAPI.Models;
using ZentechAPI.Repositories;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc.ModelBinding;


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

    public async Task<int> ProcessExcelFile(IFormFile file , string createdBy , int proudctId)
    {
        int recordsInserted = 0;

        try
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Ajout de la licence

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    if (worksheet == null)
                    {
                        throw new Exception("Excel file is empty or invalid.");
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    List<ProductModel> productModel = new List<ProductModel>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var model = worksheet.Cells[row, 1].Text;
                        var displacement = worksheet.Cells[row, 2].Text;
                        var coolingType = worksheet.Cells[row, 3].Text;
                        var motorType = worksheet.Cells[row, 4].Text;
                        var voltageFrequency = worksheet.Cells[row, 5].Text;
                        var coolingCapacityW = ParseDouble(worksheet.Cells[row, 6].Value);
                        var coolingCapacityBTUPerHour = ParseDouble(worksheet.Cells[row, 7].Value);
                        var coolingCapacityKcal = ParseDouble(worksheet.Cells[row, 8].Value);
                        var copWW = ParseDouble(worksheet.Cells[row, 9].Value);
                        var copBTUPerWH = ParseDouble(worksheet.Cells[row, 10].Value);

                        if (string.IsNullOrEmpty(model))
                        {
                            continue; // Ignore les lignes vides
                        }
                        // Vérifier si le modèle existe déjà dans la base de données
                        var existingModel = _repository.GetProductModelByNameAndProductId(proudctId, model);
                        //if (existingModel != null)
                        //{
                        //    // Supprimer l'ancien modèle avant d'ajouter le nouveau
                        //    continue; // Skips to the next model in the loop

                        //    //_repository.UpdateModel(existingModel, createdBy);
                        //}

                        if (existingModel != null)
                        {
                            // Mise à jour du modèle existant
                            existingModel.Displacement = displacement;
                            existingModel.CoolingType = coolingType;
                            existingModel.MotorType = motorType;
                            existingModel.VoltageFrequency = voltageFrequency;
                            existingModel.CoolingCapacityW = coolingCapacityW;
                            existingModel.CoolingCapacityBTUPerHour = coolingCapacityBTUPerHour;
                            existingModel.CoolingCapacityKcal = coolingCapacityKcal;
                            existingModel.COPWW = copWW;
                            existingModel.COPBTUPerWH = copBTUPerWH;
                            existingModel.UpdateDate = DateTime.Now;
                            existingModel.UpdatedBy = createdBy;
                            _repository.UpdateModel(existingModel, createdBy);
                        }
                        else
                        {


                            productModel.Add(new ProductModel
                            {
                                Model = model,
                                Displacement = displacement,
                                CoolingType = coolingType,
                                MotorType = motorType,
                                VoltageFrequency = voltageFrequency,
                                CoolingCapacityW = coolingCapacityW,
                                CoolingCapacityBTUPerHour = coolingCapacityBTUPerHour,
                                CoolingCapacityKcal = coolingCapacityKcal,
                                COPWW = copWW,
                                COPBTUPerWH = copBTUPerWH,
                                CreateDate = DateTime.Now,
                                ProductID = proudctId,
                                CreatedBy = createdBy


                            });


                        }
                    }
                        recordsInserted = await _repository.AddMultipleProducts(productModel);
                    
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing file: {ex.Message}");
        }
        return recordsInserted;
    }

    private double? ParseDouble(object input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.ToString()))
            return null;

        if (double.TryParse(input.ToString(), out double value))
            return value;

        return null;
    }

}
