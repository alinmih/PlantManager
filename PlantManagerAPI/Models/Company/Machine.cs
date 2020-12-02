using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Models.Company
{
    public class Machine
    {
        [Key]
        [Required]
        [Display(Name = "machineId")]
        public int MachineId { get; set; }

        [Required]
        [Display(Name = "costCenterId")]
        public int CostCenterId { get; set; }

        [Required]
        [Display(Name = "name")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "ratePerHour")]
        public decimal RatePerHour { get; set; }

        [Display(Name = "SetupTime")]
        public double SetupTime { get; set; }

        [Display(Name = "processTime")]
        public double ProcessTime { get; set; }

        [Display(Name = "partsPerCycle")]
        public int PartsPerCycle { get; set; }

        [Display(Name = "alarmOnOff")]
        public bool AlarmOnOff { get; set; } = false;

        [Display(Name = "alarmDate")]
        public DateTime AlarmDate { get; set; } = DateTime.Now;

        [Display(Name = "lastActivityDate")]
        public DateTime LastActivityDate { get; set; } = DateTime.Now;

        [Display(Name = "modifiedDate")]
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        



    }
}
