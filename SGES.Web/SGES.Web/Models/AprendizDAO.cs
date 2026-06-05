using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace SGES.Web.Models
{
    public class AprendizDAO
    {
        private readonly Conexion cn = new Conexion();

        public List<AprendizModel> ObtenerAprendicesPorEvento(int idEvento)
        {
            List<AprendizModel> lista = new List<AprendizModel>();

            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = "SELECT * FROM Aprendiz";

                SqlCommand cmd = new SqlCommand(sql, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new AprendizModel
                    {
                        idApr = Convert.ToInt32(dr["idApr"]),
                        nombreApr = dr["nombreApr"].ToString(),
                        EmailApr = dr["emailApr"].ToString(),
                        ContactoApr = dr["contactoApr"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}