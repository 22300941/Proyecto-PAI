using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFProyecto_PAI.Tablas
{
    class turno
    {
        public int id_turno { get; set; }
        public TimeOnly hora_inicio { get; set; }
        public TimeOnly hora_fin { get; set; }
        public string dia { get; set; }
    }
}
