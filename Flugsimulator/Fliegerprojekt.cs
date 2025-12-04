using Db4objects.Db4o;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Flugsimulator
{
    /// <summary>
    /// Hauptprojektklasse für den Flugsimulator.
    /// Steuert den Ablauf des Programms und verwaltet Transponder- sowie Flugzeugregistrierung.
    /// </summary>
    class Fliegerprojekt
    {
        /// <summary>
        /// Delegate für Transpondermeldungen (Kennung + Position).
        /// </summary>
        public static TransponderDel transponder;

        /// <summary>
        /// Delegate für die Registrierung von Flugzeugen im Steuerungsloop.
        /// </summary>
        public static FliegerregisterDel fliegerRegister;

        /// <summary>
        /// Gibt an, ob Flüge protokolliert werden sollen.
        /// </summary>
        public static bool protokollieren = true;

        /// <summary>
        /// Startet den Konfigurationsdialog, initialisiert das Flugzeug
        /// und führt den Steuerungsloop (Takten) aus.
        /// </summary>
        public void ProgrammTakten()
        {
            Duesenflugzeug flieger = new Duesenflugzeug();
            Konfigurationsdialog config = new Konfigurationsdialog(flieger);
            config.Height = 450;
            config.Width = 800;

            if (config.ShowDialog() == true)
            {
                IObjectContainer db = null;
                string dbName = "FliegerDB";
                try
                {
                    // DB wird hier geöffnet und offen halten, bis der Steuerungsloop beendet ist.
                    db = Db4oFactory.OpenFile(dbName);
                    IList<Duesenflugzeug> fluege = db.Query<Duesenflugzeug>();

                    if (fluege != null && fluege.Count > 0)
                    {
                        foreach (var f in fluege)
                        {
                            f.Starte();
                        }
                    }
                    else
                    {
                        flieger = config.Flieger;
                        if (flieger != null)
                            flieger.Starte();
                    }

                    // Steuerungsloop: solange Flugzeuge registriert sind, wird getaktet
                    while (fliegerRegister != null)
                    {
                        fliegerRegister();
                        Console.WriteLine();
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.GetType() + ": " + ex.Message); }
                finally
                {
                    if (db != null)
                        db.Close();
                }
            }
            else
            {
                Console.WriteLine(Environment.NewLine
                                + "Konfiguration abgebrochen oder nicht "
                                + "vollständig, kein Start");
            }
        }

        /// <summary>
        /// Liest und gibt die Inhalte einer Flugprotokolldatei aus.
        /// </summary>
        /// <param name="protokollpfad">Pfad zur Binärdatei mit Flugprotokoll.</param>
        public void AusgabeProtokoll(string protokollpfad)
        {
            BinaryReader reader = new BinaryReader(File.Open(protokollpfad, FileMode.Open));
            Console.WriteLine(reader.ReadString());
            bool goOn = true;
            while (goOn)
            {
                try
                {
                    for (int i = 0; i < 3; i++)
                        Console.Write("\t{0}", reader.ReadInt32());
                }
                catch
                {
                    goOn = false;
                }
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Einstiegspunkt des Programms.
    /// Initialisiert ein Fliegerprojekt und startet den Steuerungsprozess.
    /// </summary>
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Fliegerprojekt program = new Fliegerprojekt();
            program.ProgrammTakten();
        }
    }
}
