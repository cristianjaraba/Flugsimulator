using Db4objects.Db4o;
using System;
using System.Windows;

namespace Flugsimulator
{
    // Fenster zur Auswahl eines gespeicherten Düsenflugzeugs aus der Datenbank.
    partial class Flugauswahldialog : Window
    {
        private string dbName;
        private Duesenflugzeug flugauswahl;

        // Gibt das aktuell ausgewählte Flugzeug aus der ComboBox zurück.
        public Duesenflugzeug Flugauswahl
        {
            get { return (Duesenflugzeug)cmbFlugAuswahl.SelectedItem; }
        }

        /// <summary>
        /// Initialisiert den Flugauswahldialog mit dem angegebenen Datenbanknamen.
        /// </summary>
        /// <param name="dbName">Name der Db4o-Datenbankdatei.</param>
        public Flugauswahldialog(string dbName)
        {
            InitializeComponent();
            this.dbName = dbName;
            Flugauswahldialog_Load();
        }

        /// <summary>
        /// Lädt alle gespeicherten Düsenflugzeuge aus der Datenbank
        /// und füllt die ComboBox mit den Ergebnissen.
        /// </summary>
        private void Flugauswahldialog_Load()
        {
            IObjectContainer db = null;
            try
            {
                db = Db4oFactory.OpenFile(dbName);
                IObjectSet result = db.QueryByExample(typeof(Duesenflugzeug));
                foreach (Duesenflugzeug duesenflugzeug in result)
                {
                    cmbFlugAuswahl.Items.Add(duesenflugzeug);
                }
                cmbFlugAuswahl.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType() + ": " + ex.Message);
            }
            finally
            {
                if (db != null)
                    db.Close();
            }
        }

        // Bestätigt die Auswahl und schließt den Dialog mit positivem Ergebnis.
        private void btnÜbernehmen_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
