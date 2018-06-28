using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Model
{
    [DisplayName("Listaelem")]
    public class PropListItem
    {
        [BsonId, Browsable(false)]
        public ObjectId DatabaseId { get; set; }

        [DisplayName("Név")]
        public string Name { get; set; }

        [DisplayName("Megjegyzés, leírás"), Description("Pl. mérőállás")]
        public string Leiras { get; set; }

        [Description("A maximum megengedett keret egy hónapra.")]
        public decimal Limit { get; set; }

        public PropListItem(string name)
        {
            Name = name;
        }

        public PropListItem()
        {
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
