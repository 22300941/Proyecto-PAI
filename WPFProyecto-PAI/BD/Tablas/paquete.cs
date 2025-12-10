using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFProyecto_PAI.Tablas
{
    public class paquete
    {
        public int id_paquete { get; set; }
        public DateOnly fecha_entrada { get; set; }
        public DateOnly fecha_salida { get; set; }
        public string tipo_paquete { get; set; }
        public string codigo_barras { get; set; }
        public int id_sucursal { get; set; }
        public int id_cliente { get; set; }
        public int id_proveedor { get; set; }



    }
}
