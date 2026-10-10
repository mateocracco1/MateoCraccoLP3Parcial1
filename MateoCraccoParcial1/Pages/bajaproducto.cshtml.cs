using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace MateoCraccoParcial1.Pages{
    public class FilaProductoBaja{
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Categoria { get; set; }
    }

    public class bajaproductoModel : PageModel{
        private readonly IConfiguration _config;

        public bajaproductoModel(IConfiguration config){
            _config = config;
        }

        public List<FilaProductoBaja> ListaProductos { get; set; } = new();

        public string? Mensaje { get; set; }

        public void OnGet()
        {
            CargarProductos();
        }

        
        public void OnPostEliminar(int idProducto){
            string conexionString = _config.GetConnectionString("tienda")!;

            using (SqlConnection conexion = new SqlConnection(conexionString)){
                conexion.Open();

                string sql = "DELETE FROM productos WHERE idProducto = @idProducto";

                using (SqlCommand comando = new SqlCommand(sql, conexion)){
                    comando.Parameters.AddWithValue("@idProducto", idProducto);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas >= 1){
                        Mensaje = $"Se eliminó correctamente el producto con ID {idProducto}.";
                    }
                    else{
                        Mensaje = "No se encontró el producto o ya fue eliminado.";
                    }
                }
            }

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
                               FROM productos p 
                               JOIN categorias c ON c.idCategoria = p.idCategoria 
                               ORDER BY p.idProducto";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader registros = comando.ExecuteReader())
                {
                    while (registros.Read())
                    {
                        ListaProductos.Add(new FilaProductoBaja
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