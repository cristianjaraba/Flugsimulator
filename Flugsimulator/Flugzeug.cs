namespace Flugsimulator
{
    // Abgeleitete Klasse von Luftfahrzeug mit zusätzlichen Flugeigenschaften
    public class Flugzeug : Luftfahrzeug
    {
        // Parameter für Flugverhalten
        protected internal int flughoehe;
        protected internal int steighoeheProTakt;
        protected internal int sinkhoeheProTakt;
        protected internal int streckeProTakt;
        protected internal Position zielPos;

        // Statusflags für Steig- und Sinkflug
        protected bool steigt = false;
        protected bool sinkt = false;

        public Flugzeug() { }

        // Implementierung der abstrakten Methode: Flugzeug steigt um 'meter'
        public override void Steigen(int meter)
        {
            this.pos.PositionÄndern(0, 0, meter);
        }

        // Implementierung der abstrakten Methode: Flugzeug sinkt um 'meter'
        public override void Sinken(int meter)
        {
            this.pos.PositionÄndern(0, 0, -meter);
        }
    }
}
