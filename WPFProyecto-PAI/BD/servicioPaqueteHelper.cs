using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class servicioPaqueteHelper
    {
        private string cadena;

        public servicioPaqueteHelper(string cadenaConexion)
        {
            cadena = cadenaConexion;
        }

        public void AsignarServicioPaquete(int idServicio, int idPaquete)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"INSERT INTO servicio_paquete (id_servicio, id_paquete)
                                 VALUES (@s, @p)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", idServicio);
                cmd.Parameters.AddWithValue("@p", idPaquete);
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ObtenerServiciosDePaquete(int idPaquete)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"
                    SELECT s.id_servicio, s.descripción, s.costo
                    FROM servicio_paquete sp
                    INNER JOIN servicio s ON sp.id_servicio = s.id_servicio
                    WHERE sp.id_paquete = @p";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@p", idPaquete);
                da.Fill(dt);
            }

            return dt;
        }

        // ========== 3. Quitar un servicio de un paquete ==========
        public void QuitarServicioDePaquete(int idServicio, int idPaquete)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"DELETE FROM servicio_paquete
                                 WHERE id_servicio = @s AND id_paquete = @p";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@s", idServicio);
                cmd.Parameters.AddWithValue("@p", idPaquete);
                cmd.ExecuteNonQuery();
            }
        }
    }
}