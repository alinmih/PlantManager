using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Models.Company
{
    public class CostCenter
    {
        [Key]
        [Required]
        [Display(Name = "costCenterId")]
        public int CostCenterId { get; set; }

        [Required]
        [Display(Name = "departmentId")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "name")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "cost")]
        public decimal Cost { get; set; }

        [Display(Name = "averageCost")]
        public decimal AverageCost { get; set; }

        [Display(Name = "modifiedDate")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;





    }
}
