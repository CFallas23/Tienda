namespace Tienda.Models
{
    public class AccesoDatos
    {
        private readonly string _conexion;
        public AccesoDatos(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
