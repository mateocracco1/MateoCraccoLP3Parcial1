using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace MateoCraccoParcial1.Pages
{
    public class altaproductoModel : PageModel
    {
        private readonly IConfiguration _config;

        public altaproductoModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public string? Nombre { get; set; }

        [BindProperty]
        public decimal? Precio { get; set; }

        [BindProperty]
        public int IdCategoria { get; set; }

        //  <select> 
        public List<SelectListItem> Categorias { get; set; } = new();

        public string? Mensaje { get; set; }

        public void OnGet(){
            CargarCategorias();
        }

        public void OnPost(){
           
            CargarCategorias();

            if (string.IsNullOrWhiteSpace(Nombre) || Precio == null || IdCategoria <= 0)
            {
                Mensaje = "Por favor, Complete todos los campos correctamente.";
                return;
            }

            
            string conexionString = _config.GetConnectionString("tienda")!;

            using (SqlConnection conexion = new SqlConnection(conexionString)){
                
                conexion.Open();

                string sql = "INSERT INTO productos (nombre, precio, idCategoria) VALUES (@nombre, @precio, @idCategoria)";

                using (SqlCommand comando = new SqlCommand(sql, conexion)){
                    comando.Parameters.AddWithValue("@nombre", Nombre);
                    comando.Parameters.AddWithValue("@precio", Precio);
                    comando.Parameters.AddWithValue("@idCategoria", IdCategoria);

                    comando.ExecuteNonQuery();
                }
            }

            Mensaje = "Se registró el producto con éxito.";
            Nombre = "";
            Precio = null;
        }

        private void CargarCategorias()
        {
            Categorias.Clear();
            string conexionString = _config.GetConnectionString("tienda")!;

            using (SqlConnection conexion = new SqlConnection(conexionString)){
                conexion.Open();

                string sql = "SELECT idCategoria, descripcion FROM categorias ORDER BY descripcion";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader registros = comando.ExecuteReader())
                {
                    while (registros.Read()){
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
