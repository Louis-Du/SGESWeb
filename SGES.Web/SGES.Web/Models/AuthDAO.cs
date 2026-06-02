using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class AuthDAO
    {
        private readonly Conexion cn = new Conexion();

        /// <summary>
        /// Busca el usuario primero en la tabla Usuario (admin),
        /// luego en Aprendiz. Devuelve null si no existe o la
        /// contraseña no coincide.
        /// </summary>
        public UsuarioSesion Login(int id, string contrasena)
        {
            // 1. Buscar en tabla Usuario (Administrador)
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT idUser, nombreUser, tipoUser
                               FROM Usuario
                               WHERE idUser = @id
                                 AND contraseñaUser = @pass";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id",   id);
                cmd.Parameters.AddWithValue("@pass", contrasena);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id     = (int)dr["idUser"],
                        Nombre = dr["nombreUser"].ToString(),
                        Tipo   = dr["tipoUser"].ToString()
                    };
                }
            }

            // 2. Buscar en tabla Aprendiz
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT idApr, nombreApr, tipoUser
                               FROM Aprendiz
                               WHERE idApr = @id
                                 AND contraseñaUser = @pass";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id",   id);
                cmd.Parameters.AddWithValue("@pass", contrasena);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id     = (int)dr["idApr"],
                        Nombre = dr["nombreApr"].ToString(),
                        Tipo   = dr["tipoUser"].ToString()
                    };
                }
            }

            return null;
        }
    }
}
