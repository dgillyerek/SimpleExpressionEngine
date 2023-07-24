using System;
using System.Collections.Generic;

public class StockPrice
{
    public DateTime Date { get; set; }
    public double OpenPrice { get; set; }
    public double HighPrice { get; set; }
    public double LowPrice { get; set; }
    public double ClosePrice { get; set; }
    public long Volume { get; set; }

    public StockPrice(DateTime date, double openPrice, double highPrice, double lowPrice, double closePrice, long volume)
    {
        Date = date;
        OpenPrice = openPrice;
        HighPrice = highPrice;
        LowPrice = lowPrice;
        ClosePrice = closePrice;
        Volume = volume;
    }
}

public class SampleData
{
    public static List<StockPrice> GetSampleData()
    {
        List<StockPrice> stockPrices = new List<StockPrice>
        {
            // Sample data for a hypothetical stock
            new StockPrice(new DateTime(2023, 7, 18), 100.0, 105.5, 98.5, 103.2, 50000),
            new StockPrice(new DateTime(2023, 7, 19), 104.0, 110.2, 101.0, 109.8, 60000),
            new StockPrice(new DateTime(2023, 7, 20), 110.0, 115.8, 106.5, 113.5, 70000),
            new StockPrice(new DateTime(2023, 7, 21), 112.0, 118.5, 110.2, 116.7, 80000),
            new StockPrice(new DateTime(2023, 7, 22), 115.0, 120.0, 112.8, 118.4, 90000)
        };

        return stockPrices;
    }
}
