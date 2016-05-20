using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStocks
{
    /* This class holds all the stocks as well as the function to compute the shared index of all stocks */
    class StockPortfolio
    {

        /*
         * Constructors
         * First constructor takes generic list of stocks
         * Second is empty constructor
         * */
        public StockPortfolio(List<Stock> stocks)
        {

            this.stocks = stocks;
        }

        public StockPortfolio() { }

        /*
         * Private member variables
         * */
        private List<Stock> stocks;

        /*
        * Public Accessor methods
        * */

        public List<Stock> Stocks
        {
            get { return stocks; }
        }

        /* Functions
         * */

        /* Add stock to porfolio
         * Accepts a Stock object and adds to the
         * private list of stocks 
         * If the stock list is empty then create new list
         * Returns false if the stock already exists.
         * INPUT: Stock object
         * OUTPUT: true if successfully added to portfolio, false if stock is already in portfolio
         * */
        public bool AddStock(Stock stock)
        {
            bool success = true;
            //Stock symbol must not already exist in portfolio
            if (LocateStock(stock.Symbol) != null)
            {
                success = false;
            }
            else
            {
                //Add the stock to the portfolio
                if (stocks == null) stocks = new List<Stock>();
                stocks.Add(stock);
            }

            return success;
            
        }

        /*
        * Locate stock from porfolio by searching for record with matching symbol
         * INPUT: string - unique stock symbol
         * OUTPUT: Stock object (null if not found)
        * */
        public Stock LocateStock(string symbol)
        {
            //Use predicate to locate stock
            Stock foundStock = null;
            if (stocks != null) foundStock = stocks.Find(stock => stock.Symbol == symbol.ToUpper());
            return foundStock;

        }

        /*
         * Calculate Share Index
         * Multiply the stock prices and take the nth root of the result
         * where n is the number of stock prices multiplied together
         * Use Math.Pow as in calculus roots are treated as special cases
         * of exponation, where the exponent is a fraction
         * Convert pence values to pounds and pence decimal prior to computation to
         * reduce risk of overflow.
         * Rounding is performed using round to even rounding
         * Returns zero if there are no stocks in the stock portfolio
         * INPUT: N/A
         * OUTPUT: Share index in pence
         * */

        public int CalculateShareIndex()
        {
            decimal allShareIndex = 0;
            decimal productOfPrices = 0;

            //There must be stocks in order to calculate index
            if (stocks != null && stocks.Count > 0)
            {
                //Loop through stocks and multiply their stock ticker prices together
                for (int i = 0; i < stocks.Count; i++)
                {
                    Stock stock = stocks.ElementAt(i);
                    if (i == 0) productOfPrices = (decimal)stock.TickerPrice / 100; //if first item just set to it's stock price
                    else productOfPrices *= (decimal)stock.TickerPrice / 100;
                }

                //We lose precision here as having to use double for Math.Pow function
                allShareIndex = (decimal)Math.Pow((double)productOfPrices, 1.0 / stocks.Count) * 100;
            }

            return (int)Math.Round(allShareIndex, 0, MidpointRounding.ToEven);
        }
    }
 
}
