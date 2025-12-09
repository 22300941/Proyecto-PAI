using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace BD
{
    public class trabajadoresHelper
    {
        private readonly string connectionString;

        public trabajadoresHelper(string conn)
        {
            connectionString = conn;
        }

        // ------------------------------------------------------
        // INSERTAR TRABAJADOR + TURNO
        // ------------------------------------------------------
        public void AgregarTrabajadorConTurno(
            string nombre,
            string apellido,
            string puesto,
            TimeOnly horaInicio,
            TimeOnly horaFin,
            string dia)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Insertar turno y obtener ID
                string queryTurno =
                    @"INSERT INTO turno (hora_inicio, hora_fin, dia)
                      OUTPUT INSERTED.id_turno
                      VALUES (@inicio, @fin, @dia)";

                int idTurno;

                using (SqlCommand cmd = new SqlCommand(queryTurno, conn))
                {
                    cmd.Parameters.AddWithValue("@inicio", horaInicio.ToTimeSpan());
                    cmd.Parameters.AddWithValue("@fin", horaFin.ToTimeSpan());
                    cmd.Parameters.AddWithValue("@dia", dia);

                    idTurno = (int)cmd.ExecuteScalar();
                }

                // 2. Insertar trabajador con la FK del turno
                string queryTrab =
                    @"INSERT INTO trabajadores (nombre, apellido, puesto, turno) 
                      VALUES (@nombre, @apellido, @puesto, @turno)";

                using (SqlCommand cmd = new SqlCommand(queryTrab, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@puesto", puesto);
                    cmd.Parameters.AddWithValue("@turno", idTurno);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ------------------------------------------------------
        // ACTUALIZAR TRABAJADOR + TURNO
        // ------------------------------------------------------
        public void ActualizarTrabajadorConTurno(
            int idTrabajador,
            string nombre,
            string apellido,
            string puesto,
            TimeOnly horaInicio,
            TimeOnly horaFin,
            string dia)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Obtener turno asociado
                int idTurno;
                string queryGetTurno = "SELECT turno FROM trabajadores WHERE id_personal = @id";

                using (SqlCommand cmd = new SqlCommand(queryGetTurno, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idTrabajador);
                    idTurno = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 1. Actualizar turno
                string queryTurno =
                    @"UPDATE turno
                      SET hora_inicio = @inicio, hora_fin = @fin, dia = @dia
                      WHERE id_turno = @idTurno";

                using (SqlCommand cmd = new SqlCommand(queryTurno, conn))
                {
                    cmd.Parameters.AddWithValue("@inicio", horaInicio.ToTimeSpan());
                    cmd.Parameters.AddWithValue("@fin", horaFin.ToTimeSpan());
                    cmd.Parameters.AddWithValue("@dia", dia);
                    cmd.Parameters.AddWithValue("@idTurno", idTurno);

                    cmd.ExecuteNonQuery();
                }

                // 2. Actualizar trabajador
                string queryTrab =
                    @"UPDATE trabajadores
                      SET nombre = @nombre,
                          apellido = @apellido,
                          puesto = @puesto
                      WHERE id_personal = @id";

                using (SqlCommand cmd = new SqlCommand(queryTrab, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@puesto", puesto);
                    cmd.Parameters.AddWithValue("@id", idTrabajador);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ------------------------------------------------------
        // ELIMINAR TRABAJADOR + TURNO
        // ------------------------------------------------------
        public void EliminarTrabajadorConTurno(int idTrabajador)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                int? idTurno = null;

                // Obtener turno (puede ser nulo)
                string queryGetTurno = "SELECT turno FROM trabajadores WHERE id_personal = @id";
                using (SqlCommand cmd = new SqlCommand(queryGetTurno, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idTrabajador);
                    var result = cmd.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                        idTurno = Convert.ToInt32(result);
                }

                // Eliminar trabajador
                string queryTrab = "DELETE FROM trabajadores WHERE id_personal = @id";
                using (SqlCommand cmd = new SqlCommand(queryTrab, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idTrabajador);
                    cmd.ExecuteNonQuery();
                }

                // Eliminar turno solo si existe
                if (idTurno.HasValue)
                {
                    string queryTurno = "DELETE FROM turno WHERE id_turno = @id";
                    using (SqlCommand cmd = new SqlCommand(queryTurno, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idTurno.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
