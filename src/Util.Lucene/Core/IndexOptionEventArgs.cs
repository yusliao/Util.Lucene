using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
namespace Util.Lucene.Core
{
    //摘要
    //为IIndexOptionMsgCallback.IndexOpitonMsgCallBack 提供数据
    public class IndexOptionEventArgs<T>:EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mc"></param>
        /// <param name="result">true 成功，false失败</param>
        /// <param name="elapsedSeconds">调用耗时,毫秒为单位</param>
        /// <param name="error">调用出错信息</param>
        /// <param name="docCount">当前文档数量</param>
        /// <param name="messageType">1：indexing,2:search</param>
        public IndexOptionEventArgs(T mc, bool result, long? elapsedSeconds, string error = null, string docCount = null,int messageType=1)
        {
            _msgcontent = mc;
            _optionResult = result;
            _error = error;
            doccount = docCount;
            this.elapsedSeconds = elapsedSeconds;
            this.messageType = messageType;
        }
        private int messageType;
        /// <summary>
        /// 1 索引 ，2 查询
        /// </summary>
        public int MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }
        
        private long? elapsedSeconds;

        public long? ElapsedSeconds
        {
            get { return elapsedSeconds; }
            set { elapsedSeconds = value; }
        }
        
        private string doccount;

        public string DocCount
        {
            get { return doccount; }
            set { doccount = value; }
        }
        
        private T _msgcontent;
        public virtual T msgContent
        {
            get
            {
                return _msgcontent;
            }
        }
        private bool _optionResult;
        public bool OptionResult
        {
            get
            {
                return _optionResult;
            }
        }
        private string _error;
        public string Error
        {
            get
            {
                return _error;
            }
        }
    }
}
