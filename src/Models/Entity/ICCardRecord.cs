using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ICCardRecord
    {
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardCode { get; set; }
        /// <summary>
        /// 交易日期
        /// </summary>
        public string TransactionDate { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string TransactionType { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string AmountOfTransaction { get; set; }
        /// <summary>
        /// 刷卡设备
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 路线或公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 站点
        /// </summary>
        public string  Station { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarCode { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public string SettlementDate { get; set; }
        /// <summary>
        /// 星期几
        /// </summary>
        public string WeekDay { get; set; }
        /// <summary>
        /// 星期几对应的数字：0是星期日
        /// </summary>
        public string WeekNum { get; set; }
       
        
    }
}
