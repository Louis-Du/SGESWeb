using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SGES.Web.Models
{
    public class EventoDAO
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
    }
}