using System.Configuration;
using System.Data.SqlClient;

namespace SGESWeb.Models
{
    public class Conexion
    {
        private readonly string cadenaConexion;

        public Conexion()
        {
            cadenaConexion = ConfigurationManager
                .ConnectionStrings["SGESConnection"]
                .ConnectionString;
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}