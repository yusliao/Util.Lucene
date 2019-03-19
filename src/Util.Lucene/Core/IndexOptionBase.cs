using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.Store;
namespace Util.Lucene.Core
{
    public abstract class IndexOptionBase<T>:IIndexOption<T>,IDisposable
    {

        // 索引操作回调事件
        public event IndexOptionMsgCallBackEventHandle<T> IndexOpitonMsgCallBack;
       
        /// <summary>
        /// 盘古分词器
        /// </summary>
        protected static Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);// new PanGuAnalyzer();
        /// <summary>
        /// 消息泵控制标志
        /// </summary>
        protected bool isStop = false;

     //   public static Dictionary<string, IndexOptionBase<T>> indexDictionary = new Dictionary<string, IndexOptionBase<T>>();

        protected IndexOptionBase()
        {
            
            Init();
            if (!isStop)
            {
                new Thread(() => QueueToIndex()).Start();
                
            }
        }
        /// <summary>
        /// 消息泵业务处理
        /// </summary>
        /// <param name="obj"></param>
        protected abstract void QueueToIndex();
        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="t"></param>
        public abstract void Insert(T t);
     
        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="t"></param>
        public abstract void Modify(T t);
        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="t"></param>
        public abstract void Del(T t);
        /// <summary>
        /// 单文件检索
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public abstract List<T> Search(string keyword,out int recordCount,int pageindex,int pagesize);
        public abstract List<T> Search(T keyword,out int recordCount, int pageindex, int pagesize);
       
        /// <summary>
        /// 单文件索引处理
        /// </summary>
        protected abstract void CRUDIndex();

        protected virtual int GetDocCount()
        {
            using (var search = new IndexSearcher(FSDirectory.Open(IndexPath), true))
            {

                return search.IndexReader.NumDocs();
            }
        }

        public abstract void Init();

        public int DocCount
        {
            get
            {
                return GetDocCount();
            }
        }
        
        public virtual void Dispose()
        {
            isStop = true;
            analyzer.Dispose();
        }
        protected void RaiseIndexOptionMsgCallBack(IndexOptionEventArgs<T> e)
        {
            if (IndexOpitonMsgCallBack != null)
            {
                IndexOpitonMsgCallBack(this, e);
            }
        }

        public abstract string IndexPath { get; }



       
    }
}
