using Microsoft.Data.SqlClient;
using WPFProyecto_PAI.Tablas;

namespace BD
{
    public class turnoHelper
    {
        private string _conexion;

        public turnoHelper(string conexion)
        {
            _conexion = conexion;
        }

        public List<turno> ObtenerTurnos()
        {
            List<turno> lista = new List<turno>();

            using (SqlConnection sqlCon = new SqlConnection(_conexion))
            {
                sqlCon.Open();

                SqlCommand sql = new SqlCommand(
                    "SELECT id_turno, hora_inicio, hora_fin, dia FROM dbo.turno",
                    sqlCon
                );

                SqlDataReader reader = sql.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new turno
                    {
                        id_turno = reader.GetInt32(0),
                        hora_inicio = TimeOnly.Parse(reader.GetValue(1).ToString()),
                        hora_fin = TimeOnly.Parse(reader.GetValue(2).ToString()),
                        dia = reader.GetString(3)
                    });
                }
            }

            return lista;
        }
    }
}
