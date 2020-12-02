using PlantManagerAPI.Extensions;
using PlantManagerAPI.Models;
using PlantManagerAPI.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure
{
    public class PlantSeed
    {
        public static void InitData(PlantManagerContext context)
        {
            InitPlant(context);
            InitDepartment(context);
            InitCostCenter(context);
            InitMachine(context);
        }

        private static void InitPlant(PlantManagerContext context)
        {
            var id = 1;

            context.Plants.AddRange(2.Times(x =>
            {
                var plant = new PlantModel
                {
                    PlantId = id,
                    Name = $"Plant {id}",
                    Address = "Traian 4",
                    City = "Brasov",
                    Email = "asd@asd.com",
                    Phone = "0722222222"
                };
                id++;
                return plant;
            }));

            context.SaveChanges();

            //return context;
        }

        private static void InitDepartment(PlantManagerContext context)
        {
            var id = 1;
            foreach (var item in context.Plants.Select(x => x.PlantId))
            {
                context.Departments.AddRange(3.Times(x =>
                {
                    var department = new Department
                    {
                        DepartmentId = id,
                        Name = $"Department {id}",
                        Description = $"Description of department {id}",
                        PlantId = item
                    };

                    id++;

                    return department;
                }));
            }
            context.SaveChanges();
        }

        private static void InitCostCenter(PlantManagerContext context)
        {
            var id = 1;
            var rnd = new Random();

            foreach (var item in context.Departments.Select(x => x.DepartmentId))
            {
                context.CostCenters.AddRange(3.Times(x =>
                {
                    var costCenter = new CostCenter
                    {
                        CostCenterId = id,
                        DepartmentId = item,
                        Name = $"Cost Center {id}",
                        Description = $"Description of cost center {id}",
                        Cost = rnd.Next(10, 30),
                        AverageCost = rnd.Next(10, 30)
                    };
                    id++;

                    return costCenter;
                }));
            }
            context.SaveChanges();
        }

        private static void InitMachine(PlantManagerContext context)
        {
            var id = 1;
            var rnd = new Random();

            foreach (var item in context.CostCenters.Select(x => x.CostCenterId))
            {
                context.Machines.AddRange(3.Times(x =>
                {
                    var machine = new Machine
                    {
                        MachineId = id,
                        CostCenterId = item,
                        Name = $"Machine {id}",
                        Description = $"Description of machine {id}",
                        RatePerHour = rnd.Next(15,30),
                        SetupTime = rnd.Next(30, 60),
                        ProcessTime = rnd.Next(1, 120),
                        PartsPerCycle = rnd.Next(1, 5)
                    };
                    id++;

                    return machine;
                }));
            }
            context.SaveChanges();
        }
    }
}
