using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Util.Lucene.Core;
using Models;

namespace Util.Lucene
{
    public class ICCardRecordDataDispath :DataDispathBase<ICCardRecord>
    {
        private  ICCardRecordCore core = new ICCardRecordCore();
       
        public ICCardRecordDataDispath()
        {
            core.IndexOpitonMsgCallBack += core_IndexOpitonMsgCallBack;
    
        }

        void core_IndexOpitonMsgCallBack(object sender, IndexOptionEventArgs<ICCardRecord> e)
        {

            this.RaiseIndexOptionMsgCallBack(e);
            
        }
       
        public override void Enqueue(List<string[]>lst,bool IsEOF)
        {

            core.Insert(lst, IsEOF);
            
        }
        

        public override void Equeue(ICCardRecord t)
        {
            throw new NotImplementedException();
        }

        public override List<ICCardRecord> Search(ICCardRecord t,out int recordCount,int pageindex=1,int pagesize=100)
        {
            return core.Search(t,out recordCount, pageindex,pagesize);
        }
        public List<ICCardRecord> BusinessSearch(ICCardRecord t)
        {
            return core.BusinessSearch(t);
        }
        public List<ICCardRecord> BusinessExportSearch(ICCardRecord t)
        {
            return core.BusinessExportSearch(t);
        }
        public List<ICCardRecord> Search(string keyword,out int recourdCount, int pageindex = 1, int pagesize = 100)
        {
            return core.Search(keyword,out recourdCount,pageindex,pagesize);
        }

        public override int GetAllDocCount()
        {
            return core.DocCount;
        }
    }
}
