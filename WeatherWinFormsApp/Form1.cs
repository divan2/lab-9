using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;



namespace WeatherWinFormsApp
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private List<Grad> cities;
        private const string CitiesFilePath = "C:/Users/samar/YandexDisk/мгту/2 курс/АЯ/lab 9/task 2/city.txt"; // Путь к файлу с городами

        public Form1()
        {
            InitializeComponent();
            LoadCities();
        }

        private void LoadCities()
        {
            cities = LoadCitiesFromFile(CitiesFilePath);

            cityListBox.Items.Clear();
            foreach (var city in cities)
            {
                cityListBox.Items.Add(city.Name);
            }
        }


        private async void getWeatherButton_Click(object sender, EventArgs e)
        {

            // Определение выбранного города
            int selectedIndex = cityListBox.SelectedIndex;
            Grad selectedCity = cities[selectedIndex];

            try
            {
                // Получение погоды для выбранного города
                Weather? weather = await GetWeatherAsync(selectedCity.Lat, selectedCity.Lon);

                // Вывод данных в TextBox или Label
                if (weather.HasValue)
                {
                    textBox1.Text = weather.Value.ToString();
                    Console.WriteLine(weather.Value.ToString());
                }
                else
                {
                    textBox1.Text = "Не удалось получить данные о погоде.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных о погоде: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static List<Grad> LoadCitiesFromFile(string filename)
        {
            List<Grad> cities = new List<Grad>();
            foreach (string line in File.ReadAllLines(filename))
            {
                string[] parts = line.Replace(", ", "\t").Replace(".", ",").Split('\t');
                if (parts.Length == 3)
                {
                    Grad city = new Grad(
                        parts[0].Trim(),
                        Convert.ToDouble(parts[1]),
                        Convert.ToDouble(parts[2])
                    );
                    cities.Add(city);
                }
            }
            return cities;
        }

        private static async Task<Weather?> GetWeatherAsync(double lat, double lon)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                IConfiguration configuration = builder.Build();
                string apiKey = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=916dc29b0fa142e5927818b6814e62c7&units=metric";
                var response = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid=916dc29b0fa142e5927818b6814e62c7");
                dynamic json = JsonConvert.DeserializeObject(response);
                if (json.sys.country != null)
                {
                    return new Weather
                    {
                        Country = json.sys.country,
                        Name = json.name,
                        Temp = json.main.temp,
                        Description = json.weather[0].description
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных о погоде: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void InitializeComponent()
        {
            cityListBox = new ListBox();
            getWeatherButton = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // cityListBox
            // 
            cityListBox.FormattingEnabled = true;
            cityListBox.ItemHeight = 30;
            cityListBox.Location = new Point(217, 53);
            cityListBox.Name = "cityListBox";
            cityListBox.Size = new Size(1187, 364);
            cityListBox.TabIndex = 0;
            // 
            // getWeatherButton
            // 
            getWeatherButton.Location = new Point(616, 509);
            getWeatherButton.Name = "getWeatherButton";
            getWeatherButton.Size = new Size(351, 58);
            getWeatherButton.TabIndex = 1;
            getWeatherButton.Text = "getWeatherButton";
            getWeatherButton.UseVisualStyleBackColor = true;
            getWeatherButton.Click += getWeatherButton_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(85, 634);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(1630, 35);
            textBox1.TabIndex = 2;
            textBox1.Multiline = true;
            // 
            // Form1
            // 
            ClientSize = new Size(1842, 872);
            Controls.Add(textBox1);
            Controls.Add(getWeatherButton);
            Controls.Add(cityListBox);
            Name = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        public ListBox cityListBox;
        private System.Windows.Forms.Button getWeatherButton;
        private System.Windows.Forms.TextBox textBox1;
    }

    public struct Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public float Temp { get; set; }
        public string Description { get; set; }

        public Weather(string country, string name, float temp, string description)
        {
            Country = country;
            Name = name;
            Temp = temp;
            Description = description;
        }

        public override string ToString()
        {
            return $"Страна: {Country}\n Название города: {Name}\n Температура воздуха: {Temp}°C\n Описание погоды: {Description}\n";
        }
    }

    public class Grad
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public Grad(string name, double lat, double lon)
        {
            Name = name;
            Lat = lat;
            Lon = lon;
        }
    }
}
