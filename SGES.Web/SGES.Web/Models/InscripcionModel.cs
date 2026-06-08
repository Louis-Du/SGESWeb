using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGES.Web.Models
{
    public class InscripcionModel
    {
        public int idInscrip { get; set; }

        public int idEvento { get; set; }

        public int idApr { get; set; }

        public DateTime FechaInscrip { get; set; }

        public string Modalidad { get; set; }
    }
}