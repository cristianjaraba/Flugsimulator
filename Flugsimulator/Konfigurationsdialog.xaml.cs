using Db4objects.Db4o;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Flugsimulator
{
    /// <summary>
    /// Fenster zur Konfiguration eines Düsenflugzeugs.
    /// Ermöglicht Eingabe, Speicherung, Laden und Löschen von Flugdaten.
    /// </summary>
    partial class Konfigurationsdialog : Window
    {
        private Duesenflugzeug flieger;
        private IList<Duesenflugzeug> fliegerListe = new List<Duesenflugzeug>();
        private string dbName = "FliegerDB";
        internal bool isConfigurationComplete;

        // Initialisiert den Konfigurationsdialog mit einem übergebenen Flugzeug.
        public Konfigurationsdialog(Duesenflugzeug flieger)
        {
            InitializeComponent();
            this.flieger = flieger;
            Konfigurationsdialog_Load();
        }

        // Gibt das aktuell konfigurierte Flugzeug zurück.
        public Duesenflugzeug Flieger
        { get { return flieger; } }

        // Liefert alle beim Start geladenen Flugzeuge 
        public IList<Duesenflugzeug> FliegerListe
        {
            get { return fliegerListe; }
        }

        // Lädt die verfügbaren Airbus-Typen in die ComboBox.
        private void Konfigurationsdialog_Load()
        {
            foreach (Airbus element in Enum.GetValues(typeof(Airbus)))
            {
                cbTyp.Items.Add(element);
            }
            cbTyp.SelectedIndex = 0;
        }

        /// <summary>
        /// Initialisiert die Flugdaten aus den Eingabefeldern.
        /// Prüft auf gültige Eingaben und setzt isConfigurationComplete entsprechend.
        /// </summary>
        private void initializeFlug()
        {
            if (tbKennung.Text != "")
                flieger.Kennung = tbKennung.Text;
            else
            {
                Console.WriteLine("Kennung nicht gesetzt");
                isConfigurationComplete = false;
            }

            flieger.typ = (Airbus)cbTyp.SelectedItem;

            try
            {
                flieger.pos = new Position(
                    int.Parse(tbStartX.Text),
                    int.Parse(tbStartY.Text),
                    int.Parse(tbStartH.Text));
            }
            catch
            {
                Console.WriteLine("Startposition nicht gesetzt");
                isConfigurationComplete = false;
            }

            try
            {
                flieger.zielPos = new Position(
                    int.Parse(tbZielX.Text),
                    int.Parse(tbZielY.Text),
                    int.Parse(tbZielH.Text));
            }
            catch
            {
                Console.WriteLine("Zielposition nicht gesetzt");
                isConfigurationComplete = false;
            }

            try { flieger.flughoehe = int.Parse(tbFlughoehe.Text); }
            catch { Console.WriteLine("Flughöhe nicht gesetzt"); isConfigurationComplete = false; }

            try { flieger.streckeProTakt = int.Parse(tbFlugstrecke.Text); }
            catch { Console.WriteLine("Flugstrecke pro Takt nicht gesetzt"); isConfigurationComplete = false; }

            try { flieger.sinkhoeheProTakt = int.Parse(tbSinkhoehe.Text); }
            catch { Console.WriteLine("Sinkhöhe pro Takt nicht gesetzt"); isConfigurationComplete = false; }

            try { flieger.steighoeheProTakt = int.Parse(tbSteighoehe.Text); }
            catch { Console.WriteLine("Steighöhe pro Takt nicht gesetzt"); isConfigurationComplete = false; }

            try { flieger.sitzplaetze = int.Parse(tbAnzahlPlaetze.Text); }
            catch { Console.WriteLine("Anzahl Plätze nicht gesetzt"); isConfigurationComplete = false; }
        }

        
        // Startet den Flug (Dialog wird geschlossen mit positivem Ergebnis).
        private void btnStarten_Click(object sender, RoutedEventArgs e)
        {
            IObjectContainer db = null;
            try
            {
                db = Db4oFactory.OpenFile(dbName);
                IList<Duesenflugzeug> fluege = db.Query<Duesenflugzeug>();
                if (fluege != null && fluege.Count > 0)
                {
                    fliegerListe = fluege;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.GetType() + ": " + ex.Message); }
            finally
            {
                if (db != null)
                    db.Close();
            }
            DialogResult = true;
            this.Close();
        }

        
        // Speichert oder aktualisiert die Flugdaten in der Datenbank.
        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            IObjectContainer db = null;
            bool update = false;
            try
            {
                db = Db4oFactory.OpenFile(dbName);

                // Fragt alle Duesenflugzeug-Objekte ab, deren Kennung
                // mit dem Textfeld übereinstimmt
                IList<Duesenflugzeug> fluege = db.Query<Duesenflugzeug>(delegate (Duesenflugzeug flieger)
                {
                    return flieger.Kennung == tbKennung.Text;
                });

                foreach (Duesenflugzeug flieger in fluege)
                {
                    Console.WriteLine($"Flug mit der Kennung {flieger.Kennung} in Datenbank gefunden.");
                }

                if (fluege.Count > 0)
                {
                    flieger = fluege.First();
                    update = true;
                }

                this.initializeFlug();
                db.Store(flieger);

                if (update)
                    Console.WriteLine($"Datenbank-Update für den Flug mit der Kennung {flieger.Kennung}.");
                else
                    Console.WriteLine($"Flug mit der Kennung {flieger.Kennung} in der Datenbank gespeichert.");
            }
            catch (Exception ex) { Console.WriteLine(ex.GetType() + ": " + ex.Message); }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        // Lädt einen Flug aus der Datenbank über den Flugauswahldialog.
        private void btnLaden_Click(object sender, RoutedEventArgs e)
        {
            Flugauswahldialog select = new Flugauswahldialog(dbName);
            select.ShowDialog();
            if (select.DialogResult == true)
            {
                this.flieger = select.Flugauswahl;
                this.SetEingabewerte(flieger);
                Console.WriteLine("Flug \"{0}\" geladen", flieger.Kennung);
            }
            else Console.WriteLine("Flugauswahl abgebrochen");
        }

        // Setzt die Eingabefelder anhand der geladenen Flugdaten.
        private void SetEingabewerte(Duesenflugzeug flieger)
        {
            tbKennung.Text = flieger.Kennung;
            cbTyp.SelectedItem = flieger.typ;
            tbStartX.Text = flieger.pos.x.ToString();
            tbStartY.Text = flieger.pos.y.ToString();
            tbStartH.Text = flieger.pos.h.ToString();
            tbZielX.Text = flieger.zielPos.x.ToString();
            tbZielY.Text = flieger.zielPos.y.ToString();
            tbZielH.Text = flieger.zielPos.h.ToString();
            tbFlughoehe.Text = flieger.flughoehe.ToString();
            tbFlugstrecke.Text = flieger.streckeProTakt.ToString();
            tbSinkhoehe.Text = flieger.sinkhoeheProTakt.ToString();
            tbSteighoehe.Text = flieger.steighoeheProTakt.ToString();
            tbAnzahlPlaetze.Text = flieger.sitzplaetze.ToString();
        }

        
        // Löscht einen Flug aus der Datenbank anhand der Kennung.
        private void btnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            IObjectContainer db = null;
            try
            {
                db = Db4oFactory.OpenFile(dbName);
                IList<Duesenflugzeug> fluege =
                db.Query<Duesenflugzeug>(delegate (Duesenflugzeug flieger)
                {
                    return flieger.Kennung == tbKennung.Text;
                });

                if (fluege.Count > 0)
                {
                    foreach (Duesenflugzeug flieger in fluege)
                    {
                        db.Delete(flieger);
                    }
                    Console.WriteLine($"Flieger mit der Kennung {flieger.Kennung} gelöscht.");
                }
                else
                {
                    Console.WriteLine($"Kein Flug mit der Kennung {flieger.Kennung} gefunden.");
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.GetType() + ": " + ex.Message); }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }
    }
}
