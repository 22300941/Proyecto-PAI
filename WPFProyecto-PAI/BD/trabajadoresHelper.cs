using Microsoft.Data.SqlClient;
using WPFProyecto_PAI.Tablas;

namespace BD
{
    public class trabajadoresHelper
    {
        private string _conexion;

        public trabajadoresHelper(string conexion)
        {
            _conexion = conexion;
        }

        // -----------------------------------------------------------
        //  AGREGAR: Trabajador + Turno
        // -----------------------------------------------------------
        public void AgregarTrabajadorConTurno(
            string nombre,
            string apellido,
            string puesto,
            TimeOnly hora_inicio,
            TimeOnly hora_fin,
            string dia)
        {
            SqlConnection sqlCon;

            using (sqlCon = new SqlConnection(_conexion))
            {
                sqlCon.Open();

                // ===== 1. Insertar turno =====
                SqlCommand sqlTurno = new SqlCommand(
                    "INSERT INTO dbo.turno (hora_inicio, hora_fin, dia) " +
                    "OUTPUT INSERTED.id_turno " +
                    "VALUES (@hora_inicio, @hora_fin, @dia)",
                    sqlCon
                );

                sqlTurno.Parameters.AddWithValue("@hora_inicio", hora_inicio);
                sqlTurno.Parameters.AddWithValue("@hora_fin", hora_fin);
                sqlTurno.Parameters.AddWithValue("@dia", dia);

                // Obtener id_turno generado
                int id_turno = (int)sqlTurno.ExecuteScalar();

                // ===== 2. Insertar trabajador con FK =====
                SqlCommand sqlTrabajador = new SqlCommand(
                    "INSERT INTO dbo.trabajadores (nombre, apellido, puesto, turno) " +
                    "VALUES (@nombre, @apellido, @puesto, @turno)",
                    sqlCon
                );

                sqlTrabajador.Parameters.AddWithValue("@nombre", nombre);
                sqlTrabajador.Parameters.AddWithValue("@apellido", apellido);
                sqlTrabajador.Parameters.AddWithValue("@puesto", puesto);
                sqlTrabajador.Parameters.AddWithValue("@turno", id_turno);

                sqlTrabajador.ExecuteNonQuery();
            }
        }

        // -----------------------------------------------------------
        //  ELIMINAR: Trabajador + su Turno
        // -----------------------------------------------------------
        public void EliminarTrabajadorConTurno(int id_personal)
        {
            SqlConnection sqlCon;

            using (sqlCon = new SqlConnection(_conexion))
            {
                sqlCon.Open();

                // 1. Obtener el id_turno vinculado
                SqlCommand sqlGetTurno = new SqlCommand(
                    "SELECT turno FROM dbo.trabajadores WHERE id_personal = @id",
                    sqlCon
                );
                sqlGetTurno.Parameters.AddWithValue("@id", id_personal);

                object turnoObj = sqlGetTurno.ExecuteScalar();
                if (turnoObj == null) return;

                int id_turno = (int)turnoObj;

                // 2. Eliminar trabajador
                SqlCommand sqlDeleteTrab = new SqlCommand(
                    "DELETE FROM dbo.trabajadores WHERE id_personal = @id",
                    sqlCon
                );
                sqlDeleteTrab.Parameters.AddWithValue("@id", id_personal);
                sqlDeleteTrab.ExecuteNonQuery();

                // 3. Eliminar turno (extensión)
                SqlCommand sqlDeleteTurno = new SqlCommand(
                    "DELETE FROM dbo.turno WHERE id_turno = @id_turno",
                    sqlCon
                );
                sqlDeleteTurno.Parameters.AddWithValue("@id_turno", id_turno);
                sqlDeleteTurno.ExecuteNonQuery();
            }
        }

        // -----------------------------------------------------------
        //  ACTUALIZAR: Trabajador + Turno
        // -----------------------------------------------------------
        public void ActualizarTrabajadorConTurno(
            int id_personal,
            string nombre,
            string apellido,
            string puesto,
            TimeOnly hora_inicio,
            TimeOnly hora_fin,
            string dia)
        {
            SqlConnection sqlCon;

            using (sqlCon = new SqlConnection(_conexion))
            {
                sqlCon.Open();

                // 1. Obtener el turno actual
                SqlCommand sqlGetTurno = new SqlCommand(
                    "SELECT turno FROM dbo.trabajadores WHERE id_personal = @id",
                    sqlCon
                );
                sqlGetTurno.Parameters.AddWithValue("@id", id_personal);

                int id_turno = (int)sqlGetTurno.ExecuteScalar();

                // 2. Actualizar trabajador
                SqlCommand sqlUpdateTrab = new SqlCommand(
                    "UPDATE dbo.trabajadores SET nombre=@nombre, apellido=@apellido, puesto=@puesto " +
                    "WHERE id_personal=@id",
                    sqlCon
                );

                sqlUpdateTrab.Parameters.AddWithValue("@id", id_personal);
                sqlUpdateTrab.Parameters.AddWithValue("@nombre", nombre);
                sqlUpdateTrab.Parameters.AddWithValue("@apellido", apellido);
                sqlUpdateTrab.Parameters.AddWithValue("@puesto", puesto);
                sqlUpdateTrab.ExecuteNonQuery();

                // 3. Actualizar turno
                SqlCommand sqlUpdateTurno = new SqlCommand(
                    "UPDATE dbo.turno SET hora_inicio=@ini, hora_fin=@fin, dia=@dia " +
                    "WHERE id_turno=@id_turno",
                    sqlCon
                );

                sqlUpdateTurno.Parameters.AddWithValue("@id_turno", id_turno);
                sqlUpdateTurno.Parameters.AddWithValue("@ini", hora_inicio);
                sqlUpdateTurno.Parameters.AddWithValue("@fin", hora_fin);
                sqlUpdateTurno.Parameters.AddWithValue("@dia", dia);

                sqlUpdateTurno.ExecuteNonQuery();
            }
        }

        // -----------------------------------------------------------
        //  LEER: Obtener todos
        // -----------------------------------------------------------
        public List<trabajadores> ObtenerTrabajadores()
        {
            List<trabajadores> lista = new List<trabajadores>();
            SqlConnection sqlCon;

            using (sqlCon = new SqlConnection(_conexion))
            {
                sqlCon.Open();

                SqlCommand sql = new SqlCommand(
                    "SELECT id_personal, nombre, apellido, puesto, turno FROM dbo.trabajadores",
                    sqlCon
                );

                SqlDataReader reader = sql.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new trabajadores
                    {
                        id_personal = reader.GetInt32(0),
                        nombre = reader.GetString(1),
                        apellido = reader.GetString(2),
                        puesto = reader.GetString(3),
                        turno = reader.GetInt32(4)
                    });
                }
            }

            return lista;
        }
    }
}
