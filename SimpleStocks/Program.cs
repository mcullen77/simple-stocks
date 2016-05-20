using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStocks
{
    class Program
    {
        //Main method 
        static void Main(string[] args)
        {
            try
            {
                string input;
                int option;
                Stock stock = null; //pointer to located stock item

                Console.WriteLine("Starting SimpleStocks Program...");

                //Create Stock Portfolio
                StockPortfolio stockPortfolio = CreateStockPorfolio();
                Console.WriteLine("Stocks portfolio has been created.");

                do
                {
                    //Display the menu
                    showMenu();

                    //Accept user input
                    input = Console.ReadLine();

                    //Option must be numeric
                    if (!Int32.TryParse(input, out option))
                    {
                        Console.WriteLine("Please enter a numeric option.");
                    }
                    else
                    {
                        //If option is for a particular stock then prompt to get stock name
                        if ((option >= 1) && (option <= 5))
                        {
                            Console.Write("Please enter stock symbol: ");
                            input = Console.ReadLine();
                            stock = stockPortfolio.LocateStock(input); //attempt to find stock in portfolio
                            if (stock == null)
                            {
                                Console.WriteLine("That stock could not be found. Press key to continue");
                                Console.ReadKey();
                                continue;
                            }
                            else ShowStockDetails(stock); //Show current stock details

                        }


                        switch (option)
                        {
                            case 1: ShowDividendYield(stock); break;
                            case 2: ShowPERatio(stock); break;
                            case 3: RecordTransaction(stock); break;
                            case 4:
                                {
                                    Console.WriteLine("Updated ticker price=" + stock.CalculateTickerPrice());
                                }
                                break;
                            case 5: ShowTradesWithinLastPeriod(stock, 15); break;
                            case 6: ShowCurrentStockPrices(stockPortfolio); break;
                            case 7:
                                {
                                    //All share Index will be rounded to Even and in pennies
                                    Console.WriteLine("All share index=" + stockPortfolio.CalculateShareIndex());
                                }
                                break;
                            case 8:
                                break;

                            default: Console.WriteLine("Invalid option has been entered."); break;
                        }
                    } //END-IF

                    if (option != 8) //If not exit
                    {
                        Console.WriteLine("Please press any key to continue.");
                        Console.ReadKey();
                    }
                }
                while (option != 8);


            }
            catch (Exception e)
            {
                Console.WriteLine("An exception has occurred. Exception = " + e.Message);
            }


        }

        /*
         * Create Stock Portfolio - add sample stock 
         * this method returns the created Stock Portfolio with the sample data
         * Also, for each stock item a current ticker price is specified
         * INPUT: N/A
         * OUTPUT: StockPortfolio object
         * */
        static StockPortfolio CreateStockPorfolio()
        {
            StockPortfolio stockPortfolio = new StockPortfolio();

            stockPortfolio.AddStock(new Stock("TEA", Stock.DividendTypeValue.Common, 0, 0, 100, 110));
            stockPortfolio.AddStock(new Stock("TEA", Stock.DividendTypeValue.Common, 0, 0, 100, 110));
            stockPortfolio.AddStock(new Stock("POP", Stock.DividendTypeValue.Common, 8, 0, 100, 120));
            stockPortfolio.AddStock(new Stock("ALE", Stock.DividendTypeValue.Common, 23, 0, 60, 70));
            stockPortfolio.AddStock(new Stock("GIN", Stock.DividendTypeValue.Preferred, 8, 0.02m, 100, 105));
            stockPortfolio.AddStock(new Stock("JOE", Stock.DividendTypeValue.Common, 13, 0, 250, 265));

            return stockPortfolio;
        }

        /*
         * Show main menu for processing stock requests
         * INPUT: N/A
         * OUTPUT: N/A
         * */
        static void showMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Global Beverage Corporation Exchange");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("1. Calculate Dividend Yield for a particular stock");
            Console.WriteLine("2. Calculate P/E Ratio for a particular stock");
            Console.WriteLine("3. Record a trade for a particular stock");
            Console.WriteLine("4. Calculate price for a particular stock based on trades in past 15 minutes");
            Console.WriteLine("5. Show trades within last 15 minutes for a particular stock");
            Console.WriteLine("6. Display current prices for all stock");
            Console.WriteLine("7. Calculate GBCE Share Index");
            Console.WriteLine("8. Exit");
            Console.WriteLine();
            Console.Write("Please select an option:");

        }

        /*
         * Attempt to record a transaction for this particular stock
         * INPUT: Stock object
         * OUTPUT: N/A
         * */
        static void RecordTransaction(Stock stock)
        {
            string input;
            bool error = false;
            int quantity;
            int price;
            Transaction.TransactionType transactionType = Transaction.TransactionType.Buy;


            //Read transaction type
            do
            {
                Console.Write("Is this a BUY transaction (Y/N) ? ");
                input = Console.ReadLine().ToUpper();

                //If invalid input continue until correct input entered (must be Y or N)
                error = ((input[0] != 'Y') && (input[0] != 'N'));
                if (error)
                {
                    Console.WriteLine("Only responses beginning with Y or N are valid.");
                }
                else
                {
                    if (input[0] == 'Y') transactionType = Transaction.TransactionType.Buy;
                    else transactionType = Transaction.TransactionType.Sell;
                }
            }
            while (error == true);


            //Read stock price
            do
            {
                Console.Write("Enter stock trade price in pence:");
                input = Console.ReadLine();

                error = !Int32.TryParse(input, out price);
                if (error) Console.WriteLine("Stock trade price entered must be a whole number (in pennies).");

            }
            while (error == true);

            //Read quantity
            do
            {
                Console.Write("Enter quantity:");
                input = Console.ReadLine();

                error = !Int32.TryParse(input, out quantity);
                if (error) Console.WriteLine("Invalid quanity entered, must be a whole number.");

            }
            while (error == true);


            //Now that we have all the necessary input fields - log the transaction
            stock.AddTransaction(new Transaction(quantity, transactionType, price));
        }


        /*
         * Show Prices for each stock item in the porfolio 
         * INPUTS: StockPorfolio object which contains all stocks
         * OUTPUT: N/A
         * */
        static void ShowCurrentStockPrices(StockPortfolio portfolio)
        {
            foreach (Stock stock in portfolio.Stocks)
            {
                Console.WriteLine("SYMBOL: " + stock.Symbol + ", TICKER PRICE:" + stock.TickerPrice);
            }
        }



        /*
        * Show trades on a particular stock item made within recent time period
         * INPUTS: Stock Object and minutes as double (can specify fractions of minutes as well)
         * OUTPUT: N/A
         * */
        static void ShowTradesWithinLastPeriod(Stock stock, double minutes)
        {
            bool foundTransactions = false;

            Console.WriteLine();

            //Check that there are transactions recorded
            if ((stock.Transactions != null) && (stock.Transactions.Count > 0))
            {
                foreach (Transaction transaction in stock.Transactions)
                {
                    //Transaction recorded within last 15 minutes
                    if (transaction.RecordedInLastInterval(minutes))
                    {
                        Console.WriteLine("[{0}]- QUANTITY:{1,10}, TRADEPRICE: {2,10}, TYPE:{3}",
                            transaction.Timestamp, transaction.Quantity, transaction.TradePrice, transaction.TypeDescription);
                        foundTransactions = true;
                    }
                }

            }

            if (!foundTransactions) Console.WriteLine("No transactions retrieved.");
        }

        /*
         * Show Dividend Yield on the screen
         * Shows full unrounded computed value, as well as the value round to 5 decimal places (no rounding specified)
         * Use to even rounding as this is often used in financial calculations
         * INPUTS: Stock Object
         * OUTPUT: N/A
         * */
        static void ShowDividendYield(Stock stock)
        {
            Stock.StockDecimalResult dividendYieldResult = stock.CalculateDividendYield();
            if (dividendYieldResult.Success)
            {
                Console.WriteLine("Dividend yield=" + dividendYieldResult.Value + ", Rounded to 5 decimal places=" + Math.Round(dividendYieldResult.Value, 5, MidpointRounding.ToEven));
            }
            else Console.WriteLine(dividendYieldResult.ErrorMessage);

        }


        /*
         * Show P/E Ratio on the screen
         * Shows full unrounded decimal value, as well as the value rounded to 5 decimal places (no rounding specified)
         * Use to even rounding as this is often used in financial calculations
         * INPUTS: Stock Object
         * OUTPUT: N/A
         * */
        static void ShowPERatio(Stock stock)
        {
            Stock.StockDecimalResult peRatioResult = stock.CalculatePERatio();
            if (peRatioResult.Success)
            {
                Console.WriteLine("P/E ratio=" + peRatioResult.Value + ", Rounded to 5 decimal places=" + Math.Round(peRatioResult.Value, 5, MidpointRounding.ToEven));
            }
            else Console.WriteLine(peRatioResult.ErrorMessage);

        }

        /*
         * Show Stock details
         * Print out current ticker price, last dividend, dividend type, fixed dividend,  par value
         * INPUTS: Stock Object
         * OUTPUT: N/A
         * */
        static void ShowStockDetails(Stock stock)
        {
            Console.WriteLine("TICKER PRICE:" + stock.TickerPrice +
                ", LAST DIVIDEND:" + stock.LastDividend +
                ", DIVIDEND TYPE:" + stock.DividendTypeDescription +
                ", FIXED DIVIDEND:" + stock.FixedDividend +
                ", PAR VALUE:" + stock.ParValue);
        }

    }
}
