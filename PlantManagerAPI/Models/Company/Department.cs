using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Models.Company
{
    public class Department
    {
        [Key]
        [Required]
        [Display(Name = "departmentId")]
        public int DepartmentId { get; set; }
        
        [Required]
        [Display(Name = "plantId")]
        public int PlantId { get; set; }

        [Required]
        [Display(Name = "name")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }



    }
}
