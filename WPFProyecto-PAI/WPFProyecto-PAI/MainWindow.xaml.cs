using BD;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFProyecto_PAI.Tablas;

namespace WPFProyecto_PAI
{
    public partial class MainWindow : Window
    {
        private trabajadoresHelper trabajadorBD;
        private turnoHelper turnoBD;
        private string cadenaConexion;

        public MainWindow()
        {
            InitializeComponent();

            // Cadena de conexion a la base de datos
            cadenaConexion = @"Server=DESKTOP-LEHP21J;Database=Paqueteria;Integrated Security=True;TrustServerCertificate=True;";

            // Inicializar helpers
            trabajadorBD = new trabajadoresHelper(cadenaConexion);
            turnoBD = new turnoHelper(cadenaConexion);
        }

        // ------------------------------- TRABAJADORES -------------------------------
        private void CargarTrabajadores()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                {
                    conn.Open();

                    string query = "SELECT id_personal, nombre, apellido, puesto, turno FROM trabajadores";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgTabla.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar trabajadores: " + ex.Message);
            }
        }

        private void tabContenedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.Source is TabControl)) return;

            if (tabContenedor.SelectedItem is TabItem tab)
            {
                if (tab.Header.ToString() == "Trabajadores")
                {
                    CargarTrabajadores();
                }
            }
        }

        private void dgTabla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla.SelectedItem is DataRowView row)
            {
                try
                {
                    txtID.Text = row["id_personal"]?.ToString() ?? "";
                    txtNombre.Text = row["nombre"]?.ToString() ?? "";
                    txtApellido.Text = row["apellido"]?.ToString() ?? "";
                    txtPuesto.Text = row["puesto"]?.ToString() ?? "";

                    if (row["turno"] == DBNull.Value)
                    {
                        txtHoraInicio.Text = "";
                        txtHoraFin.Text = "";
                        txtDia.Text = "";
                        return;
                    }

                    int idTurno = Convert.ToInt32(row["turno"]);
                    var turnos = turnoBD.ObtenerTurnos();
                    var t = turnos.FirstOrDefault(x => x.id_turno == idTurno);

                    if (t != null)
                    {
                        txtHoraInicio.Text = t.hora_inicio.ToString("HH:mm");
                        txtHoraFin.Text = t.hora_fin.ToString("HH:mm");
                        txtDia.Text = t.dia;
                    }
                    else
                    {
                        txtHoraInicio.Text = "";
                        txtHoraFin.Text = "";
                        txtDia.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al seleccionar registro: " + ex.Message);
                }
            }
        }

        // ------------------------------- INSERTAR -------------------------------
        private void btnInsertar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TimeOnly.TryParse(txtHoraInicio.Text, out var hInicio))
                {
                    MessageBox.Show("Hora inicio inválida.");
                    return;
                }

                if (!TimeOnly.TryParse(txtHoraFin.Text, out var hFin))
                {
                    MessageBox.Show("Hora fin inválida.");
                    return;
                }

                trabajadorBD.AgregarTrabajadorConTurno(
                    txtNombre.Text,
                    txtApellido.Text,
                    txtPuesto.Text,
                    hInicio,
                    hFin,
                    txtDia.Text
                );

                MessageBox.Show("Trabajador insertado correctamente.");

                CargarTrabajadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar: " + ex.Message);
            }
        }

        // ------------------------------- EDITAR -------------------------------
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("Selecciona un trabajador primero.");
                return;
            }

            if (!TimeOnly.TryParse(txtHoraInicio.Text, out var hInicio) ||
                !TimeOnly.TryParse(txtHoraFin.Text, out var hFin))
            {
                MessageBox.Show("Horas inválidas.");
                return;
            }

            try
            {
                trabajadorBD.ActualizarTrabajadorConTurno(
                    int.Parse(txtID.Text),
                    txtNombre.Text,
                    txtApellido.Text,
                    txtPuesto.Text,
                    hInicio,
                    hFin,
                    txtDia.Text
                );

                MessageBox.Show("Trabajador actualizado.");

                CargarTrabajadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }

        // ------------------------------- ELIMINAR -------------------------------
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("Selecciona un trabajador primero.");
                return;
            }

            if (MessageBox.Show("¿Eliminar este trabajador?", "Confirmación",
                                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                trabajadorBD.EliminarTrabajadorConTurno(int.Parse(txtID.Text));

                MessageBox.Show("Trabajador eliminado.");

                CargarTrabajadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        // ------------------------------- LIMPIAR -------------------------------
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtID.Text = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtPuesto.Text = "";
            txtHoraInicio.Text = "";
            txtHoraFin.Text = "";
            txtDia.Text = "";
        }
    }
}
