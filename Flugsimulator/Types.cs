namespace Flugsimulator
{
    // Delegate für Transpondermeldungen: überträgt Kennung und aktuelle Position eines Flugzeugs
    delegate void TransponderDel(string kennung, Position pos);

    // Delegate für die Registrierung von Flugzeugen im Projekt 
    delegate void FliegerregisterDel();

    public enum Airbus { A300, A310, A318, A319, A320, A321, A330, A340, A350, A380 }

    public struct Position
    {
        public int x, y, h;
        public Position(int x, int y, int h)
        {
            this.x = x;
            this.y = y;
            this.h = h;
        }
        public void PositionÄndern(int deltaX, int deltaY, int deltaH)
        {
            x += deltaX;
            y += deltaY;
            h += deltaH;
        }
    }

    // Interface für Transponder-Funktionalität:
    // Jedes Flugzeug mit Transponder muss die Methode Transpond implementieren
    interface ITransponder 
    {
        void Transpond(string kennung, Position pos);
    }
}
