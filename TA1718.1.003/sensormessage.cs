using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA1718._1._003
{
    public class sensormessage
    {
        public decimal temperature { get; set; }
        public decimal humidity { get; set; }
        public int lightintensity { get; set; }
        public int soundlevel { get; set; }
        public string recomtemp { get; set; }
        public string recomhumid { get; set; }
        public string recomlight { get; set; }
        public string recomsound { get; set; }
        public string sensor_id { get; set; }
    }
}
