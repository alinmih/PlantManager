using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Models.Company
{
    public class Plant
    {
        [Key]
        [Required]
        [Display(Name = "plantId")]
        public int PlantId { get; set; }

        [Required]
        [Display(Name = "name")]
        public string Name { get; set; }

        [Display(Name = "address")]
        public string Address { get; set; }

        [Display(Name = "city")]
        public string City { get; set; }

        [Display(Name = "phone")]
        public string Phone { get; set; }

        [Display(Name = "email")]
        public string Email { get; set; }

    }
}
