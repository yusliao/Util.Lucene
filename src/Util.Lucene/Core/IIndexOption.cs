using System;
using System.Collections.Generic;
using System.Text;

namespace Util.Lucene.Core
{
    //摘要
    //定义索引操作
    interface IIndexOption<T> :IIndexOptionMsgCallback<T>
    {
        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="t"></param>
        void Insert(T t);
        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="t"></param>
        void Modify(T t);
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="t"></param>
        void Del(T t);
        /// <summary>
        /// 根据关键字查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        List<T> Search(string keyword,out int recordCount,int pageindex,int pagesize);

        string IndexPath { get; }

       
       
    }

}
