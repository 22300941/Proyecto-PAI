using System;

namespace WPFProyecto_PAI.Tablas
{
    public class cliente
    {
        public int id_cliente { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string firma { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int id_sucursal { get; set; }   // FK hacia sucursal
    }
}
