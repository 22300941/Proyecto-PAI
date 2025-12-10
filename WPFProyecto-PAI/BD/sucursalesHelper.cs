using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class sucursalesHelper
    {
        private string cadenaConexion;

        public sucursalesHelper(string conexion)
        {
            cadenaConexion = conexion;
        }

        // ================== OBTENER TODAS (DATA TABLE) ==================
        public DataTable ObtenerSucursales()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                string query = "SELECT id_sucursal, nombre, direccion, telefono FROM sucursal";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        // ================== INSERTAR ==================
        public void InsertarSucursal(string nombre, string direccion, int telefono)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                string query = @"INSERT INTO sucursal (nombre, direccion, telefono)
                                 VALUES (@nombre, @direccion, @telefono)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@direccion", direccion);
                cmd.Parameters.AddWithValue("@telefono", telefono);
                cmd.ExecuteNonQuery();
            }
        }

        // ================== EDITAR ==================
        public void EditarSucursal(int id, string nombre, string direccion, int telefono)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                string query = @"UPDATE sucursal
                                 SET nombre = @nombre, direccion = @direccion, telefono = @telefono
                                 WHERE id_sucursal = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@direccion", direccion);
                cmd.Parameters.AddWithValue("@telefono", telefono);
                cmd.ExecuteNonQuery();
            }
        }

        // ================== ELIMINAR ==================
        public void EliminarSucursal(int id)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                string query = "DELETE FROM sucursal WHERE id_sucursal = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
