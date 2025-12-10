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
        private sucursalesHelper sucursalBD;
        private servicioHelper servicioBD;



        private string cadenaConexion;

        public MainWindow()
        {
            InitializeComponent();

            // Cadena de conexion a la base de datos
            cadenaConexion = @"Server=FABIANCETI;Database=Paqueteria;Integrated Security=True;TrustServerCertificate=True;";

            // Inicializar helpers
            trabajadorBD = new trabajadoresHelper(cadenaConexion);
            turnoBD = new turnoHelper(cadenaConexion);

            sucursalBD = new sucursalesHelper(cadenaConexion);
            servicioBD = new servicioHelper(cadenaConexion);




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
                // Si selecciona Trabajadores
                if (tab.Header.ToString() == "Trabajadores")
                {
                    CargarTrabajadores();
                }

                // Si selecciona Sucursales
                if (tab.Header.ToString() == "Sucursales")
                {
                    CargarSucursales();
                }
                if (tab.Header.ToString() == "Servicios")
                {
                    CargarServicios();
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
        // -------------------------------  FIN TRABAJADORES -------------------------------


        // ------------------------------- SUCURSALES -------------------------------



        private void CargarSucursales()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                {
                    conn.Open();

                    string query = "SELECT id_sucursal, nombre, direccion, telefono FROM sucursal";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgTabla_Sucursales.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar sucursales: " + ex.Message);
            }
        }



        private void dgTabla_Sucursales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla_Sucursales.SelectedItem is DataRowView row)
            {
                txtID_sucursales.Text = row["id_sucursal"]?.ToString() ?? "";
                txtNombre_sucursales.Text = row["nombre"]?.ToString() ?? "";
                txtDireccion_sucursales.Text = row["direccion"]?.ToString() ?? "";
                txtTelefono_sucursales.Text = row["telefono"]?.ToString() ?? "";
            }
        }


        private void btnInsertar_sucursales_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtTelefono_sucursales.Text, out int tel))
            {
                MessageBox.Show("Teléfono inválido.");
                return;
            }

            try
            {
                sucursalBD.InsertarSucursal(
                    txtNombre_sucursales.Text,
                    txtDireccion_sucursales.Text,
                    tel
                );

                MessageBox.Show("Sucursal insertada.");
                CargarSucursales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar: " + ex.Message);
            }
        }

        private void btnEditar_sucursales_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_sucursales.Text, out int id))
            {
                MessageBox.Show("Selecciona una sucursal.");
                return;
            }

            if (!int.TryParse(txtTelefono_sucursales.Text, out int tel))
            {
                MessageBox.Show("Teléfono inválido.");
                return;
            }

            try
            {
                sucursalBD.EditarSucursal(id, txtNombre_sucursales.Text, txtDireccion_sucursales.Text, tel);
                MessageBox.Show("Sucursal actualizada.");
                CargarSucursales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }


        private void btnEliminar_sucursales_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_sucursales.Text, out int id))
            {
                MessageBox.Show("Selecciona una sucursal.");
                return;
            }

            if (MessageBox.Show("¿Eliminar esta sucursal?", "Confirmación", MessageBoxButton.YesNo)
                != MessageBoxResult.Yes)
                return;

            try
            {
                sucursalBD.EliminarSucursal(id);
                MessageBox.Show("Sucursal eliminada.");
                CargarSucursales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }


        private void btnLimpiar_sucursales_Click(object sender, RoutedEventArgs e)
        {
            txtID_sucursales.Text = "";
            txtNombre_sucursales.Text = "";
            txtDireccion_sucursales.Text = "";
            txtTelefono_sucursales.Text = "";
        }






        // ------------------------------- FIN SUCURSALES -------------------------------


        // ------------------------------- SERVICIO -------------------------------
        private void CargarServicios()
        {
            try
            {
                // Cargar servicios en la tabla
                dgTabla_Servicio.ItemsSource = servicioBD.ObtenerServicios().DefaultView;

                // Cargar lista de paquetes
                var paquetes = servicioBD.ObtenerPaquetes();
                cbPaquetes_servicio.ItemsSource = paquetes.DefaultView;
                cbPaquetes_servicio.DisplayMemberPath = "id_paquete";
                cbPaquetes_servicio.SelectedValuePath = "id_paquete";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar servicios: " + ex.Message);
            }
        }

        private void dgTabla_Servicio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla_Servicio.SelectedItem is DataRowView row)
            {
                txtID_servicio.Text = row["id_servicio"]?.ToString() ?? "";
                txtDescripcion_servicio.Text = row["descripción"]?.ToString() ?? "";
                txtCosto_servicio.Text = row["costo"]?.ToString() ?? "";

                cbPaquetes_servicio.SelectedValue = row["id_paquete"];
            }
        }

        private void btnInsertar_servicio_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtCosto_servicio.Text, out decimal costo))
            {
                MessageBox.Show("Costo inválido.");
                return;
            }

            if (cbPaquetes_servicio.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un paquete.");
                return;
            }

            servicioBD.InsertarServicio(
                txtDescripcion_servicio.Text,
                costo,
                (int)cbPaquetes_servicio.SelectedValue
            );

            MessageBox.Show("Servicio insertado.");
            CargarServicios();
        }

        private void btnEditar_servicio_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_servicio.Text, out int id))
            {
                MessageBox.Show("Selecciona un servicio.");
                return;
            }

            if (!decimal.TryParse(txtCosto_servicio.Text, out decimal costo))
            {
                MessageBox.Show("Costo inválido.");
                return;
            }

            servicioBD.EditarServicio(
                id,
                txtDescripcion_servicio.Text,
                costo,
                (int)cbPaquetes_servicio.SelectedValue
            );

            MessageBox.Show("Servicio actualizado.");
            CargarServicios();
        }

        private void btnEliminar_servicio_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_servicio.Text, out int id))
            {
                MessageBox.Show("Selecciona un servicio.");
                return;
            }

            servicioBD.EliminarServicio(id);

            MessageBox.Show("Servicio eliminado.");
            CargarServicios();
        }

        private void btnLimpiar_servicio_Click(object sender, RoutedEventArgs e)
        {
            txtID_servicio.Text = "";
            txtDescripcion_servicio.Text = "";
            txtCosto_servicio.Text = "";
            cbPaquetes_servicio.SelectedIndex = -1;
        }


        // ------------------------------- FIN SERVICIO -------------------------------





    }
}
