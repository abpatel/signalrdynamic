using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Common
{
    public class Setting
    {
        public int? ID { get; set; }
        [Required]
        public string Topic { get; set; }
        [Required]
        public string Key
       {
           get;
           set;
       }
        [Required]
        public string Value
        {
            get;
            set;
        }
        public override string ToString()
        {
            return string.Format(@"ID:{0}-System:{1}-{2}({3})", ID, Topic, Key, Value);
        }
    }
}
