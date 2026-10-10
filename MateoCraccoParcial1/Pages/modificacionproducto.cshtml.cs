using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace MateoCraccoParcial1.Pages
{
    public class modificacionproductoModel : PageModel
    {
        
            private readonly IConfiguration _config;

            public modificacionproductoModel(IConfiguration config)
            {
                _config = config;
            }

            [BindProperty]
            public int? IdProducto { get; set; }

            [BindProperty]
            public string? Nombre { get; set; }

            [BindProperty]
            public decimal? Precio { get; set; }

            [BindProperty]
            public int? IdCategoria { get; set; }

            [BindProperty]
            public string? Accion { get; set; }

            [BindProperty]
            public bool Encontrado { get; set; }

            [BindProperty]
            public string? NombreActual { get; set; }

            [BindProperty]
            public decimal? PrecioActual { get; set; }

            [BindProperty]
            public string? CategoriaActual { get; set; }

            public List<SelectListItem> Categorias { get; set; } = new();

            public string? Mensaje { get; set; }

            public void OnGet()
            {
                CargarCategorias();
            }

            public void OnPost()
            {
                CargarCategorias();

                if (Accion == "Buscar")
                {
                    Buscar();
                }
                else if (Accion == "Modificar")
                {
                    Modificar();
                }
            }

            private void Buscar()
            {
                if (IdProducto == null || IdProducto <= 0)
                {
                    Mensaje = "Ingrese un ID de producto válido para buscar.";
                    Encontrado = false;
                    return;
                }

                string conexionString = _config.GetConnectionString("tienda")!;

                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();

                    string sql = @"SELECT p.nombre, p.precio, p.idCategoria, c.descripcion AS categoria 
                               FROM productos p 
                               JOIN categorias c ON c.idCategoria = p.idCategoria 
                               WHERE p.idProducto = @idProducto";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@idProducto", IdProducto);

                        using (SqlDataReader registro = comando.ExecuteReader())
                        {
                            if (registro.Read())
                            {
                                // datos  para mostrar 
                                NombreActual = registro["nombre"].ToString();
                                PrecioActual = Convert.ToDecimal(registro["precio"]);
                                CategoriaActual = registro["categoria"].ToString();

                                Nombre = NombreActual;
                                Precio = PrecioActual;
                                IdCategoria = Convert.ToInt32(registro["idCategoria"]);

                                Encontrado = true;
                                Mensaje = "Producto encontrado. A continuación puede visualizar sus valores y modificarlos.";
                            }
                            else
                            {
                                Mensaje = "No existe un producto con ese ID.";
                                Encontrado = false;
                                NombreActual = null;
                                PrecioActual = null;
                                CategoriaActual = null;
                                Nombre = null;
                                Precio = null;
                                IdCategoria = null;
                            }
                        }
                    }
                }
            }

            private void Modificar()
            {
                if (IdProducto == null || IdProducto <= 0)
                {
                    Mensaje = "Primero debe buscar un producto valido.";
                    return;
                }

                string conexionString = _config.GetConnectionString("tienda")!;

                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();

                    string sqlBuscar = @"SELECT p.nombre, p.precio, p.idCategoria, c.descripcion AS categoria 
                                     FROM productos p 
                                     JOIN categorias c ON c.idCategoria = p.idCategoria 
                                     WHERE p.idProducto = @idProducto";

                    string dbNombre;
                    decimal dbPrecio;
                    int dbCategoria;
                    string dbDescCategoria;

                    using (SqlCommand cmdBuscar = new SqlCommand(sqlBuscar, conexion))
                    {
                        cmdBuscar.Parameters.AddWithValue("@idProducto", IdProducto);
                        using (SqlDataReader reader = cmdBuscar.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                Mensaje = "El producto no existe en la base de datos.";
                                Encontrado = false;
                                return;
                            }

                            dbNombre = reader["nombre"].ToString()!;
                            dbPrecio = Convert.ToDecimal(reader["precio"]);
                            dbCategoria = Convert.ToInt32(reader["idCategoria"]);
                            dbDescCategoria = reader["categoria"].ToString()!;
                        }
                    }

                    // validacion de nuevos valores
                    string nombreFinal = !string.IsNullOrWhiteSpace(Nombre) ? Nombre.Trim() : dbNombre;
                    decimal precioFinal = (Precio != null && Precio > 0) ? Precio.Value : dbPrecio;
                    int categoriaFinal = (IdCategoria != null && IdCategoria > 0) ? IdCategoria.Value : dbCategoria;

                    
                    bool cambioNombre = (nombreFinal != dbNombre);
                    bool cambioPrecio = (precioFinal != dbPrecio);
                    bool cambioCategoria = (categoriaFinal != dbCategoria);

                    if (!cambioNombre && !cambioPrecio && !cambioCategoria)
                    {
                        NombreActual = dbNombre;
                        PrecioActual = dbPrecio;
                        CategoriaActual = dbDescCategoria;
                        Encontrado = true;

                        Mensaje = "Atención: Debe modificar al menos un dato (Nombre, Precio o Categoría) para poder guardar.";
                        return;
                    }

                    string sqlUpdate = "UPDATE productos SET nombre = @nombre, precio = @precio, idCategoria = @idCategoria WHERE idProducto = @idProducto";

                    using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conexion))
                    {
                        cmdUpdate.Parameters.AddWithValue("@nombre", nombreFinal);
                        cmdUpdate.Parameters.AddWithValue("@precio", precioFinal);
                        cmdUpdate.Parameters.AddWithValue("@idCategoria", categoriaFinal);
                        cmdUpdate.Parameters.AddWithValue("@idProducto", IdProducto);

                        int filas = cmdUpdate.ExecuteNonQuery();

                        if (filas >= 1)
                        {
                            Mensaje = "¡Producto modificado con éxito!";
                            NombreActual = nombreFinal;
                            PrecioActual = precioFinal;

                            var catItem = Categorias.FirstOrDefault(c => c.Value == categoriaFinal.ToString());
                            CategoriaActual = catItem != null ? catItem.Text : dbDescCategoria;

                            Nombre = nombreFinal;
                            Precio = precioFinal;
                            IdCategoria = categoriaFinal;
                            Encontrado = true;
                        }
                        else
                        {
                            Mensaje = "No se pudo actualizar el producto.";
                        }
                    }
                }
            }

            private void CargarCategorias()
            {
                Categorias.Clear();
                string conexionString = _config.GetConnectionString("tienda")!;

                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();
                    string sql = "SELECT idCategoria, descripcion FROM categorias ORDER BY descripcion";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    using (SqlDataReader registros = comando.ExecuteReader())
                    {
                        while (registros.Read())
                        {
                            Categorias.Add(new SelectListItem(
                                registros["descripcion"].ToString(),
                                registros["idCategoria"].ToString()
                            ));
                        }
                    }
                }
            }
        }
    }
