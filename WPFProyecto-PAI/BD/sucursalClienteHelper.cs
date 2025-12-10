using Microsoft.Data.SqlClient;
using System.Data;

namespace BD
{
    public class sucursalClienteHelper
    {
        private string cadena;

        public sucursalClienteHelper(string conexion)
        {
            cadena = conexion;
        }

    
        public void AsignarClienteSucursal(int idSucursal, int idCliente)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"INSERT INTO sucursal_cliente (id_sucursal, id_cliente)
                                 VALUES (@s, @c)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", idSucursal);
                cmd.Parameters.AddWithValue("@c", idCliente);
                cmd.ExecuteNonQuery();
            }
        }

        // ========== 2. Obtener sucursales de un cliente ==========
        public DataTable ObtenerSucursalesDeCliente(int idCliente)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"
                    SELECT sc.id_sucursal, s.nombre, s.direccion, s.telefono
                    FROM sucursal_cliente sc
                    INNER JOIN sucursal s ON sc.id_sucursal = s.id_sucursal
                    WHERE sc.id_cliente = @c";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@c", idCliente);
                da.Fill(dt);
            }

            return dt;
        }

        // ========== 3. Quitar cliente de una sucursal ==========
        public void QuitarClienteDeSucursal(int idSucursal, int idCliente)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"DELETE FROM sucursal_cliente
                                 WHERE id_sucursal = @s AND id_cliente = @c";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", idSucursal);
                cmd.Parameters.AddWithValue("@c", idCliente);
                cmd.ExecuteNonQuery();
            }
        }
    }
}