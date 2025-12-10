using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class transaccionesHelper
    {
        private string cadena;

        public transaccionesHelper(string cadenaConexion)
        {
            cadena = cadenaConexion;
        }

        // ===================== OBTENER =====================
        public DataTable ObtenerTransacciones()
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = @"
            SELECT 
                id_transaccion,
                FORMAT(fecha_pago, 'dd/MM/yyyy') AS fecha_pago,
                total,
                metodo_pago,
                id_paquete
            FROM transacciones";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }


        // ===================== INSERTAR =====================
        public void InsertarTransaccion(DateOnly fecha, int total, string metodo, int idPaquete)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query =
                    "INSERT INTO transacciones(fecha_pago, total, metodo_pago, id_paquete) VALUES(@fecha, @total, @metodo, @idPaquete)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@metodo", metodo);
                cmd.Parameters.AddWithValue("@idPaquete", idPaquete);

                cmd.ExecuteNonQuery();
            }
        }

        // ===================== EDITAR =====================
        public void EditarTransaccion(int id, DateOnly fecha, int total, string metodo, int idPaquete)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query =
                    "UPDATE transacciones SET fecha_pago=@fecha, total=@total, metodo_pago=@metodo, id_paquete=@idPaquete WHERE id_transaccion=@id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@metodo", metodo);
                cmd.Parameters.AddWithValue("@idPaquete", idPaquete);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }

        // ===================== ELIMINAR =====================
        public void EliminarTransaccion(int id)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM transacciones WHERE id_transaccion=@id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
