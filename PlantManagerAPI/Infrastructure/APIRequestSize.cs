using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure
{
    public class APIRequestSize
    {
        [FromQuery(Name = "limit")]
        public int Limit { get; set; } = 15;

        [FromQuery(Name = "offset")]
        public int Offset { get; set; }
    }
}
