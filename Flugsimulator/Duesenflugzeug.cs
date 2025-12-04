using System;
using System.IO;

namespace Flugsimulator
{
    /// <summary>
    /// Repräsentiert ein Düsenflugzeug im Flugsimulator.
    /// Erbt von <see cref="Starrfluegelflugzeug"/> und erweitert es um Typ, Sitzplätze und Buchungslogik.
    /// </summary>
    public class Duesenflugzeug : Starrfluegelflugzeug
    {
        protected internal Airbus typ;

        protected internal int sitzplaetze;

        private int fluggaeste;

        /// <summary>
        /// Anzahl der gebuchten Fluggäste.
        /// Beim Setzen wird geprüft, ob die Kapazität überschritten wird.
        /// </summary>
        public int Fluggaeste
        {
            set
            {
                if (sitzplaetze < (fluggaeste + value))
                    Console.WriteLine("Keine Buchung: Die Fluggastzahl würde "
                        + "mit der Zubuchung von {0} Plätzen die verfügbaren "
                        + "Plätze von {1} um {2} übersteigen!", value, sitzplaetze,
                        value + fluggaeste - sitzplaetze);
                else
                    fluggaeste += value;
            }
            get { return fluggaeste; }
        }

        public Duesenflugzeug() { }

        /// <summary>
        /// Startet das Flugzeug, aktiviert Steigflug und registriert Transponder sowie Steuerlogik.
        /// </summary>
        public void Starte()
        {
            Console.WriteLine("Flieger \"{0}\", Typ {1} ({2} Sitzplätze) startet", Kennung, typ, sitzplaetze);
            steigt = true;
            Fliegerprojekt.transponder += this.Transpond;
            Fliegerprojekt.fliegerRegister += this.Steuern;
        }

        public void Buchen(int plätze)
        {
            Fluggaeste += plätze;
        }

        /// <summary>
        /// Initialisiert den Flugschreiber (Protokollierung in Binärdatei).
        /// Erstellt eine Datei mit Zeitstempel und schreibt Headerinformationen.
        /// </summary>
        public void FlugschreiberInitialisieren()
        {
            if (Fliegerprojekt.protokollieren)
            {
                DateTime timestamp = DateTime.Now;
                string pfad = kennung + "_"
                    + timestamp.Day + "-" + timestamp.Hour + "-"
                    + timestamp.Minute + "-" + timestamp.Second + ".bin";
                protokollpfad = pfad;

                writer = new BinaryWriter(File.Open(pfad, FileMode.Create));
                string header =
                    "Flug \"" + kennung + "\" (Typ " + this.typ
                    + ") startet an Position " + pos.x + "-" + pos.y
                    + "-" + pos.h + " mit Zielposition " + zielPos.x + "-"
                    + zielPos.y + "-" + zielPos.h;
                writer.Write(header);
            }
        }
    }
}
