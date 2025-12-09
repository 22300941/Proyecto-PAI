using System;
using System.Windows;
using WPFProyecto_PAI.Tablas;
using BD;

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

        public MainWindow()
        {
            InitializeComponent();

            string cadenaConexion = "Falta la conexion";
            trabajadorBD = new trabajadoresHelper(cadenaConexion);
            turnoBD = new turnoHelper(cadenaConexion);

            OcultarTodosLosCampos();
        }

        // ---------------------------------------------------
        // OCULTAR TODO
        // ---------------------------------------------------
        private void OcultarTodosLosCampos()
        {
            txtNombre.Visibility = Visibility.Collapsed;
            txtApellido.Visibility = Visibility.Collapsed;
            txtPuesto.Visibility = Visibility.Collapsed;
            txtHoraInicio.Visibility = Visibility.Collapsed;
            txtHoraFin.Visibility = Visibility.Collapsed;
            txtDia.Visibility = Visibility.Collapsed;

            lblNombre.Visibility = Visibility.Collapsed;
            lblApellido.Visibility = Visibility.Collapsed;
            lblPuesto.Visibility = Visibility.Collapsed;
            lblHoraInicio.Visibility = Visibility.Collapsed;
            lblHoraFin.Visibility = Visibility.Collapsed;
            lblDia.Visibility = Visibility.Collapsed;

            btnInsertar.Visibility = Visibility.Collapsed;
            btnEditar.Visibility = Visibility.Collapsed;
            btnEliminar.Visibility = Visibility.Collapsed;
        }

        // ---------------------------------------------------
        // MOSTRAR CAMPOS TRABAJADOR + TURNO
        // ---------------------------------------------------
        private void MostrarCamposTrabajador()
        {
            OcultarTodosLosCampos();

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

        // ---------------------------------------------------
        // CARGAR GRID
        // ---------------------------------------------------
        private void CargarVistaTrabajadores()
        {
            dgTabla.ItemsSource = trabajadorBD.ObtenerTrabajadores();
        }

        // ---------------------------------------------------
        // BOTÓN TRABAJADORES
        // ---------------------------------------------------
        private void btnTrabajador_Click(object sender, RoutedEventArgs e)
        {
            modoActual = ModoVista.Trabajadores;

            MostrarCamposTrabajador();
            CargarVistaTrabajadores();
        }

        // ---------------------------------------------------
        // INSERTAR
        // ---------------------------------------------------
        private void btnInsertar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual == ModoVista.Trabajadores)
            {
                try
                {
                    string nombre = txtNombre.Text;
                    string apellido = txtApellido.Text;
                    string puesto = txtPuesto.Text;

                    TimeOnly inicio = TimeOnly.Parse(txtHoraInicio.Text);
                    TimeOnly fin = TimeOnly.Parse(txtHoraFin.Text);
                    string dia = txtDia.Text;

                    // SE USA EL MÉTODO REAL DEL HELPER
                    trabajadorBD.AgregarTrabajadorConTurno(
                        nombre,
                        apellido,
                        puesto,
                        inicio,
                        fin,
                        dia
                    );

                    MessageBox.Show("Trabajador agregado correctamente.");
                    CargarVistaTrabajadores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al insertar: " + ex.Message);
                }
            }
        }

        // ---------------------------------------------------
        // EDITAR
        // ---------------------------------------------------
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual == ModoVista.Trabajadores)
            {
                try
                {
                    if (dgTabla.SelectedItem is trabajadores t)
                    {
                        string nombre = txtNombre.Text;
                        string apellido = txtApellido.Text;
                        string puesto = txtPuesto.Text;

                        TimeOnly inicio = TimeOnly.Parse(txtHoraInicio.Text);
                        TimeOnly fin = TimeOnly.Parse(txtHoraFin.Text);
                        string dia = txtDia.Text;

                        // SE USA EL MÉTODO REAL DEL HELPER
                        trabajadorBD.ActualizarTrabajadorConTurno(
                            t.id_personal,
                            nombre,
                            apellido,
                            puesto,
                            inicio,
                            fin,
                            dia
                        );

                        MessageBox.Show("Trabajador actualizado.");
                        CargarVistaTrabajadores();
                    }
                    else
                    {
                        MessageBox.Show("Selecciona un trabajador.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al editar: " + ex.Message);
                }
            }
        }

        // ---------------------------------------------------
        // ELIMINAR
        // ---------------------------------------------------
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (modoActual == ModoVista.Trabajadores)
            {
                try
                {
                    if (dgTabla.SelectedItem is trabajadores t)
                    {
                        // SE USA EL MÉTODO REAL DEL HELPER
                        trabajadorBD.EliminarTrabajadorConTurno(t.id_personal);

                        MessageBox.Show("Trabajador eliminado.");
                        CargarVistaTrabajadores();
                    }
                    else
                    {
                        MessageBox.Show("Selecciona un trabajador.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }
    }
}
