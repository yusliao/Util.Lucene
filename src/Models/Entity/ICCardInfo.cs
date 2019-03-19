using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ICCardInfo
    {
        public string CardCode { get; set; }
        public int MarkState { get; set; }
     
    }
    class ICCardMarkState
    {
        public const int WHITELIST = 0; //白名单
        public const int BLACKLIST = 1;  //黑名单
        public const int SUSPECTLIST = 2;  //嫌疑名单
    }
}
