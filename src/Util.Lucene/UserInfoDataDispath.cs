using Util.Lucene.Core;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Lucene
{
    public class UserInfoDataDispath: DataDispathBase<UserInfo>
    {
        private Core.UserInfoCore core = new Core.UserInfoCore();

        public UserInfoDataDispath()
        {
            core.IndexOpitonMsgCallBack += core_IndexOpitonMsgCallBack;

        }

        void core_IndexOpitonMsgCallBack(object sender, IndexOptionEventArgs<UserInfo> e)
        {

            this.RaiseIndexOptionMsgCallBack(e);

        }

        public override void Enqueue(List<string[]> lst, bool IsEOF)
        {

           // core.Insert(lst, IsEOF);

        }


        public override void Equeue(UserInfo t)
        {
            throw new NotImplementedException();
        }

        public override List<UserInfo> Search(UserInfo t, out int recordCount, int pageindex = 1, int pagesize = 100)
        {
            return core.Search(t, out recordCount, pageindex, pagesize);
        }
        public List<UserInfo> BusinessSearch(UserInfo t)
        {
            return core.BusinessSearch(t);
        }
        public List<UserInfo> BusinessExportSearch(UserInfo t)
        {
            // return core.BusinessExportSearch(t);
            return null;
        }
        public List<UserInfo> Search(string keyword, out int recourdCount, int pageindex = 1, int pagesize = 100)
        {
            return core.Search(keyword, out recourdCount, pageindex, pagesize);
        }

        public override int GetAllDocCount()
        {
            return core.DocCount;
        }
    }
}
