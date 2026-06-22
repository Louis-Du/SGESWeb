using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SGES.Web.Models
{
    // InscripcionDAO se encarga de todas las operaciones de base de datos
    // relacionadas con inscripciones: validar y guardar.
    public class InscripcionDAO
    {
        // Reutilizamos la clase Conexion del proyecto para abrir la BD.
        private readonly Conexion cn = new Conexion();

        // ─────────────────────────────────────────────────────────────
        // VALIDACIÓN 1: ¿El aprendiz ya está inscrito en este evento?
        // Retorna true si ya existe una inscripción con ese par (idApr, idEvento).
        // ─────────────────────────────────────────────────────────────
        public bool YaInscrito(int idApr, int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                // COUNT(*) devuelve cuántas filas coinciden.
                // Si es mayor a 0, ya está inscrito.
                string sql = @"SELECT COUNT(*) FROM Inscripciones
                               WHERE idApr = @idApr AND idEvento = @idEvento";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idApr", idApr);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);

                con.Open();
                int cantidad = (int)cmd.ExecuteScalar();
                return cantidad > 0;
            }
        }

        // ─────────────────────────────────────────────────────────────
        // VALIDACIÓN 2: ¿Las fechas del evento nuevo se cruzan con
        // algún evento en el que el aprendiz ya está inscrito?
        //
        // Dos eventos se cruzan si uno empieza antes de que el otro termine.
        // Fórmula: inicio_nuevo < fin_existente  Y  fin_nuevo > inicio_existente
        // ─────────────────────────────────────────────────────────────
        public bool TieneCruceDeHorario(int idApr, int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"
                    SELECT COUNT(*)
                    FROM Inscripciones     i
                    -- Unimos con Eventos para obtener las fechas del evento nuevo
                    JOIN Eventos           nuevo    ON nuevo.idEvento    = @idEvento
                    -- Unimos con Eventos para obtener las fechas de los eventos existentes
                    JOIN Eventos           existente ON existente.idEvento = i.idEvento
                    WHERE i.idApr = @idApr
                      -- Excluimos el mismo evento (por si acaso)
                      AND i.idEvento <> @idEvento
                      -- Condición de cruce de horario (algoritmo de solapamiento de intervalos)
                      AND nuevo.fechaHoraInicio    < existente.fechaHoraFin
                      AND nuevo.fechaHoraFin        > existente.fechaHoraInicio";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idApr", idApr);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);

                con.Open();
                int cantidad = (int)cmd.ExecuteScalar();
                return cantidad > 0;
            }
        }

        public int ContarInscritos(int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = "SELECT COUNT(*) FROM Inscripciones WHERE idEvento = @idEvento";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // ─────────────────────────────────────────────────────────────
        // GUARDAR: Inserta la inscripción en la tabla Inscripciones.
        // Solo se llama después de pasar las dos validaciones anteriores.
        // idGrupo se deja en NULL porque la asignación de grupos es tarea del admin.
        // ─────────────────────────────────────────────────────────────
        public void Inscribir(InscripcionModel inscripcion)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = @"INSERT INTO Inscripciones (fechaInscrip, idApr, idEvento, idGrupo)
               VALUES (@fechaInscrip, @idApr, @idEvento, NULL)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@fechaInscrip", inscripcion.FechaInscrip);    
                cmd.Parameters.AddWithValue("@idApr", inscripcion.IdApr);
                cmd.Parameters.AddWithValue("@idEvento", inscripcion.IdEvento);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int VerificarInscritos(int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = "SELECT COUNT(1) FROM Inscripciones WHERE idEvento = @idEvento";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void EliminarInscritos(int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                string sql = "DELETE FROM Inscripciones WHERE idEvento = @idEvento";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idEvento", idEvento);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void InscribirGrupo(List<int> idAprendices, int idEvento, DateTime fechaInscrip)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    // Evitar insertar duplicados si la lista enviada tiene repetidos
                    idAprendices = idAprendices.Distinct().ToList();
                    // 1. Crear el grupo
                    string sqlGrupo = @"INSERT INTO Grupos (nombreGrupo, descripcion, idEvento)
                                VALUES (@nombre, @desc, @idEvento);
                                SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdGrupo = new SqlCommand(sqlGrupo, con, tx);
                    cmdGrupo.Parameters.AddWithValue("@nombre", "Grupo " + DateTime.Now.ToString("yyyyMMddHHmm"));
                    cmdGrupo.Parameters.AddWithValue("@desc", "Grupo inscrito en evento " + idEvento);
                    cmdGrupo.Parameters.AddWithValue("@idEvento", idEvento);
                    int idGrupo = Convert.ToInt32(cmdGrupo.ExecuteScalar());

                    // 2. Inscribir a cada aprendiz con ese grupo
                    string sqlInsc = @"INSERT INTO Inscripciones (fechaInscrip, idApr, idEvento, idGrupo)
                               VALUES (@fecha, @idApr, @idEvento, @idGrupo)";

                    foreach (int idApr in idAprendices)
                    {
                        // Si el aprendiz ya está inscrito, saltarlo (protección adicional)
                        if (YaInscrito(idApr, idEvento))
                            continue;

                        SqlCommand cmdInsc = new SqlCommand(sqlInsc, con, tx);
                        cmdInsc.Parameters.AddWithValue("@fecha", fechaInscrip);
                        cmdInsc.Parameters.AddWithValue("@idApr", idApr);
                        cmdInsc.Parameters.AddWithValue("@idEvento", idEvento);
                        cmdInsc.Parameters.AddWithValue("@idGrupo", idGrupo);
                        cmdInsc.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public void EliminarGruposDeEvento(int idEvento)
        {
            using (SqlConnection con = cn.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Grupos WHERE idEvento = @id", con);
                cmd.Parameters.AddWithValue("@id", idEvento);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}