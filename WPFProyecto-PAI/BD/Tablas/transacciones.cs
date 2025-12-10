using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFProyecto_PAI.Tablas
{
    public class transacciones
    {
        public int id_transaccion { get; set; }
        public DateOnly fecha_pago { get; set; }
        public int total { get; set; }
        public string metodo_pago { get; set; }
        public int id_paquete { get; set; }
    }
}
