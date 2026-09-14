using Proyecto_TFG.Services;
using MySqlConnector;
using Proyecto_TFG.Models;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_TFG.Functions

{
    public class MySqlFunctions : IMysqlFunctions
    {
        const string connectionString = "";
        public string ConnectionMessage { get; private set; }


        /// <summary>
        /// Conectar a la base de datos de FacturaEdi
        /// </summary>
        /// <returns></returns>

        public Usuario ConectToFacturaEdi(Secrets secrets)
        {

            MySqlConnection connection;
            // Intentar abrir la conexión
            using (connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    ConnectionMessage = "Conexión a MariaDB exitosa.";
                    Usuario user = RecuperarDatosUsuario(connection, secrets);
                    return user;
                }
                catch (Exception ex)
                {
                    ConnectionMessage = $"Error al conectar a la base de datos: {ex.Message}";
                }

            }
            //Hay que devolver un mensaje de error si no se puede conectar a la base de datos
            return null;
        }




        private Usuario RecuperarDatosUsuario(MySqlConnection sqlConnection, Secrets secrets)
        {
            // Datos del usuario
            string email = secrets.usuario;
            string password = secrets.password;

            // Objeto para almacenar los datos del usuario
            Usuario usuario = null;

            // Comando SQL seguro con parámetros
            string sqlCommand = "SELECT nombre, email, id_empresa, id_usuario FROM usuarios WHERE email = @Email AND contrasena = @Password";

            using (var command = new MySqlCommand(sqlCommand, sqlConnection))
            {
                // Agregar parámetros para evitar inyección SQL
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Mapear los datos al objeto Usuario
                        usuario = new Usuario
                        {
                            Nombre = reader["nombre"] as string,
                            Email = reader["email"] as string,
                            IdEmpresa = reader["id_empresa"] != DBNull.Value ? Convert.ToInt32(reader["id_empresa"]) : -1, //se asigna -1 para indicar que no hay empresa
                            IdUsuario = Convert.ToInt32(reader["id_usuario"])

                        };
                    }
                }
            }

            return usuario;
        }

        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            string query = "SELECT nombre, email, id_empresa FROM usuarios WHERE id_usuario = @IdUsuario";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Nombre = reader["nombre"] as string,
                                Email = reader["email"] as string,
                                IdEmpresa = reader["id_empresa"] != DBNull.Value ? Convert.ToInt32(reader["id_empresa"]) : -1
                            };
                        }
                    }
                }
            }
            return null; // Si no se encuentra el usuario
        }

        public Empresa ObtenerEmpresaPorId(int idEmpresa)
        {
            string query = "SELECT nombre_empresa, cif, email_empresa FROM empresas WHERE id_empresa = @IdEmpresa";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdEmpresa", idEmpresa);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Empresa
                            {
                                NombreEmpresa = reader["nombre_empresa"] as string,
                                CIF = reader["cif"] as string,
                                EmailEmpresa = reader["email_empresa"] as string
                            };
                        }
                    }

                }

            }
            return null;
        }

        public Usuario ActualizarProveedores(Secrets secrets)
        {

            MySqlConnection connection;
            // Intentar abrir la conexión
            using (connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    ConnectionMessage = "Conexión a MariaDB exitosa.";
                    Usuario user = RecuperarDatosUsuario(connection, secrets);
                    return user;
                }
                catch (Exception ex)
                {
                    ConnectionMessage = $"Error al conectar a la base de datos: {ex.Message}";
                }

            }
            //Hay que devolver un mensaje de error si no se puede conectar a la base de datos
            return null;
        }

        public List<Proveedor> RecuperarProveedores()
        {
            List<Proveedor> listadoProveedores = new List<Proveedor>();

            // Comando SQL seguro
            string sqlCommandProveedor = "SELECT id, nombre_razon_social, cif, direccion, telefono, email FROM Proveedor;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(sqlCommandProveedor, connection))
                {

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idProveedor = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : -1;
                            // Mapear los datos al objeto Usuario
                            Proveedor proveedor = new Proveedor
                            {
                                proveedor_id = idProveedor.ToString(),
                                nombre_razon_social = reader["nombre_razon_social"] as string,
                                nif_cif = reader["cif"] as string,
                                direccion = ObtenerDireccionPorProveedor(idProveedor),
                                contacto_principal = ObtenerContactoPorProveedor(idProveedor)
                            };
                            listadoProveedores.Add(proveedor);
                        }
                    }
                }
            }
            return listadoProveedores;
        }

        private Proveedor.Direccion ObtenerDireccionPorProveedor(int idProveedor)
        {
            string sqlCommandDireccion = "SELECT calle, ciudad, pais, codigo_postal FROM DireccionProveedor WHERE id_proveedor = @idProveedor";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            using (var command = new MySqlCommand(sqlCommandDireccion, connection))
            {
                command.Parameters.AddWithValue("@idProveedor", idProveedor);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Proveedor.Direccion
                        {
                            calle = reader["calle"] as string,
                            ciudad = reader["ciudad"] as string,
                            pais = reader["pais"] as string,
                            codigo_postal = reader["codigo_postal"] as string
                        };
                    }
                }
            }
            connection.Close();

            return null; // Si no se encuentra una dirección
        }

        private Proveedor.Contacto_Principal ObtenerContactoPorProveedor(int idProveedor)
        {
            string sqlCommandContacto = "SELECT nombre, cargo, telefono, correo_electronico FROM ContactoPrincipalProveedor WHERE id_proveedor = @idProveedor";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            using (var command = new MySqlCommand(sqlCommandContacto, connection))
            {
                command.Parameters.AddWithValue("@idProveedor", idProveedor);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Proveedor.Contacto_Principal
                        {
                            nombre = reader["nombre"] as string,
                            cargo = reader["cargo"] as string,
                            telefono = reader["telefono"] as string,
                            correo_electronico = reader["correo_electronico"] as string
                        };
                    }
                }
            }
            connection.Close();
            return null; // Si no se encuentra un contacto
        }

        public List<Producto> RecuperarProductos()
        {
            List<Producto> listadoProductos = new List<Producto>();

            // Comando SQL seguro
            string sqlCommandProducto = "SELECT id, codigo, nombre, descripcion, precio_unitario, unidades_disponibles, stock_maximo, id_proveedor FROM Producto;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(sqlCommandProducto, connection))
                {

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idProducto = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : -1;
                            int idProveedor = reader["id_proveedor"] != DBNull.Value ? Convert.ToInt32(reader["id_proveedor"]) : -1;
                            // Mapear los datos al objeto Usuario
                            Producto producto = new Producto
                            {
                                producto_id = idProducto.ToString(),
                                codigo = reader["codigo"] as string,
                                nombre = reader["nombre"] as string,
                                descripcion = reader["descripcion"] as string,
                                precio_unitario = reader["precio_unitario"] != DBNull.Value ? Convert.ToInt32(reader["precio_unitario"]) : -1,
                                unidades_disponibles = reader["unidades_disponibles"] != DBNull.Value ? Convert.ToInt32(reader["unidades_disponibles"]) : -1,
                                stock_maximo = reader["stock_maximo"] != DBNull.Value ? Convert.ToInt32(reader["stock_maximo"]) : -1,
                                proveedor_id = idProveedor.ToString(),
                                CantidadProducto = 0,
                                SubtotalProducto = 0
                            };

                            listadoProductos.Add(producto);
                        }
                    }
                }
            }
            return listadoProductos;
        }
    }
}
