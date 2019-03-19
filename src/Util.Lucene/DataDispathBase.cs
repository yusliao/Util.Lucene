using Util.Lucene.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Lucene
{
    public abstract class DataDispathBase<T> :IDataDispath<T>, IIndexOptionMsgCallback<T>
    {
        public event IndexOptionMsgCallBackEventHandle<T> IndexOpitonMsgCallBack;

      
       
        public abstract void Enqueue(List<string[]> lst,bool IsEOF);
       
        public abstract void Equeue(T t);
       
        public abstract List<T> Search(T t,out int recordCount,int pageindex,int pagesize);

        public abstract int GetAllDocCount();
        protected void RaiseIndexOptionMsgCallBack(IndexOptionEventArgs<T> e)
        {
            if (IndexOpitonMsgCallBack != null)
            {
                IndexOpitonMsgCallBack(this, e);
            }
        }
        
    }
}
