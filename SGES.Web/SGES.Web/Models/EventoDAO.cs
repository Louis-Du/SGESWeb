using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public partial class EventoDAO
    {
        private readonly Conexion cn = new Conexion();

        public List<EventoModel> ObtenerEventos()
        {
            List<EventoModel> lista = new List<EventoModel>();

            using(SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"SELECT idEvento, nombreEvento, tipoEvento, modalidadEvento, tipoInscrip, cupoMaximo, fechaHoraInicio, fechaHoraFin, idAdmin
                               FROM Eventos";

                SqlCommand cmd = new SqlCommand(sql, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while(dr.Read())
                {
                    lista.Add(new EventoModel
                    {
                        IdEvento = Convert.ToInt32(dr["idEvento"]),

                        NombreEvento = dr["nombreEvento"].ToString(),

                        TipoEvento = dr["tipoEvento"].ToString(),

                        ModalidadEvento = dr["modalidadEvento"].ToString(),

                        TipoInscrip = dr["tipoInscrip"].ToString(),

                        CupoMaximo = Convert.ToInt32(dr["cupoMaximo"]),

                        FechaHoraInicio = Convert.ToDateTime(dr["fechaHoraInicio"]),

                        FechaHoraFin = Convert.ToDateTime(dr["fechaHoraFin"]),

                        IdAdmin = Convert.ToInt32(dr["idAdmin"])
                    });
                }
            }

            return lista;
        }

        public void InsertarEvento(EventoModel evento)
        {
            using(SqlConnection con = cn.ObtenerConexion())
            {

                string sql = @"
                INSERT INTO Eventos
                (nombreEvento, tipoEvento, modalidadEvento, tipoInscrip, cupoMaximo, fechaHoraInicio, fechaHoraFin, idAdmin)
                VALUES
                (@nombre, @tipo, @modalidad, @inscrip, @cupo, @inicio, @fin, @admin)";


                SqlCommand cmd = new SqlCommand(sql, con);


                cmd.Parameters.AddWithValue("@nombre", evento.NombreEvento);

                cmd.Parameters.AddWithValue("@tipo", evento.TipoEvento);

                cmd.Parameters.AddWithValue("@modalidad", evento.ModalidadEvento);

                cmd.Parameters.AddWithValue("@inscrip", evento.TipoInscrip);

                cmd.Parameters.AddWithValue("@cupo", evento.CupoMaximo);

                cmd.Parameters.AddWithValue("@inicio", evento.FechaHoraInicio);

                cmd.Parameters.AddWithValue("@fin", evento.FechaHoraFin);

                cmd.Parameters.AddWithValue("@admin", evento.IdAdmin);


                con.Open();

                cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarEvento(EventoModel evento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"UPDATE Eventos
                             SET nombreEvento    = @nombreEvento,
                             tipoEvento      = @tipoEvento,
                             modalidadEvento = @modalidadEvento,
                             tipoInscrip     = @tipoInscrip,
                             cupoMaximo = @cupoMaximo,
                             fechaHoraInicio = @fechaHoraInicio,
                             fechaHoraFin    = @fechaHoraFin
                             WHERE idEvento = @idEvento";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nombreEvento", evento.NombreEvento);
                cmd.Parameters.AddWithValue("@tipoEvento", evento.TipoEvento);
                cmd.Parameters.AddWithValue("@modalidadEvento", evento.ModalidadEvento);  // NUEVO
                cmd.Parameters.AddWithValue("@tipoInscrip", evento.TipoInscrip);       // NUEVO
                cmd.Parameters.AddWithValue("@cupoMaximo", evento.CupoMaximo);
                cmd.Parameters.AddWithValue("@fechaHoraInicio", evento.FechaHoraInicio);
                cmd.Parameters.AddWithValue("@fechaHoraFin", evento.FechaHoraFin);
                cmd.Parameters.AddWithValue("@idEvento", evento.IdEvento);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarEventos(int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = "DELETE FROM Eventos WHERE idEvento = @idEvento";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<AprendizModel> ObtenerAprendicesPorEvento(int idEvento)
        {
            var lista = new List<AprendizModel>();

            using (SqlConnection con = cn.ObtenerConexion())
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT A.idApr, A.nombreApr, A.emailApr, A.contactoApr
                    FROM Aprendiz A
                    INNER JOIN Inscripciones I ON A.idApr = I.idApr
                    WHERE I.idEvento = @idEvento";

                cmd.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new AprendizModel
                        {
                            IdApr = rdr["idApr"] != DBNull.Value ? Convert.ToInt32(rdr["idApr"]) : 0,
                            NombreApr = rdr["nombreApr"] != DBNull.Value ? rdr["nombreApr"].ToString() : "",
                            EmailApr = rdr["emailApr"] != DBNull.Value ? rdr["emailApr"].ToString() : "",
                            ContactoApr = rdr["contactoApr"] != DBNull.Value ? rdr["contactoApr"].ToString() : ""
                        });
                    }
                }
            }

            return lista;
        }
    }
}
