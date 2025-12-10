using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class servicioHelper
    {
        private string cadena;

        public servicioHelper(string cadenaConexion)
        {
            cadena = cadenaConexion;
        }

        public DataTable ObtenerServicios()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "SELECT id_servicio, descripción, costo, id_paquete FROM servicio";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        public DataTable ObtenerPaquetes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "SELECT id_paquete FROM paquete";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        public void InsertarServicio(string descripcion, decimal costo, int idPaquete)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "INSERT INTO servicio (descripción, costo, id_paquete) VALUES (@d, @c, @p)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@d", descripcion);
                cmd.Parameters.AddWithValue("@c", costo);
                cmd.Parameters.AddWithValue("@p", idPaquete);

                cmd.ExecuteNonQuery();
            }
        }

        public void EditarServicio(int id, string descripcion, decimal costo, int idPaquete)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "UPDATE servicio SET descripción=@d, costo=@c, id_paquete=@p WHERE id_servicio=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@d", descripcion);
                cmd.Parameters.AddWithValue("@c", costo);
                cmd.Parameters.AddWithValue("@p", idPaquete);

                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarServicio(int id)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "DELETE FROM servicio WHERE id_servicio=@id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
