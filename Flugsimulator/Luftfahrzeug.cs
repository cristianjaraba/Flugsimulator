namespace Flugsimulator
{
    // Abstrakte Basisklasse für alle Luftfahrzeuge
    public abstract class Luftfahrzeug
    {
        protected string kennung;
        protected internal Position pos;
        public string Kennung
        {
            get { return kennung; }
            set { kennung = value; }
        }
        public Luftfahrzeug() { }

        // Muss von abgeleiteten Klassen implementiert werden
        public abstract void Steigen(int meter);
        public abstract void Sinken(int meter);
        public override string ToString()
        {
            return this.Kennung;
        }
    }
}
