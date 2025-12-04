using System;
using System.IO;

namespace Flugsimulator
{
    /// <summary>
    /// Repräsentiert ein Starrflügelflugzeug im Flugsimulator.
    /// Implementiert die Transponder-Funktionalität und steuert den Flugverlauf
    /// (Steigflug, Sinkflug, Horizontalflug, Landung).
    /// </summary>
    public class Starrfluegelflugzeug : Flugzeug, ITransponder
    {
        private double a, b, alpha, a1, b1;
        private bool gelandet = false;
        protected BinaryWriter writer;

        protected string protokollpfad;

        /// <summary>
        /// Pfad zur Protokolldatei des Flugschreibers.
        /// </summary>
        public string Protokollpfad
        {
            get { return protokollpfad; }
        }

        /// <summary>
        /// Transponder-Methode: gibt die aktuelle Position des Flugzeugs aus
        /// oder meldet Abstand zu anderen Flugzeugen.
        /// </summary>
        /// <param name="kennung">Kennung des Flugzeugs</param>
        /// <param name="pos">Aktuelle Position</param>
        public void Transpond(string kennung, Position pos)
        {
            DateTime timestamp = DateTime.Now;
            if (kennung.Equals(this.kennung))
            {
                Console.Write("{0:D2}:{1:D2}:{2:D2} ", timestamp.Hour, timestamp.Minute, timestamp.Second);
                Console.Write("\t{0}: Position={1}/{2}/{3}",
                    this.kennung, pos.x, pos.y, pos.h);
                Console.Write(", Zieldistanz={0} m\n", Zieldistanz());
                if (Fliegerprojekt.protokollieren && writer != null)
                {
                    writer.Write(pos.x);
                    writer.Write(pos.y);
                    writer.Write(pos.h);
                }
            }
            else
            {
                double abstand = Math.Sqrt(Math.Pow(this.pos.x - pos.x, 2)
                    + Math.Pow(this.pos.y - pos.y, 2));
                Console.Write("\t{0}: {1} ist {2} m entfernt.\n",
                    this.kennung, kennung, (int)abstand);

                if (Math.Abs(this.pos.h - pos.h) < 100 && abstand < 500)
                    Console.WriteLine("\t{0}-WARNUNG: {1} hat nur {2} m Höhenabstand!",
                        this.kennung, kennung, Math.Abs(this.pos.h - pos.h));
            }
        }

        /// <summary>
        /// Prüft, ob der Sinkflug eingeleitet werden soll.
        /// </summary>
        /// <returns>True, wenn Sinkflug eingeleitet werden kann.</returns>
        private bool SinkenEinleiten()
        {
            double strecke = Math.Sqrt(Math.Pow(streckeProTakt, 2)
                             - Math.Pow(sinkhoeheProTakt, 2));
            int sinkstrecke = (int)(strecke * (pos.h - zielPos.h) / sinkhoeheProTakt);
            int zieldistanz = Zieldistanz();
            if (sinkstrecke >= zieldistanz)
            {
                Console.WriteLine("{0}-Sinkstrecke {1} >= Zieldistanz {2}",
                    kennung, sinkstrecke, zieldistanz);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Berechnet die neue Position des Flugzeugs anhand Strecke und Steighöhe.
        /// </summary>
        private void PositionBerechnen(double strecke, int steighoeheProTakt)
        {
            a = zielPos.x - pos.x;
            b = zielPos.y - pos.y;
            alpha = Math.Atan2(b, a);
            a1 = Math.Cos(alpha) * strecke;
            b1 = Math.Sin(alpha) * strecke;
            pos.PositionÄndern((int)a1, (int)b1, steighoeheProTakt);
        }

        /// <summary>
        /// Berechnet die Distanz zum Zielpunkt.
        /// </summary>
        private int Zieldistanz()
        {
            return (int)Math.Sqrt(Math.Pow(zielPos.x - pos.x, 2) + Math.Pow(zielPos.y - pos.y, 2));
        }

        /// <summary>
        /// Steuert den Flugverlauf: Steigen, Sinken, Horizontalflug und Landung.
        /// </summary>
        public void Steuern()
        {
            if (steigt)
            {
                if (this.SinkenEinleiten())
                {
                    steigt = false;
                    sinkt = true;
                }
                else if (pos.h > flughoehe)
                {
                    steigt = false;
                }
            }
            else if (sinkt)
            {
                if (pos.h <= zielPos.h + sinkhoeheProTakt)
                    gelandet = true;
            }
            else
            {
                if (this.SinkenEinleiten())
                {
                    sinkt = true;
                }
            }
            if (!gelandet)
            {
                Fliegerprojekt.transponder(kennung, pos);
                if (steigt)
                {
                    double strecke = Math.Sqrt(Math.Pow(streckeProTakt, 2)
                                     - Math.Pow(steighoeheProTakt, 2));
                    this.PositionBerechnen(strecke, steighoeheProTakt);
                }
                else if (sinkt)
                {
                    double strecke = Math.Sqrt(Math.Pow(streckeProTakt, 2)
                                     - Math.Pow(sinkhoeheProTakt, 2));
                    this.PositionBerechnen(strecke, -sinkhoeheProTakt);
                }
                else
                {
                    this.PositionBerechnen(streckeProTakt, 0);
                }
            }
            else
            {
                Fliegerprojekt.fliegerRegister -= this.Steuern;
                Fliegerprojekt.transponder -= this.Transpond;
                Console.WriteLine("\n{0} gelandet (Zieldistanz={1}, Höhendistanz={2})",
                    kennung, Zieldistanz(), pos.h - zielPos.h);
                if (Fliegerprojekt.protokollieren && writer != null)
                {
                    writer.Write(pos.x);
                    writer.Write(pos.y);
                    writer.Write(pos.h);
                    writer.Close();
                }
            }
        }
    }
}
