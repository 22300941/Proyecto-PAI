using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using WPFProyecto_PAI.Tablas;

namespace BD
{
    public class turnoHelper
    {
        private readonly string connectionString;

        public turnoHelper(string conn)
        {
            connectionString = conn;
        }

        public List<turno> ObtenerTurnos()
        {
            List<turno> lista = new();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id_turno, hora_inicio, hora_fin, dia FROM turno";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new turno
                        {
                            id_turno = dr.GetInt32(0),
                            hora_inicio = TimeOnly.FromTimeSpan(dr.GetTimeSpan(1)),
                            hora_fin = TimeOnly.FromTimeSpan(dr.GetTimeSpan(2)),
                            dia = dr.GetString(3).Trim()
                        });
                    }
                }
            }

            return lista;
        }
    }
}
