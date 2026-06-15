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
                string sql = "SELECT A.idApr," +
                    " A.nombreApr," + "A.emailApr," + "A.contactoApr " + 
                    "FROM Aprendiz A " + "INNER JOIN Inscripciones I ON A.idApr = I.idApr " 
                    + "WHERE I.idEvento = @idEvento";

                SqlCommand cmd = new SqlCommand(sql, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new AprendizModel
                    {
                        IdApr = Convert.ToInt32(dr["idApr"]),
                        NombreApr = dr["nombreApr"].ToString(),
                        EmailApr = dr["emailApr"].ToString(),
                        ContactoApr = dr["contactoApr"].ToString()
                    });
                }
            }

            return lista;
        }

        public List<AprendizModel> ObtenerAprendicesDisponibles(int idEvento, int idAprActual)
        {
            List<AprendizModel> lista = new List<AprendizModel>();

            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
            SELECT idApr, nombreApr, emailApr, contactoApr
            FROM Aprendiz
            WHERE idApr NOT IN (
                SELECT idApr FROM Inscripciones WHERE idEvento = @idEvento
            )
            AND idApr <> @idAprActual";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);
                cmd.Parameters.AddWithValue("@idAprActual", idAprActual);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new AprendizModel
                    {
                        IdApr = Convert.ToInt32(dr["idApr"]),
                        NombreApr = dr["nombreApr"].ToString(),
                        EmailApr = dr["emailApr"].ToString(),
                        ContactoApr = dr["contactoApr"].ToString()
                    });
                }
            }
            return lista;
        }
    }
}