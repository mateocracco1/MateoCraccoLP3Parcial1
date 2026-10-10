using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MateoCraccoParcial1.Pages{
    // fila para cada  join
    public class FilaProducto
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Categoria { get; set; }
    }

    public class consultaproductoModel : PageModel
    {
        private readonly IConfiguration _config;

        public consultaproductoModel(IConfiguration config)
        {
            _config = config;
        }

        public List<FilaProducto> ListaProductos { get; set; } = new();

        public void OnGet()
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            ListaProductos.Clear();
            string conexionString = _config.GetConnectionString("tienda")!;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"SELECT p.idProducto, 
                                      p.nombre, 
                                      p.precio, 
                                      c.descripcion AS categoria
                               FROM productos AS p
                               JOIN categorias AS c ON c.idCategoria = p.idCategoria
                               ORDER BY p.idProducto";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader registros = comando.ExecuteReader())
                {
                    while (registros.Read())
                    {
                        ListaProductos.Add(new FilaProducto
                        {
                            IdProducto = Convert.ToInt32(registros["idProducto"]),
                            Nombre = registros["nombre"].ToString(),
                            Precio = Convert.ToDecimal(registros["precio"]),
                            Categoria = registros["categoria"].ToString()
                        });
                    }
                }
            }
        }
    }
}