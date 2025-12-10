using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class paqueteHelper
    {
        private string cadena;

        public paqueteHelper(string conexion)
        {
            cadena = conexion;
        }

        // ================= OBTENER =================
        public DataTable ObtenerPaquetes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"
        SELECT 
            id_paquete,
            FORMAT(fecha_entrada, 'dd/MM/yyyy') AS fecha_entrada,
            FORMAT(fecha_salida, 'dd/MM/yyyy') AS fecha_salida,
            tipo_paquete,
            codigo_barras,
            id_sucursal,
            id_cliente,
            id_proveedor
        FROM paquete";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        // ================= INSERTAR =================
        public void InsertarPaquete(
            DateOnly entrada,
            DateOnly salida,
            string tipo,
            string codigo,
            int sucursal,
            int cliente,
            int proveedor)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"INSERT INTO paquete
                                 (fecha_entrada, fecha_salida, tipo_paquete, codigo_barras, id_sucursal, id_cliente, id_proveedor)
                                 VALUES (@fentrada, @fsalida, @tipo, @codigo, @sucursal, @cliente, @proveedor)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@fentrada", SqlDbType.Date).Value = entrada;
                cmd.Parameters.Add("@fsalida", SqlDbType.Date).Value = salida;
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cmd.Parameters.AddWithValue("@sucursal", sucursal);
                cmd.Parameters.AddWithValue("@cliente", cliente);
                cmd.Parameters.AddWithValue("@proveedor", proveedor);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= EDITAR =================
        public void EditarPaquete(
            int id,
            DateOnly entrada,
            DateOnly salida,
            string tipo,
            string codigo,
            int sucursal,
            int cliente,
            int proveedor)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"UPDATE paquete SET
                                 fecha_entrada=@fentrada,
                                 fecha_salida=@fsalida,
                                 tipo_paquete=@tipo,
                                 codigo_barras=@codigo,
                                 id_sucursal=@sucursal,
                                 id_cliente=@cliente,
                                 id_proveedor=@proveedor
                                 WHERE id_paquete=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@fentrada", entrada.ToDateTime(new TimeOnly(0, 0)));
                cmd.Parameters.AddWithValue("@fsalida", salida.ToDateTime(new TimeOnly(0, 0)));
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@codigo", codigo);
                cmd.Parameters.AddWithValue("@sucursal", sucursal);
                cmd.Parameters.AddWithValue("@cliente", cliente);
                cmd.Parameters.AddWithValue("@proveedor", proveedor);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= ELIMINAR =================
        public void EliminarPaquete(int id)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = "DELETE FROM paquete WHERE id_paquete=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
