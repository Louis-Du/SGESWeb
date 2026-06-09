        using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public partial class EventoDAO
    {
        private readonly Conexion cn = new Conexion();

        public List<EventoModel> ObtenerEventos()
        {
            List<EventoModel> lista = new List<EventoModel>();

            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = "SELECT * FROM Eventos";

                SqlCommand cmd = new SqlCommand(sql, con);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new EventoModel
                    {
                        IdEvento = Convert.ToInt32(dr["idEvento"]),
                        NombreEvento = dr["nombreEvento"].ToString(),
                        TipoEvento = dr["tipoEvento"].ToString(),
                        FechaHoraInicio = Convert.ToDateTime(dr["fechaHoraInicio"]),
                        FechaHoraFin = Convert.ToDateTime(dr["fechaHoraFin"]),
                        IdUser = Convert.ToInt32(dr["idUser"])
                    });
                }
            }

            return lista;
        }

        public void InsertarEvento(EventoModel evento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string query = @"INSERT INTO Eventos (nombreEvento, tipoEvento, diaEvento, fechaHoraInicio, fechaHoraFin, idUser)
                         VALUES (@nombreEvento, @tipoEvento, @diaEvento, @fechaHoraInicio, @fechaHoraFin, @idUser)";

                SqlCommand cmd = new SqlCommand(query, con);

                // ← Se eliminó el @idEvento que no pertenecía al query
                cmd.Parameters.AddWithValue("@nombreEvento", evento.NombreEvento);
                cmd.Parameters.AddWithValue("@tipoEvento", evento.TipoEvento);
                cmd.Parameters.AddWithValue("@diaEvento", evento.FechaHoraInicio.Date);
                cmd.Parameters.AddWithValue("@fechaHoraInicio", evento.FechaHoraInicio);
                cmd.Parameters.AddWithValue("@fechaHoraFin", evento.FechaHoraFin);
                cmd.Parameters.AddWithValue("@idUser", evento.IdUser);

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
          
        // Devuelve aprendices inscritos en un evento
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

        // ActualizarEvento: modifica los datos de un evento existente en la BD.
        // Recibe un EventoModel con todos los campos ya validados desde el controlador
        public void ActualizarEvento(EventoModel evento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"UPDATE Eventos
                       SET nombreEvento    = @nombreEvento,
                           tipoEvento      = @tipoEvento,
                           diaEvento       = @diaEvento,
                           fechaHoraInicio = @fechaHoraInicio,
                           fechaHoraFin    = @fechaHoraFin
                       WHERE idEvento = @idEvento";

                SqlCommand cmd = new SqlCommand(sql, con);

                // Parámetros: cada @nombre debe coincidir exactamente con el SQL de arriba
                cmd.Parameters.AddWithValue("@nombreEvento", evento.NombreEvento);
                cmd.Parameters.AddWithValue("@tipoEvento", evento.TipoEvento);
                // diaEvento se deriva de la fecha de inicio (solo la parte de fecha)
                cmd.Parameters.AddWithValue("@diaEvento", evento.FechaHoraInicio.Date);
                cmd.Parameters.AddWithValue("@fechaHoraInicio", evento.FechaHoraInicio);
                cmd.Parameters.AddWithValue("@fechaHoraFin", evento.FechaHoraFin);
                cmd.Parameters.AddWithValue("@idEvento", evento.IdEvento);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}