using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Util.Lucene
{
    interface IDataDispath<T>
    {
        /// <summary>
        /// 插入整块数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="IsEOF">是否文档插入完毕，开始合并段文件</param>
        void Enqueue(List<string[]> lst, bool IsEOF);
        void Equeue(T t);
        List<T> Search(T t,out int recordCount,int pageindex,int pagesize);
        int GetAllDocCount();
      
        
    }
}
