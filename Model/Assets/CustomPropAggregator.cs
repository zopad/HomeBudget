using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Model.Assets
{
    public class CustomPropAggregator
    {
        [BsonId]
        public ObjectId DatabaseId { get; set; }

        public DateTime TimeStamp
        {
            get; set;
        }

        public decimal Spending
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public CustomPropAggregator()
        {
            TimeStamp = DateTime.Now;
        }

        public CustomPropAggregator(DateTime timeStamp, decimal spending, string title)
        {
            TimeStamp = timeStamp;
            Spending = spending;
            Title = title;
        }
    }
}
