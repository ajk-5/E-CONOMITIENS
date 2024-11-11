using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_conomitiens.model
{
    public class Room
    {
        public string RoomId { get; set; }
        public string Batiment { get; set; }
        public List<Electricity> Electricities { get; set; } = new List<Electricity>();
        public List<Heat> Heats { get; set; } = new List<Heat>();
        public List<Light> Lights { get; set; } = new List<Light>();
        public List<Humidity> Humidities { get; set; } = new List<Humidity>();
    }
}
