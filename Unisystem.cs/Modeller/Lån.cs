using System;
using System.Collections.Generic;
using System.Text;




namespace Unisystem.cs.Modeller
{
    public class Lån
    {
        public int LånId { get; set; }
        public Bok Bok { get; set; }
        public object Låntaker { get; set; } // Student eller ansatt
        public DateTime Lånedato { get; set; }
        public DateTime? ReturnertDato { get; set; } // Må være nullable!

        public bool Aktiv => ReturnertDato == null;
    }
}

