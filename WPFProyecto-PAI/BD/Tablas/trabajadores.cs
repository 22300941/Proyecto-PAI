using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFProyecto_PAI.Tablas
{
    class trabajadores
    {
        public int id_personal { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string puesto { get; set; }
        public string turno { get; set; } = string.Empty;
    }
}
