using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleStocks
{
    /* Holds a transaction against a particular stock item */
    public class Transaction
    {

        //Transaction type (BUY or SELL)
        public enum TransactionType
        {
            Buy=1,
            Sell=2
        }

        /*
         * Constructor 
         * */
        public Transaction(int quantity,
            TransactionType type,
            int tradePrice)
        {
            //Set timestamp for this transaction
            this.timestamp = DateTime.Now;

            this.quantity = quantity;
            this.type = type;
            this.tradePrice = tradePrice;
        }


        /*
          * Private member variables
          * */

        private DateTime timestamp; //timestamp when this transaction was performed
        private int quantity; //quantity of shares being exchanged
        private TransactionType type; //Was this a Buy or Sell transaction
        private int tradePrice; //price stock bought or sold for (in pennies)


        /*
        * Accessor methods
         * Do not allow transaction values to change from when transaction was recorded 
         * Only allow transaction to be marked as processed.
        * */

 
        public DateTime Timestamp
        {
            get { return timestamp; }
        }

        public int Quantity
        {
            get { return quantity; }
        }

        public TransactionType Type
        {
            get { return type; }
        }

        //Return string description of Transaction type (BUY/SELL)
        public string TypeDescription
        {
            get { 
                string value = "";
                if (type == TransactionType.Buy) value = "BUY";
                else value = "SELL";
                return value;
            }
        }

        public int TradePrice
        {
            get { return tradePrice; }
        }

        /* Check if transaction was recorded within last number of minutes specified
         * so if minutes=15 this method will return true if the transaction was
         * recorded within the last 15 minutes
         * INPUT: double (number of minutes) 
         * OUTPUT: boolean */
        public bool RecordedInLastInterval(double minutes)
        {
            TimeSpan span = DateTime.Now - this.timestamp;
            return (span.TotalMinutes <= minutes);
        }

      
    }
}
