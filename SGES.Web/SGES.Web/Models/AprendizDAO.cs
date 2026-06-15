using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class AprendizDAO
    {
        private readonly Conexion cn = new Conexion();


        public List<AprendizModel> ObtenerAprendicesPorEvento(int idEvento)
        {
            List<AprendizModel> lista = new List<AprendizModel>();

            using(SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
                SELECT 
                    A.idApr,
                    A.nombreApr,
                    A.emailApr,
                    A.contactoApr,
                    A.codigoFic
                FROM Aprendiz A
                INNER JOIN Inscripciones I
                ON A.idApr = I.idApr
                WHERE I.idEvento = @idEvento";


                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue(
                    "@idEvento",
                    idEvento
                );


                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();


                while(dr.Read())
                {
                    lista.Add(new AprendizModel
                    {
                        IdApr = Convert.ToInt32(dr["idApr"]),

                        NombreApr = dr["nombreApr"].ToString(),

                        EmailApr = dr["emailApr"].ToString(),

                        ContactoApr = dr["contactoApr"].ToString(),

                        CodigoFic = Convert.ToInt32(dr["codigoFic"])
                    });
                }
            }


            return lista;
        }


        public AprendizModel ObtenerPorId(int id)
        {
            using(SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
                SELECT *
                FROM Aprendiz
                WHERE idApr=@id";


                SqlCommand cmd =
                    new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@id", id);


                con.Open();


                SqlDataReader dr =
                    cmd.ExecuteReader();


                if(dr.Read())
                {
                    return new AprendizModel
                    {
                        IdApr =
                        Convert.ToInt32(dr["idApr"]),


                        NombreApr =
                        dr["nombreApr"].ToString(),


                        EmailApr =
                        dr["emailApr"].ToString(),


                        ContactoApr =
                        dr["contactoApr"].ToString(),


                        CodigoFic =
                        Convert.ToInt32(dr["codigoFic"])
                    };
                }
            }


            return null;
        }
    }
}