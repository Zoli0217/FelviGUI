using Microsoft.Data.Sqlite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FelviGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string ConnectionString = "Data Source = felvi.db";
        private SqliteConnection connection;

        List<diakok> diakokList = [];
        public MainWindow()
        {
            InitializeComponent();
            connection = new SqliteConnection(ConnectionString);
            connection.Open();

            ReadFromDatabase();
        }

        public void ReadFromDatabase()
        {
            string queryText = "SELECT * FROM diakok";
            SqliteCommand command = new SqliteCommand(queryText, connection);
            SqliteDataReader reader = command.ExecuteReader();

            diakokList = [];
            while (reader.Read())
            {
                string oktazon = reader.GetString(0);
                string nev = reader.GetString(1);
                int hozott = reader.GetInt32(2);
                int kpmagy = reader.GetInt32(3);
                int kpmat = reader.GetInt32(4);
                diakokList.Add(new diakok(oktazon, nev, hozott, kpmagy, kpmat));
            }
            reader.Close();
            diakokDg.ItemsSource = diakokList;
            diakokDg.SelectedIndex = 0;

        }

        private void diakokDg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            diakok selectedItem = (diakok)diakokDg.SelectedItem;

            string query = """
                SELECT hely, agazat FROM tagozatok INNER JOIN jelentkezesek ON tagozatok.akod=jelentkezesek.tag 
                INNER JOIN diakok ON jelentkezesek.diak = diakok.oktazon 
                WHERE diakok.oktazon = @oktazon
                """;

            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@oktazon", selectedItem.Oktazon);

            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string hely = reader.GetString(0);
                string agazat = reader.GetString(1);
                jelentkezettLb.Content = $"Jelentkezett {hely}. helyen: {agazat}";
            }
            reader.Close();
        }

        private void jelentkezokBtn_Click(object sender, RoutedEventArgs e)
        {
            string query = """
                SELECT agazat, COUNT(*) AS jelentkezokSzama FROM tagozatok INNER JOIN jelentkezesek ON tagozatok.akod=jelentkezesek.tag
                GROUP BY agazat
                """;
            SqliteCommand command = new SqliteCommand(query, connection);
            SqliteDataReader reader = command.ExecuteReader();

            List<string> jelentkezok = [];

            while (reader.Read())
            {
                string agazat = reader.GetString(0);
                int jelentkezokSzama = reader.GetInt32(1);
                jelentkezok.Add($"{agazat}: {jelentkezokSzama} fő");
            }
            jelentkezokLb.ItemsSource = jelentkezok;
        }

        private void NyekBtn_Click(object sender, RoutedEventArgs e)
        {
            string query = """
                SELECT AVG(hozott + kpmagy + kpmat) AS atlag FROM tagozatok INNER JOIN jelentkezesek ON tagozatok.akod=jelentkezesek.tag
                INNER JOIN diakok ON jelentkezesek.diak = diakok.oktazon
                WHERE nyek = 1
                """;
            SqliteCommand command = new SqliteCommand(query, connection);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                double atlag = reader.GetDouble(0);
                MessageBox.Show($"NYEK tagozat jelentkezők átlagos pontszáma: {atlag:F1}");
            }
        }
    }
}