using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 满金店用户ID
        /// </summary>
        public string mjdUserId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string photo { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 签名留言
        /// </summary>
        public string userSign { get; set; }
        /// <summary>
        /// 所在行业
        /// </summary>
        public string trade { get; set; }

        /// <summary>
        /// 所在地ID
        /// </summary>
        public string locationAreaId { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string brithday { get; set; }

        /// <summary>
        /// 生肖
        /// </summary>
        public string zodiac { get; set; }
        /// <summary>
        /// 星座
        /// </summary>
        public string constellation { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public string userGrade { get; set; }

        /// <summary>
        /// 全局禁言设置
        /// </summary>
        public string gagGlag { get; set; }

        /// <summary>
        /// 解禁时间
        /// </summary>
        public string removeGagTime { get; set; }

        /// <summary>
        /// 注册来源系统，参见代码类型REGIST_SOURCE
        /// </summary>
        public string registSource { get; set; }

        /// <summary>
        /// 用户来源，参见代码类型USER_COME_FROM
        /// </summary>
        public string userComeFrom { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        //二维码
        public string qrCode { get; set; }

        /// <summary>
        /// 省份ID
        /// </summary>
        public string areaAID { get; set; }
        /// <summary>
        /// 地市ID
        /// </summary>
        public string areaBID { get; set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        public string areaAName { get; set; }
        /// <summary>
        /// 地市名称
        /// </summary>
        public string areaBName { get; set; }
        //public string partnerRemark { get; set; }
        /// <summary>
        /// 上次头像
        /// </summary>
        public string prePhoto { get; set; }
        /// <summary>
        /// 相册风格
        /// </summary>
        public string photoStyle { get; set; }

    }
}
