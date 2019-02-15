using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceCLip2
{
    public class ClipboardHistoryItem
    {
        public string Data { get; set; }
        public DateTime At { get; set; }

        public ClipboardHistoryItem()
        {

        }

        public ClipboardHistoryItem(string data)
        {
            Data = data;
            At = DateTime.Now;
        }

        public static implicit operator string(ClipboardHistoryItem c)
        {
            return c.Data;
        }
    }
}
