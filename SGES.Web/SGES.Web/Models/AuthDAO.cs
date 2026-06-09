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
    }
}
