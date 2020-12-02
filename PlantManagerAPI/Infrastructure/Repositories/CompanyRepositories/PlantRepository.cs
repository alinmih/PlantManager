using PlantManagerAPI.Infrastructure.Db;
using PlantManagerAPI.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure.Repositories.CompanyRepositories
{
    public class PlantRepository:GenericRepository<PlantModel>
    {
        private readonly IDataAccess _dataAccess;

        public PlantRepository():base()
        {
        }

        public PlantRepository(IDataAccess dataAccess):base(dataAccess)
        {
            InsertProcedure = "Company.PlantInsert";
            ReadProcedure = "Company.PlantSelect";
            UpdateProcedure = "Company.PlantUpdate";
            DeleteProcedure = "Company.PlantDelete";
            _dataAccess = dataAccess;
        }

        public override async Task<IEnumerable<PlantModel>> GetAllAsync()
        {
            PlantModel plant = new PlantModel();
            return await _dataAccess.LoadData<PlantModel, dynamic>(ReadProcedure, plant);
        }

        public override Task<IEnumerable<PlantModel>> GetMultipleWithRelatedDataAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<PlantModel> GetOneWithRelatedDataAsync(PlantModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
