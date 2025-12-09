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

            if (dgTabla.SelectedItem is trabajadores t)
            {
                trabajadorSeleccionadoId = t.id_personal;

                // Mostrar ID
                txtID.Text = trabajadorSeleccionadoId.ToString();

                txtNombre.Text = t.nombre;
                txtApellido.Text = t.apellido;
                txtPuesto.Text = t.puesto;

                // Buscar turno
                try
                {
                    var turnos = turnoBD.ObtenerTurnos();
                    var turno = turnos.FirstOrDefault(x => x.id_turno == t.turno);

                    if (turno != null)
                    {
                        txtHoraInicio.Text = turno.hora_inicio.ToString();
                        txtHoraFin.Text = turno.hora_fin.ToString();
                        txtDia.Text = turno.dia;
                    }
                    else
                    {
                        txtHoraInicio.Text = "";
                        txtHoraFin.Text = "";
                        txtDia.Text = "";
                    }
                }
                catch { }
            }
            else
            {
                trabajadorSeleccionadoId = 0;
                txtID.Text = "";
            }
        }

        // ------------------ INSERTAR ------------------
        private void btnInsertar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual != ModoVista.Trabajadores) return;

            try
            {
                trabajadorBD.AgregarTrabajadorConTurno(
                    txtNombre.Text,
                    txtApellido.Text,
                    txtPuesto.Text,
                    TimeOnly.Parse(txtHoraInicio.Text),
                    TimeOnly.Parse(txtHoraFin.Text),
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

            trabajadorSeleccionadoId = int.Parse(txtID.Text);

            try
            {
                trabajadorBD.ActualizarTrabajadorConTurno(
                    trabajadorSeleccionadoId,
                    txtNombre.Text,
                    txtApellido.Text,
                    txtPuesto.Text,
                    TimeOnly.Parse(txtHoraInicio.Text),
                    TimeOnly.Parse(txtHoraFin.Text),
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
