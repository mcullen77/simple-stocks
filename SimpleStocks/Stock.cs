using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStocks
{
   
    /*This class will hold details for each stock offered by the Global Beverage Corporation Exchange (GBCE) */
    public class Stock
    {
 
        /* This class holds the result of decimal operations performed on the Stock object
         * This is used for those methods returning a decimal value where there needs to be
         * a boolean success value passed back to the form along with an error message when
         * there is a failure */
        public class StockDecimalResult
        {

            public StockDecimalResult(bool success, decimal value, string errorMessage)
            {

                this.success = success;
                this.value = value;
                this.errorMessage = errorMessage;
            }

            private bool success; //was the operation successful
            private decimal value; //decimal result
            private string errorMessage; //error message populated if there was a failure

            public bool Success
            {
                get { return success; }
            }

            public decimal Value
            {
                get { return value; }
            }

            public string ErrorMessage
            {
                get { return errorMessage; }
            }
            
        }

        //DividendTypeValue indicates whether Common or Preferred Dividend
        public enum DividendTypeValue
        {

            Common = 1,
            Preferred = 2

        }

        /*
        * Constructor 
        * */
        public Stock(string symbol,
            DividendTypeValue dividendType,
            int lastDividend,
            decimal fixedDividend,
            int parValue,
            int tickerPrice)
        {
            this.symbol = symbol.ToUpper(); //always store symbol in upper case
            this.dividendType = dividendType;
            this.lastDividend = lastDividend;
            this.fixedDividend = fixedDividend;
            this.parValue = parValue;
            this.tickerPrice = tickerPrice;
        }

        /*
         * Private member variables
         * */

        private string symbol;
        private DividendTypeValue dividendType;  //DividendType
        private int lastDividend; //Last Dividend value in pence
        private decimal fixedDividend; //Fixed dividend decimal percentage
        private int parValue; //Par value of stock in pence
        private int tickerPrice; //Current ticker price in pence
        private List<Transaction> transactions; //List of transactions performed against this stock item
 
        /*
         * Accessor methods
         * */

        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }

        public DividendTypeValue DividendType
        {
            get { return dividendType; }
            set { dividendType = value; }
        }

        public string DividendTypeDescription
        {
            get
            {
                string value = "";
                if (dividendType == DividendTypeValue.Common) value = "COMMON";
                else value = "PREFERRED";
                return value;
            }
                
        }

        public int LastDividend
        {
            get { return lastDividend; }
            set { lastDividend = value; }
        }

        public decimal FixedDividend
        {
            get { return fixedDividend; }
            set { fixedDividend = value; }
        }

        public int ParValue
        {
            get { return parValue; }
            set { parValue = value; }
        }

        //Ticker Price can not be updated directly, it will be computed in a separate method.
        public int TickerPrice
        {
            get { return tickerPrice; }
            
        }

        public List<Transaction> Transactions
        {
            get { return transactions;  }
        }

        /* Calculate P/E Ratio
         * This should be calculated as Ticker Price / Dividend
         * P/E ratio should look at dividend from last 12 months
         * If Last Dividend is 0 then return an error
         * INPUT : N/A
         * OUTPUT: StockDecimalResult 
         * */
        public StockDecimalResult CalculatePERatio()
        {
            decimal peRatio = this.tickerPrice;
            string errorMessage = "";
            bool success = true;

            //If dividend is zero then we can not compute P/E Ratio, can't divide by zero
            if (this.lastDividend == 0)
            {
                success = false;
                errorMessage = "Last Dividend is zero, therefore cannot compute P/E ratio";
            }
            else
            {
                peRatio /= this.LastDividend;
            }
           

            //Return result as StockDecimalResult object
            return new StockDecimalResult(success, peRatio, errorMessage);
        }

        /*
         * Calculate Dividend Yield for stock
         * No rounding has been specified so this code will not round the result and result will be expressed as raw decimal
         * If Stock Dividend Type is 'Common' this is computed as Last Dividend / Ticker Price
         * If Stock Dividend Type is 'Preferred' this is computed as Fixed Dividend X Par Value / Ticker Price
         * Checks to avoid division by zero (should ticker price be zero) and that the Dividend Type specified is correct
         * INPUT: N/A
         * OUTPUT: StockDecimalResult
         * */

        public StockDecimalResult CalculateDividendYield()
        {
            decimal dividendYield = 0;
            bool success = true;
            string errorMessage = "";
       
            //Avoid division by zero error
            if (this.tickerPrice != 0)

            {
                switch (this.dividendType)
                {
                    case DividendTypeValue.Common:

                        dividendYield = ((decimal)this.lastDividend) / this.tickerPrice;
                        break;

                    case DividendTypeValue.Preferred:

                        //Fixed dividend is a decimal so result will be decimal
                        dividendYield = (this.fixedDividend * this.parValue) / this.tickerPrice;
                        break;

                    default:
                        success = false;
                        errorMessage = "Invalid Dividend Type specified.";
                        break;

                }
            }
            else
            {
                success = false;
                errorMessage = "Current ticker price is zero, cannot divide by zero.";
            }


            //Return result as StockDecimalResult object
            return new StockDecimalResult(success, dividendYield, errorMessage);

        }

      
        /*
         * Set stock price based on transactions recorded in the last 15 minutes 
         * Returns updated stock price in pennies
         * If there are no transactions OR no transactions in the last 15 minutes return the 
         * current ticker price
         * Based on requirements BUY vs SELL shares doesn't come into it
         * Calculations performed in decimal pounds and pennies to minimize risk of overflow
         * INPUT: N/A
         * OUTPUT: Integer (ticket price in pennies)
         * */
        public int CalculateTickerPrice()
        {
            decimal totalSales = 0; //Hold total sales in pence
            int totalQuantity = 0; //Total number of shares traded

            //First check that there are transactions
            if (transactions != null)
            {
                //Loop through transactions
                foreach (Transaction t in transactions)
                {
                    //Has this transaction been recorded in the last 15 minutes 
                    if (t.RecordedInLastInterval(15))
                    {
                        //Increment total sales
                        totalSales += ((decimal)t.TradePrice / 100) * t.Quantity;
                        totalQuantity += t.Quantity;
                        //Update transaction as processed, don't want to include in further calculations
                    }
                }

                //If sales were made in the last 15 minute then compute new stock price
                if (totalQuantity > 0) 
                {
                    tickerPrice = (int)Math.Round(((decimal)totalSales * 100) / totalQuantity, 0, MidpointRounding.ToEven);
                }
            }
            return tickerPrice;

        }

        /* Add Transaction performed on this stock
         * If transaction list is null then create it
         * Add passed Transaction to transactions list
         * INPUT: Transaction object
         * OUTPUT: n/a
         * */
        public void AddTransaction(Transaction transation)
        {
            if (transactions == null) transactions = new List<Transaction>();
            transactions.Add(transation);
        }
    }
}
