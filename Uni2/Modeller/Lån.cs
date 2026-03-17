namespace Unisystem.cs.Modeller
{
    public class Lån
    {
        public Bok Bok { get; set; }
        public object Låntaker { get; set; } // Student eller ansatt
        public DateTime Lånedato { get; set; }
        public DateTime? ReturnertDato { get; set; } // Null = aktivt lån

        public bool Aktiv => ReturnertDato == null;

        public Lån(Bok bok, object låntaker)
        {
            Bok = bok;
            Låntaker = låntaker;
            Lånedato = DateTime.Now;
            ReturnertDato = null;
        }

        public void Returner()
        {
            ReturnertDato = DateTime.Now;
            Bok.Antall++;
        }
    }
}