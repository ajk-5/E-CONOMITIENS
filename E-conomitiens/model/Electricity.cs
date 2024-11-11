using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_conomitiens.model
{
    public class Electricity
    {
        public int Id { get; set; }
        public DateTime DateTimeCollected { get; set; }
        public int Consumption { get; set; }
    }
}
