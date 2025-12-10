using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class proveedoresHelper
    {
        private string cadena;

        public proveedoresHelper(string conexion)
        {
            cadena = conexion;
        }

        // ================= OBTENER =================
        public DataTable ObtenerProveedores()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "SELECT * FROM proveedor";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        // ================= INSERTAR =================
        public void InsertarProveedor(string nombre, string telefono)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"INSERT INTO proveedor (nombre, telefono)
                                 VALUES (@nombre, @telefono)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@telefono", telefono);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= EDITAR =================
        public void EditarProveedor(int id, string nombre, string telefono)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"UPDATE proveedor SET
                                 nombre = @nombre,
                                 telefono = @telefono
                                 WHERE id_proveedor = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@telefono", telefono);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= ELIMINAR =================
        public void EliminarProveedor(int id)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = "DELETE FROM proveedor WHERE id_proveedor = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
