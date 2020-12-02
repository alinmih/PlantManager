using Microsoft.EntityFrameworkCore;
using PlantManagerAPI.Models;
using PlantManagerAPI.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure
{
    public class PlantManagerContext : DbContext
    {
        public PlantManagerContext(DbContextOptions<PlantManagerContext> options) : base(options)
        {
        }

        public DbSet<PlantModel> Plants { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<Machine> Machines { get; set; }
    }
}
