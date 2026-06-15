public class EventoModel
{
    public int IdEvento { get; set; }

    public string NombreEvento { get; set; }

    public string TipoEvento { get; set; }

    public string ModalidadEvento { get; set; }

    public string TipoInscrip { get; set; }

    public int CupoMaximo { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public DateTime FechaHoraFin { get; set; }


    // administrador creador
    public int IdAdmin { get; set; }
}