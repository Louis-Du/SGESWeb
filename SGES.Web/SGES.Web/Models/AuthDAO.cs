using System;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class AuthDAO
    {
        private readonly Conexion cn = new Conexion();


        public UsuarioSesion Login(int id, string contrasena)
        {

            // 1. Buscar administrador
            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                SELECT 
                    idAdmin,
                    nombreAdmin
                FROM Administrador
                WHERE idAdmin = @id
                AND passwordHash = @pass";


                SqlCommand cmd =
                    new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pass", contrasena);


                con.Open();


                SqlDataReader dr =
                    cmd.ExecuteReader();


                if(dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id =
                        Convert.ToInt32(dr["idAdmin"]),

                        Nombre =
                        dr["nombreAdmin"].ToString(),

                        Tipo = "Administrador"
                    };
                }
            }



            // 2. Buscar aprendiz
            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                SELECT
                    idApr,
                    nombreApr
                FROM Aprendiz
                WHERE idApr = @id
                AND passwordHash = @pass";


                SqlCommand cmd =
                    new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pass", contrasena);


                con.Open();


                SqlDataReader dr =
                    cmd.ExecuteReader();



                if(dr.Read())
                {

                    return new UsuarioSesion
                    {
                        Id =
                        Convert.ToInt32(dr["idApr"]),


                        Nombre =
                        dr["nombreApr"].ToString(),


                        Tipo = "Aprendiz"
                    };
                }

            }


            return null;
        }



        public UsuarioSesion ObtenerUsuarioPorId(int id)
        {

            // administrador
            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                SELECT
                    idAdmin,
                    nombreAdmin
                FROM Administrador
                WHERE idAdmin=@id";


                SqlCommand cmd =
                    new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@id", id);


                con.Open();


                SqlDataReader dr =
                    cmd.ExecuteReader();



                if(dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id =
                        Convert.ToInt32(dr["idAdmin"]),

                        Nombre =
                        dr["nombreAdmin"].ToString(),

                        Tipo =
                        "Administrador"
                    };
                }
            }



            // aprendiz
            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                SELECT
                    idApr,
                    nombreApr
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
                    return new UsuarioSesion
                    {
                        Id =
                        Convert.ToInt32(dr["idApr"]),


                        Nombre =
                        dr["nombreApr"].ToString(),


                        Tipo =
                        "Aprendiz"
                    };
                }

            }


            return null;
        }



        public void ActualizarPassword(
            int id,
            string password,
            string tipo)
        {

            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql;


                if(tipo == "Administrador")
                {
                    sql = @"
                    UPDATE Administrador
                    SET passwordHash=@password
                    WHERE idAdmin=@id";
                }
                else
                {
                    sql = @"
                    UPDATE Aprendiz
                    SET passwordHash=@password
                    WHERE idApr=@id";
                }



                SqlCommand cmd =
                    new SqlCommand(sql, con);



                cmd.Parameters.AddWithValue(
                    "@password",
                    password
                );


                cmd.Parameters.AddWithValue(
                    "@id",
                    id
                );


                con.Open();

                cmd.ExecuteNonQuery();

            }

        }

    }
}