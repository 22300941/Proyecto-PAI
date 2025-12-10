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
        private transaccionesHelper transaccionesBD;
        private paqueteHelper paqueteBD;
        private clientesHelper clientesBD;
        private proveedoresHelper proveedoresBD;







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

            transaccionesBD = new transaccionesHelper(cadenaConexion);
            paqueteBD = new paqueteHelper(cadenaConexion);

            clientesBD = new clientesHelper(cadenaConexion);
            proveedoresBD = new proveedoresHelper(cadenaConexion);







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
                //MessageBox.Show("Error al cargar trabajadores: " + ex.Message);
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

                if (tab.Header.ToString() == "Transacciones")
                {
                    CargarTransacciones();
                }
                // Si selecciona Clientes
                if (tab.Header.ToString() == "Clientes")
                {
                    CargarClientes();
                }

                // Si selecciona Proveedores
                if (tab.Header.ToString() == "Proveedores")
                {
                    CargarProveedores();
                }
                if (tab.Header.ToString() == "Paquetes")
                {
                    CargarPaquetes();
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



        // ------------------------------- TRANSACCIONES -------------------------------

        private void CargarTransacciones()
        {
            try
            {
                dgTabla_transacciones.ItemsSource =
                    transaccionesBD.ObtenerTransacciones().DefaultView;

                // Cargar paquetes para el ComboBox
                var paquetes = servicioBD.ObtenerPaquetes();
                cbPaquetes_transacciones.ItemsSource = paquetes.DefaultView;
                cbPaquetes_transacciones.DisplayMemberPath = "id_paquete";
                cbPaquetes_transacciones.SelectedValuePath = "id_paquete";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar transacciones: " + ex.Message);
            }
        }

        private void dgTabla_transacciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla_transacciones.SelectedItem is DataRowView row)
            {
                txtID_transacciones.Text = row["id_transaccion"]?.ToString() ?? "";
                txtTotal_transacciones.Text = row["total"]?.ToString() ?? "";
                txtMetodo_transacciones.Text = row["metodo_pago"]?.ToString() ?? "";
                cbPaquetes_transacciones.SelectedValue = row["id_paquete"];

                // Convertir SQL DateTime → DatePicker
                if (DateTime.TryParse(row["fecha_pago"]?.ToString(), out DateTime fechaSQL))
                {
                    dpFechaPago.SelectedDate = fechaSQL;
                }
                else
                {
                    dpFechaPago.SelectedDate = null;
                }
            }

        }

        private void btnInsertar_transacciones_Click(object sender, RoutedEventArgs e)
        {
            if (dpFechaPago.SelectedDate == null)
            {
                MessageBox.Show("Selecciona una fecha.");
                return;
            }

            DateOnly fecha = DateOnly.FromDateTime(dpFechaPago.SelectedDate.Value);

            if (!int.TryParse(txtTotal_transacciones.Text, out int total))
            {
                MessageBox.Show("Total inválido.");
                return;
            }

            if (cbPaquetes_transacciones.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un paquete.");
                return;
            }

            transaccionesBD.InsertarTransaccion(
                fecha,
                total,
                txtMetodo_transacciones.Text,
                (int)cbPaquetes_transacciones.SelectedValue
            );

            MessageBox.Show("Transacción insertada.");
            CargarTransacciones();
        }


        private void btnEditar_transacciones_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_transacciones.Text, out int id))
            {
                MessageBox.Show("Selecciona una transacción.");
                return;
            }

            if (dpFechaPago.SelectedDate == null)
            {
                MessageBox.Show("Selecciona una fecha.");
                return;
            }

            DateOnly fecha = DateOnly.FromDateTime(dpFechaPago.SelectedDate.Value);

            if (!int.TryParse(txtTotal_transacciones.Text, out int total))
            {
                MessageBox.Show("Total inválido.");
                return;
            }

            transaccionesBD.EditarTransaccion(
                id,
                fecha,
                total,
                txtMetodo_transacciones.Text,
                (int)cbPaquetes_transacciones.SelectedValue
            );

            MessageBox.Show("Transacción actualizada.");
            CargarTransacciones();
        }

        private void btnEliminar_transacciones_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_transacciones.Text, out int id))
            {
                MessageBox.Show("Selecciona una transacción.");
                return;
            }

            transaccionesBD.EliminarTransaccion(id);

            MessageBox.Show("Transacción eliminada.");
            CargarTransacciones();
        }

        private void btnLimpiar_transacciones_Click(object sender, RoutedEventArgs e)
        {
            txtID_transacciones.Text = "";
            dpFechaPago.SelectedDate = null;
            txtTotal_transacciones.Text = "";
            txtMetodo_transacciones.Text = "";
            cbPaquetes_transacciones.SelectedIndex = -1;

            dgTabla_transacciones.UnselectAll();
        }





        // ------------------------------- FIN TRANSACCIONES -------------------------------



        // ------------------------------- PAQUETES -------------------------------



        private void CargarPaquetes()
        {
            try
            {
                // Cargar la tabla
                dgTabla_paquetes.ItemsSource = paqueteBD.ObtenerPaquetes().DefaultView;

                // ===== Cargar Sucursales =====
                var sucursales = sucursalBD.ObtenerSucursales();
                cbSucursal_paquetes.ItemsSource = sucursales.DefaultView;
                cbSucursal_paquetes.DisplayMemberPath = "nombre"; // <-- Se ve bonito   
                cbSucursal_paquetes.SelectedValuePath = "id_sucursal";

                // ===== Cargar Clientes =====
                var clientes = clientesBD.ObtenerClientes();
                cbCliente_paquetes.ItemsSource = clientes.DefaultView;
                cbCliente_paquetes.DisplayMemberPath = "nombre"; // <-- Puedes mostrar nombre
                cbCliente_paquetes.SelectedValuePath = "id_cliente";

                // ===== Cargar Proveedores =====
                var proveedores = proveedoresBD.ObtenerProveedores();
                cbProveedor_paquetes.ItemsSource = proveedores.DefaultView;
                cbProveedor_paquetes.DisplayMemberPath = "nombre";
                cbProveedor_paquetes.SelectedValuePath = "id_proveedor";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar paquetes: " + ex.Message);
            }
        }



        private void dgTabla_paquetes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla_paquetes.SelectedItem is DataRowView row)
            {
                txtID_paquete.Text = row["id_paquete"]?.ToString() ?? "";
                txtTipo_paquetes.Text = row["tipo_paquete"]?.ToString() ?? "";
                txtCodigoBarras_paquetes.Text = row["codigo_barras"]?.ToString() ?? "";

                cbSucursal_paquetes.SelectedValue = row["id_sucursal"];
                cbCliente_paquetes.SelectedValue = row["id_cliente"];
                cbProveedor_paquetes.SelectedValue = row["id_proveedor"];

                if (DateTime.TryParse(row["fecha_entrada"]?.ToString(), out DateTime fe))
                    dpFechaEntrada_paquetes.SelectedDate = fe;
                else
                    dpFechaEntrada_paquetes.SelectedDate = null;

                if (DateTime.TryParse(row["fecha_salida"]?.ToString(), out DateTime fs))
                    dpFechaSalida_paquetes.SelectedDate = fs;
                else
                    dpFechaSalida_paquetes.SelectedDate = null;
            }
        }

        private void btnInsertar_paquetes_Click(object sender, RoutedEventArgs e)
        {
            if (dpFechaEntrada_paquetes.SelectedDate == null ||
                dpFechaSalida_paquetes.SelectedDate == null)
            {
                MessageBox.Show("Selecciona ambas fechas.");
                return;
            }

            if (cbSucursal_paquetes.SelectedValue == null ||
                cbCliente_paquetes.SelectedValue == null ||
                cbProveedor_paquetes.SelectedValue == null)
            {
                MessageBox.Show("Selecciona sucursal, cliente y proveedor.");
                return;
            }

            DateOnly entrada = DateOnly.FromDateTime(dpFechaEntrada_paquetes.SelectedDate.Value);
            DateOnly salida = DateOnly.FromDateTime(dpFechaSalida_paquetes.SelectedDate.Value);

            paqueteBD.InsertarPaquete(
                entrada,
                salida,
                txtTipo_paquetes.Text,
                txtCodigoBarras_paquetes.Text,
                (int)cbSucursal_paquetes.SelectedValue,
                (int)cbCliente_paquetes.SelectedValue,
                (int)cbProveedor_paquetes.SelectedValue
            );

            MessageBox.Show("Paquete insertado.");
            CargarPaquetes();
        }


        private void btnEditar_paquetes_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_paquete.Text, out int id))
            {
                MessageBox.Show("Selecciona un paquete.");
                return;
            }

            DateOnly entrada = DateOnly.FromDateTime(dpFechaEntrada_paquetes.SelectedDate.Value);
            DateOnly salida = DateOnly.FromDateTime(dpFechaSalida_paquetes.SelectedDate.Value);

            paqueteBD.EditarPaquete(
                id,
                entrada,
                salida,
                txtTipo_paquetes.Text,
                txtCodigoBarras_paquetes.Text,
                (int)cbSucursal_paquetes.SelectedValue,
                (int)cbCliente_paquetes.SelectedValue,
                (int)cbProveedor_paquetes.SelectedValue
            );

            MessageBox.Show("Paquete actualizado.");
            CargarPaquetes();
        }


        private void btnEliminar_paquetes_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_paquete.Text, out int id))
            {
                MessageBox.Show("Selecciona un paquete.");
                return;
            }

            paqueteBD.EliminarPaquete(id);

            MessageBox.Show("Paquete eliminado.");
            CargarPaquetes();
        }

        private void btnLimpiar_paquetes_Click(object sender, RoutedEventArgs e)
        {
            txtID_paquete.Text = "";
            txtTipo_paquetes.Text = "";
            txtCodigoBarras_paquetes.Text = "";

            cbSucursal_paquetes.SelectedIndex = -1;
            cbCliente_paquetes.SelectedIndex = -1;
            cbProveedor_paquetes.SelectedIndex = -1;

            dpFechaEntrada_paquetes.SelectedDate = null;
            dpFechaSalida_paquetes.SelectedDate = null;

            dgTabla_paquetes.UnselectAll();
        }


        // ------------------------------- FIN PAQUETES -------------------------------

        // ------------------------------- CLIENTES -------------------------------


        private void CargarClientes()
        {
            try
            {
                // Cargar tabla de clientes
                dgTabla_clientes.ItemsSource = clientesBD.ObtenerClientes().DefaultView;

                // Cargar sucursales para el ComboBox (FK id_sucursal)
                var sucursales = sucursalBD.ObtenerSucursales();
                cbSucursal_clientes.ItemsSource = sucursales.DefaultView;
                cbSucursal_clientes.DisplayMemberPath = "nombre";
                cbSucursal_clientes.SelectedValuePath = "id_sucursal";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
        }

        private void dgTabla_clientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla_clientes.SelectedItem is DataRowView row)
            {
                txtID_clientes.Text = row["id_cliente"]?.ToString() ?? "";
                txtNombre_clientes.Text = row["nombre"]?.ToString() ?? "";
                txtApellido_clientes.Text = row["apellido"]?.ToString() ?? "";
                txtTelefono_clientes.Text = row["telefono"]?.ToString() ?? "";
                txtDireccion_clientes.Text = row["direccion"]?.ToString() ?? "";
                txtFirma_clientes.Text = row["firma"]?.ToString() ?? "";

                // FK a sucursal
                cbSucursal_clientes.SelectedValue = row["id_sucursal"];
            }
        }

        private void btnInsertar_clientes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbSucursal_clientes.SelectedValue == null)
                {
                    MessageBox.Show("Selecciona una sucursal.");
                    return;
                }

                clientesBD.InsertarCliente(
                    txtTelefono_clientes.Text,
                    txtDireccion_clientes.Text,
                    txtFirma_clientes.Text,
                    txtNombre_clientes.Text,
                    txtApellido_clientes.Text,
                    (int)cbSucursal_clientes.SelectedValue
                );

                MessageBox.Show("Cliente insertado correctamente.");
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar cliente: " + ex.Message);
            }
        }

        private void btnEditar_clientes_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_clientes.Text, out int id))
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }

            if (cbSucursal_clientes.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una sucursal.");
                return;
            }

            try
            {
                clientesBD.EditarCliente(
                    id,
                    txtTelefono_clientes.Text,
                    txtDireccion_clientes.Text,
                    txtFirma_clientes.Text,
                    txtNombre_clientes.Text,
                    txtApellido_clientes.Text,
                    (int)cbSucursal_clientes.SelectedValue
                );

                MessageBox.Show("Cliente actualizado.");
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar cliente: " + ex.Message);
            }
        }


        private void btnEliminar_clientes_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_clientes.Text, out int id))
            {
                MessageBox.Show("Selecciona un cliente.");
                return;
            }

            if (MessageBox.Show("¿Eliminar este cliente?", "Confirmación", MessageBoxButton.YesNo)
                != MessageBoxResult.Yes)
                return;

            try
            {
                clientesBD.EliminarCliente(id);
                MessageBox.Show("Cliente eliminado.");
                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar cliente: " + ex.Message);
            }
        }


        private void btnLimpiar_clientes_Click(object sender, RoutedEventArgs e)
        {
            txtID_clientes.Text = "";
            txtNombre_clientes.Text = "";
            txtApellido_clientes.Text = "";
            txtTelefono_clientes.Text = "";
            txtDireccion_clientes.Text = "";
            txtFirma_clientes.Text = "";
            cbSucursal_clientes.SelectedIndex = -1;

            dgTabla_clientes.UnselectAll();
        }


        // ------------------------------- FIN CLIENTES -------------------------------

        // ------------------------------- PROVEEDORES -------------------------------

        private void CargarProveedores()
        {
            try
            {
                dgTabla_proveedores.ItemsSource = proveedoresBD.ObtenerProveedores().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedores: " + ex.Message);
            }
        }


        private void dgTabla_proveedores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTabla_proveedores.SelectedItem is DataRowView row)
            {
                txtID_proveedores.Text = row["id_proveedor"]?.ToString() ?? "";
                txtNombre_proveedores.Text = row["nombre"]?.ToString() ?? "";
                txtTelefono_proveedores.Text = row["telefono"]?.ToString() ?? "";
            }
        }

        private void btnInsertar_proveedores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                proveedoresBD.InsertarProveedor(
                    txtNombre_proveedores.Text,
                    txtTelefono_proveedores.Text
                );

                MessageBox.Show("Proveedor insertado.");
                CargarProveedores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar proveedor: " + ex.Message);
            }
        }

        private void btnEditar_proveedores_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_proveedores.Text, out int id))
            {
                MessageBox.Show("Selecciona un proveedor.");
                return;
            }

            try
            {
                proveedoresBD.EditarProveedor(
                    id,
                    txtNombre_proveedores.Text,
                    txtTelefono_proveedores.Text
                );

                MessageBox.Show("Proveedor actualizado.");
                CargarProveedores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar proveedor: " + ex.Message);
            }
        }

        private void btnEliminar_proveedores_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtID_proveedores.Text, out int id))
            {
                MessageBox.Show("Selecciona un proveedor.");
                return;
            }

            if (MessageBox.Show("¿Eliminar este proveedor?", "Confirmación", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                proveedoresBD.EliminarProveedor(id);
                MessageBox.Show("Proveedor eliminado.");
                CargarProveedores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar proveedor: " + ex.Message);
            }
        }

        private void btnLimpiar_proveedores_Click(object sender, RoutedEventArgs e)
        {
            txtID_proveedores.Text = "";
            txtNombre_proveedores.Text = "";
            txtTelefono_proveedores.Text = "";
            dgTabla_proveedores.UnselectAll();
        }

        // ------------------------------- FIN PROVEEDORES -------------------------------


    }
}
