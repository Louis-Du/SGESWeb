using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;

namespace SGES.Web.Models
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
            int intentos = 5;
            int espera = 3000;

            while (intentos > 0)
            {
                try
                {
                    var conn = new SqlConnection(cadenaConexion);
                    conn.Open();
                    return conn;
                }
                catch (SqlException ex) when (ex.Number == 40613 || ex.Number == 40501 || ex.Number == 40143 || ex.Number == 233)
                {
                    intentos--;
                    if (intentos == 0) throw;
                    Thread.Sleep(espera);
                    espera *= 2;
                }
            }
            return new SqlConnection(cadenaConexion);
        }
    }
}
