using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Lucene.Core
{
    //摘要：
        //表示将处理 IIndexOptionMsgCallback.IndexOpitonMsgCallBack 事件 的方法,该事件在索引操作进行时触发。
    public delegate void IndexOptionMsgCallBackEventHandle<T>(object sender,IndexOptionEventArgs<T> e);
    // 摘要: 
    //     向服务器发出索引操作已进行的通知。
    public interface IIndexOptionMsgCallback<T>
    {
        //摘要： 
            //在进行索引操作时发生
        event IndexOptionMsgCallBackEventHandle<T> IndexOpitonMsgCallBack;
    }
}
