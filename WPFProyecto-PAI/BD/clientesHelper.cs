using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace BD
{
    public class clientesHelper
    {
        private string cadena;

        public clientesHelper(string conexion)
        {
            cadena = conexion;
        }

        // ================= OBTENER =================
        public DataTable ObtenerClientes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();
                string query = "SELECT * FROM cliente";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }

        // ================= INSERTAR =================
        public void InsertarCliente(
            string telefono,
            string direccion,
            string firma,
            string nombre,
            string apellido,
            int idSucursal)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"INSERT INTO cliente
                                 (telefono, direccion, firma, nombre, apellido, id_sucursal)
                                 VALUES (@telefono, @direccion, @firma, @nombre, @apellido, @idsucursal)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@telefono", telefono);
                cmd.Parameters.AddWithValue("@direccion", direccion);
                cmd.Parameters.AddWithValue("@firma", firma);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);
                cmd.Parameters.AddWithValue("@idsucursal", idSucursal);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= EDITAR =================
        public void EditarCliente(
            int id,
            string telefono,
            string direccion,
            string firma,
            string nombre,
            string apellido,
            int idSucursal)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = @"UPDATE cliente SET
                                 telefono = @telefono,
                                 direccion = @direccion,
                                 firma = @firma,
                                 nombre = @nombre,
                                 apellido = @apellido,
                                 id_sucursal = @idsucursal
                                 WHERE id_cliente = @id";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@telefono", telefono);
                cmd.Parameters.AddWithValue("@direccion", direccion);
                cmd.Parameters.AddWithValue("@firma", firma);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellido", apellido);
                cmd.Parameters.AddWithValue("@idsucursal", idSucursal);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= ELIMINAR =================
        public void EliminarCliente(int id)
        {
            using (SqlConnection conn = new SqlConnection(cadena))
            {
                conn.Open();

                string query = "DELETE FROM cliente WHERE id_cliente = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
