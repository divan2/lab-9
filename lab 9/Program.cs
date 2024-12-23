
class lab091
{
    static readonly Mutex mutex = new Mutex();
    static async Task Main()
    {


        List<string> tickers = new List<string>();

        using (StreamReader reader = new StreamReader("C:/Users/samar/YandexDisk/мгту/2 курс/АЯ/lab 9/lab 9/ticker.txt"))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                tickers.Add(line);
            }
        }


        List<Task> tasks = new List<Task>();
        foreach (string ticker in tickers)
        {

            tasks.Add(GetDataForTicker(ticker));

        }
        await Task.WhenAll(tasks);


    }

    static async Task GetDataForTicker(string ticker)
    {
        using (HttpClient client = new HttpClient())
        {

            string url = $"https://api.marketdata.app/v1/stocks/candles/D/{ticker}/?from=2024-12-10&to=2024-12-21&token=ZEFSZWVaNmFoenl0Wlh4NHAtekdVN3dCdnpHOElVU2cyNm5hWjVTeXZaVT0";

            HttpResponseMessage response = await client.GetAsync(url);

            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic responceObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
            double averagePrice = 0;


            if (responceObject != null && responceObject.t != null && responceObject.h != null && responceObject.l != null)
            {
                List<long> timestamps = responceObject?.t?.ToObject<List<long>>() ?? new List<long>();
                List<double> highs = responceObject?.h?.ToObject<List<double>>() ?? new List<double>();
                List<double> lows = responceObject?.l?.ToObject<List<double>>() ?? new List<double>();

                for (int i = 0; i < timestamps.Count; i++)
                {
                    averagePrice += (highs[i] + lows[i]) / 2;
                }
                averagePrice /= timestamps.Count;
                if (averagePrice == 0)
                {
                    Console.WriteLine($"Ошибка при обработке {ticker}: Отсутствуют данные.");
                    await WriteToFile(ticker, averagePrice);
                }
                else
                {
                    Console.WriteLine(ticker, averagePrice);
                    await WriteToFile(ticker, averagePrice);
                }
            }
        }
    }



    private static async Task WriteToFile(string ticker, double averagePrice)
    {
        mutex.WaitOne();
        try
        {
            using (StreamWriter writer = new StreamWriter("C:/Users/samar/YandexDisk/мгту/2 курс/АЯ/lab 9/lab 9/Average.txt", true))
            {
                await writer.WriteAsync($"{ticker}:{averagePrice} \n");
                Console.WriteLine($"{ticker}: {averagePrice}");
            }
        }
        finally
        {
            mutex.ReleaseMutex(); // Освобождаем мьютекс после завершения операции
        }
    }
}