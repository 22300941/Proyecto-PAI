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
        private enum ModoVista
        {
            Ninguno,
            Trabajadores
        }

        private ModoVista modoActual = ModoVista.Ninguno;

        private trabajadoresHelper trabajadorBD;
        private turnoHelper turnoBD;
        private int trabajadorSeleccionadoId = 0;
        private string cadenaConexion;

        public MainWindow()
        {
            InitializeComponent();

            cadenaConexion = @"Server=DESKTOP-LEHP21J;Database=Paqueteria;Integrated Security=True;TrustServerCertificate=True;";
            trabajadorBD = new trabajadoresHelper(cadenaConexion);
            turnoBD = new turnoHelper(cadenaConexion);

            OcultarTodosLosCampos();
        }

        private void OcultarTodosLosCampos()
        {
            lblID.Visibility = Visibility.Collapsed;
            txtID.Visibility = Visibility.Collapsed;

            lblNombre.Visibility = Visibility.Collapsed;
            txtNombre.Visibility = Visibility.Collapsed;

            lblApellido.Visibility = Visibility.Collapsed;
            txtApellido.Visibility = Visibility.Collapsed;

            lblPuesto.Visibility = Visibility.Collapsed;
            txtPuesto.Visibility = Visibility.Collapsed;

            lblHoraInicio.Visibility = Visibility.Collapsed;
            txtHoraInicio.Visibility = Visibility.Collapsed;

            lblHoraFin.Visibility = Visibility.Collapsed;
            txtHoraFin.Visibility = Visibility.Collapsed;

            lblDia.Visibility = Visibility.Collapsed;
            txtDia.Visibility = Visibility.Collapsed;

            btnInsertar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Collapsed;
            btnEliminar.Visibility = Visibility.Collapsed;
        }

        private void MostrarCamposTrabajador()
        {
            OcultarTodosLosCampos();

            lblID.Visibility = Visibility.Visible;
            txtID.Visibility = Visibility.Visible;

            lblNombre.Visibility = Visibility.Visible;
            txtNombre.Visibility = Visibility.Visible;

            lblApellido.Visibility = Visibility.Visible;
            txtApellido.Visibility = Visibility.Visible;

            lblPuesto.Visibility = Visibility.Visible;
            txtPuesto.Visibility = Visibility.Visible;

            lblHoraInicio.Visibility = Visibility.Visible;
            txtHoraInicio.Visibility = Visibility.Visible;

            lblHoraFin.Visibility = Visibility.Visible;
            txtHoraFin.Visibility = Visibility.Visible;

            lblDia.Visibility = Visibility.Visible;
            txtDia.Visibility = Visibility.Visible;

            btnInsertar.Visibility = Visibility.Visible;
            btnEditar.Visibility = Visibility.Visible;
            btnEliminar.Visibility = Visibility.Visible;
        }

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
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnTrabajador_Click(object sender, RoutedEventArgs e)
        {
            modoActual = ModoVista.Trabajadores;
            MostrarCamposTrabajador();
            CargarTrabajadores();
        }

        // ------------------ SELECCIÓN DEL GRID ------------------
        private void dgTabla_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modoActual != ModoVista.Trabajadores) return;

            if (dgTabla.SelectedItem is DataRowView row)
            {
                try
                {
                    // Validar columnas antes de leer
                    string[] columnas = { "id_personal", "nombre", "apellido", "puesto", "turno" };
                    foreach (var col in columnas)
                    {
                        if (!row.DataView.Table.Columns.Contains(col))
                        {
                            MessageBox.Show($"ERROR: La columna '{col}' no existe en el DataGrid.");
                            return;
                        }
                    }

                    // Asignar valores seguros
                    txtID.Text = row["id_personal"]?.ToString() ?? "";
                    txtNombre.Text = row["nombre"]?.ToString() ?? "";
                    txtApellido.Text = row["apellido"]?.ToString() ?? "";
                    txtPuesto.Text = row["puesto"]?.ToString() ?? "";

                    // ==========================
                    // Obtener el id_turno
                    // ==========================
                    if (row["turno"] == DBNull.Value)
                    {
                        txtHoraInicio.Text = "";
                        txtHoraFin.Text = "";
                        txtDia.Text = "";
                        return;
                    }

                    int idTurno = Convert.ToInt32(row["turno"]);

                    // Obtener turno desde BD
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
                    MessageBox.Show("Error al cargar selección: " + ex.Message);
                }
            }
        }

        // ------------------ INSERTAR ------------------
        private void btnInsertar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual != ModoVista.Trabajadores) return;

            try
            {
                if (string.IsNullOrWhiteSpace(txtHoraInicio.Text) ||
                    string.IsNullOrWhiteSpace(txtHoraFin.Text))
                {
                    MessageBox.Show("Debes escribir horas válidas (formato HH:mm).");
                    return;
                }

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

        // ------------------ EDITAR ------------------
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual != ModoVista.Trabajadores) return;

            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("Selecciona un trabajador primero.");
                return;
            }

            if (!TimeOnly.TryParse(txtHoraInicio.Text, out var hInicio) ||
                !TimeOnly.TryParse(txtHoraFin.Text, out var hFin))
            {
                MessageBox.Show("Horas inválidas (usa HH:mm).");
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

        // ------------------ ELIMINAR ------------------
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual != ModoVista.Trabajadores) return;

            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("Selecciona un trabajador primero.");
                return;
            }

            trabajadorSeleccionadoId = int.Parse(txtID.Text);

            var confirmar = MessageBox.Show("¿Eliminar este trabajador?", "Confirmación", MessageBoxButton.YesNo);
            if (confirmar != MessageBoxResult.Yes) return;

            try
            {
                trabajadorBD.EliminarTrabajadorConTurno(trabajadorSeleccionadoId);

                MessageBox.Show("Trabajador eliminado.");

                CargarTrabajadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }
    }
}
