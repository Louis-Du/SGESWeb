using System;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class AuthDAO
    {
        private readonly Conexion cn = new Conexion();


        // =====================================================
        // LOGIN
        // Primero busca administrador
        // Si no existe busca aprendiz
        // =====================================================

        public UsuarioSesion Login(int id, string contrasena)
        {

            // -----------------------------
            // 1. Buscar Administrador
            // -----------------------------
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
                    SELECT idAdmin, nombreAdmin
                    FROM Administrador
                    WHERE idAdmin = @id
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
                        Id = Convert.ToInt32(dr["idAdmin"]),
                        Nombre = dr["nombreAdmin"].ToString(),
                        Tipo = "Administrador"
                    };
                }
            }



            // -----------------------------
            // 2. Buscar Aprendiz
            // -----------------------------
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
                    SELECT idApr, nombreApr
                    FROM Aprendiz
                    WHERE idApr = @id
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
                        Id = Convert.ToInt32(dr["idApr"]),
                        Nombre = dr["nombreApr"].ToString(),
                        Tipo = "Aprendiz"
                    };
                }

            }


            return null;
        }





        // =====================================================
        // Obtener usuario para restablecer contraseña
        // =====================================================

        public UsuarioSesion ObtenerUsuarioPorId(int id)
        {

            // Administrador
            using (SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                    SELECT idAdmin, nombreAdmin
                    FROM Administrador
                    WHERE idAdmin = @id";


                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@id", id);


                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();


                if(dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id = Convert.ToInt32(dr["idAdmin"]),
                        Nombre = dr["nombreAdmin"].ToString(),
                        Tipo = "Administrador"
                    };
                }
            }



            // Aprendiz
            using (SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                    SELECT idApr, nombreApr
                    FROM Aprendiz
                    WHERE idApr = @id";


                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@id", id);


                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();


                if(dr.Read())
                {
                    return new UsuarioSesion
                    {
                        Id = Convert.ToInt32(dr["idApr"]),
                        Nombre = dr["nombreApr"].ToString(),
                        Tipo = "Aprendiz"
                    };
                }
            }


            return null;
        }





        // =====================================================
        // Cambiar contraseña
        // =====================================================

        public void ActualizarPassword(
            int id,
            string password,
            string tipo)
        {

            string tabla;


            if(tipo == "Administrador")
                tabla = "Administrador";
            else
                tabla = "Aprendiz";



            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = $@"
                    UPDATE {tabla}
                    SET passwordHash = @password
                    WHERE {(tipo=="Administrador" ? "idAdmin" : "idApr")} = @id";



                SqlCommand cmd = new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@id", id);


                con.Open();

                cmd.ExecuteNonQuery();

            }

        }

    }
}