using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace HomeBudget.Model
{
    public class DataAggregator
    {
        /*public Guid Id
        {
            get; set;
        }*/

        [BsonId]
        public ObjectId DatabaseId { get; set; }

        public DateTime TimeStamp
        {
            get; set;
        }

        public decimal Income
        {
            get; set;
        }

        public decimal Spending
        {
            get; set;
        }

        public DataAggregator()
        {
            TimeStamp = DateTime.Now;
        }

        public DataAggregator(decimal income, decimal spending, DateTime timeStamp)
        {
            TimeStamp = timeStamp;
            Income = income;
            Spending = spending;
        }
    }


    
}
