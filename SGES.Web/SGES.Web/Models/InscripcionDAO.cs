using System;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class InscripcionDAO
    {

        private readonly Conexion cn = new Conexion();



        public bool YaInscrito(
            int idApr,
            int idEvento)
        {

            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                SELECT COUNT(*)
                FROM Inscripciones
                WHERE idApr=@idApr
                AND idEvento=@idEvento";


                SqlCommand cmd =
                    new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@idApr", idApr);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);


                con.Open();


                return (int)cmd.ExecuteScalar() > 0;
            }
        }




        public bool TieneCruceDeHorario(
            int idApr,
            int idEvento)
        {

            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"

                SELECT COUNT(*)

                FROM Inscripciones I

                INNER JOIN Eventos Nuevo
                ON Nuevo.idEvento=@idEvento


                INNER JOIN Eventos Actual
                ON Actual.idEvento=I.idEvento


                WHERE I.idApr=@idApr

                AND I.idEvento<>@idEvento

                AND Nuevo.fechaHoraInicio < Actual.fechaHoraFin

                AND Nuevo.fechaHoraFin > Actual.fechaHoraInicio";


                SqlCommand cmd =
                    new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@idApr", idApr);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);


                con.Open();


                return (int)cmd.ExecuteScalar() > 0;

            }

        }




        public int ContarInscritos(int idEvento)
        {

            using(SqlConnection con = cn.ObtenerConexion())
            {

                SqlCommand cmd =
                new SqlCommand(
                "SELECT COUNT(*) FROM Inscripciones WHERE idEvento=@id",
                con);


                cmd.Parameters.AddWithValue("@id", idEvento);


                con.Open();


                return (int)cmd.ExecuteScalar();

            }
        }





        public void Inscribir(
            InscripcionModel inscripcion)
        {

            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"

                INSERT INTO Inscripciones
                (
                    fechaInscrip,
                    idApr,
                    idEvento,
                    idGrupo
                )

                VALUES
                (
                    @fecha,
                    @apr,
                    @evento,
                    NULL
                )";



                SqlCommand cmd =
                    new SqlCommand(sql, con);



                cmd.Parameters.AddWithValue(
                    "@fecha",
                    inscripcion.FechaInscrip);


                cmd.Parameters.AddWithValue(
                    "@apr",
                    inscripcion.IdApr);


                cmd.Parameters.AddWithValue(
                    "@evento",
                    inscripcion.IdEvento);



                con.Open();

                cmd.ExecuteNonQuery();

            }

        }





        public int VerificarInscritos(int idEvento)
        {
            return ContarInscritos(idEvento);
        }




        public void EliminarInscritos(int idEvento)
        {

            using(SqlConnection con = cn.ObtenerConexion())
            {

                SqlCommand cmd =
                new SqlCommand(
                "DELETE FROM Inscripciones WHERE idEvento=@id",
                con);


                cmd.Parameters.AddWithValue("@id", idEvento);


                con.Open();

                cmd.ExecuteNonQuery();

            }

        }

        public int? ObtenerOCrearGrupo(int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                // Buscar grupo existente para este evento
                string sqlBuscar = "SELECT TOP 1 idGrupo FROM Grupos WHERE idEvento = @idEvento";
                SqlCommand cmdBuscar = new SqlCommand(sqlBuscar, con);
                cmdBuscar.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();
                var resultado = cmdBuscar.ExecuteScalar();
                if (resultado != null)
                    return Convert.ToInt32(resultado);
            }

            // Si no existe, crear uno nuevo
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sqlCrear = @"INSERT INTO Grupos (nombreGrupo, idEvento)
                            OUTPUT INSERTED.idGrupo
                            VALUES (@nombre, @idEvento)";
                SqlCommand cmdCrear = new SqlCommand(sqlCrear, con);
                cmdCrear.Parameters.AddWithValue("@nombre", "Grupo-" + idEvento);
                cmdCrear.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();
                return Convert.ToInt32(cmdCrear.ExecuteScalar());
            }
        }

        public void Inscribir(InscripcionModel inscripcion, bool esGrupal)
        {
            int? idGrupo = null;

            if (esGrupal)
                idGrupo = ObtenerOCrearGrupo(inscripcion.IdEvento);

            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"INSERT INTO Inscripciones
                       (fechaInscrip, idApr, idEvento, idGrupo)
                       VALUES (@fecha, @apr, @evento, @grupo)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@fecha", inscripcion.FechaInscrip);
                cmd.Parameters.AddWithValue("@apr", inscripcion.IdApr);
                cmd.Parameters.AddWithValue("@evento", inscripcion.IdEvento);
                cmd.Parameters.AddWithValue("@grupo",
                    idGrupo.HasValue ? (object)idGrupo.Value : DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}