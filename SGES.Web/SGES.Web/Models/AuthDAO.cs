using System;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class AuthDAO
    {
        private readonly Conexion cn = new Conexion();

        public UsuarioSesion Login(int id, string contrasena)
        {
            // 1. Buscar en Usuario (Administrador)
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT idUser, nombreUser, tipoUser
                               FROM Usuario
                               WHERE idUser = @id
                                 AND passwordHash = @pass";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pass", contrasena);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id = (int)dr["idUser"],
                        Nombre = dr["nombreUser"].ToString(),
                        Tipo = dr["tipoUser"].ToString()
                    };
                }
            }

            // 2. Buscar en Aprendiz (join con Usuario para obtener passwordHash y tipoUser)
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT A.idApr, A.nombreApr, U.tipoUser
                               FROM Aprendiz A
                               JOIN Usuario U ON A.idUser = U.idUser
                               WHERE A.idApr = @id
                                 AND U.passwordHash = @pass";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pass", contrasena);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id = (int)dr["idApr"],
                        Nombre = dr["nombreApr"].ToString(),
                        Tipo = dr["tipoUser"].ToString()
                    };
                }
            }

            return null;
        }

        public UsuarioSesion ObtenerUsuarioPorId(int id)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT idUser, nombreUser, tipoUser FROM Usuario WHERE idUser = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id = Convert.ToInt32(dr["idUser"]),
                        Nombre = dr["nombreUser"].ToString(),
                        Tipo = dr["tipoUser"].ToString()
                    };
                }
            }

            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT A.idApr, A.nombreApr, U.tipoUser, A.idUser
                       FROM Aprendiz A
                       JOIN Usuario U ON A.idUser = U.idUser
                       WHERE A.idApr = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id = Convert.ToInt32(dr["idApr"]),
                        Nombre = dr["nombreApr"].ToString(),
                        Tipo = dr["tipoUser"].ToString()
                    };
                }
            }

            return null;
        }

        public void ActualizarPassword(int id, string password)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
                    UPDATE Usuario
                    SET passwordHash = @password
                    WHERE idUser = @id";

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
        
    }
}
