using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Models;


namespace Util.Lucene.Core
{
    //摘要
    // 提供对表Cargo 的索引操作处理
    public class ICCardRecordCore : IndexOptionBase<ICCardRecord>
    {

        public const string indexPath = "ICCardRecord";
        private static Queue<ICCardRecord> cardRecordQueue = new Queue<ICCardRecord>();
        private static FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory, indexPath)), new NativeFSLockFactory());
       
        static IndexWriter writer;

        static ICCardRecordCore()
        {
            bool isExist = IndexReader.IndexExists(directory);
            if (isExist)
            {
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }
            writer = new IndexWriter(directory, analyzer, !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            writer.MergeFactor = 10;
        }

        protected override void QueueToIndex()
        {
            while (!isStop)
            {
                //if(Instance.DocCount)
                if (cardRecordQueue.Count > 0)
                {
                    try
                    {
                        CRUDIndex();
                    }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    Thread.Sleep(8000);
                }
            }
        }
        public override string IndexPath
        {
            get
            {
                return indexPath;
            }
        }

        public override void Insert(ICCardRecord t)
        {
            cardRecordQueue.Enqueue(t);
        }

        public override void Modify(ICCardRecord t)
        {
            cardRecordQueue.Enqueue(t);
        }

        public override void Del(ICCardRecord t)
        {
            cardRecordQueue.Enqueue(t);
        }


        public override List<ICCardRecord> Search(string keyword, out int recordCount, int pageindex = 1, int pagesize = 100)
        {
            List<ICCardRecord> modelList = new List<ICCardRecord>();
            using (ParallelMultiSearcher searcher = new ParallelMultiSearcher(new IndexSearcher(directory, true)))
            {

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                #region 采用综合分析查询器进行分词查询
              //  QueryParser parser = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, "SettlementData", analyzer);

               // StringBuilder sb = new StringBuilder();
                //把用户输入的关键字进行分词
                // sb.Append(string.Format("CardCode:{0} ", keyword));
               // sb.Append(string.Format("CompanyName:{0}  ", keyword));
               // string str = "CardCode:20160802 +SettlementDate:[20160802 TO 20160802]";
               // Query query = parser.Parse(str);
                #endregion

                #region MultiFieldQueryParse

                MultiFieldQueryParser parser = new MultiFieldQueryParser(global::Lucene.Net.Util.Version.LUCENE_30, new string[] { "CardCode", "TransactionDate", "TransactionType", "Time", "AmountOfTransaction", "DeviceCode", "Station", "CompanyName", "CarCode", "SettlementData", "WeekDay", "WeekNum" }, analyzer);

                Query query = parser.Parse(keyword);

                #endregion


                TopDocs tds = searcher.Search(query, pageindex * pagesize);
                recordCount = tds.TotalHits;

                // Console.WriteLine("TotalHits: " + tds.TotalHits);

                foreach (ScoreDoc sd in tds.ScoreDocs.Skip(pageindex * pagesize-pagesize))
                {

                    Document doc = searcher.Doc(sd.Doc);
                    ICCardRecord model = new ICCardRecord()
                    {
                        CardCode = doc.Get("CardCode"),
                        TransactionDate = doc.Get("TransactionDate"),
                        Time = doc.Get("Time"),
                        TransactionType = doc.Get("TransactionType"),
                        AmountOfTransaction = doc.Get("AmountOfTransaction"),

                        DeviceCode = doc.Get("DeviceCode"),
                        CompanyName = doc.Get("CompanyName"),
                        Station = doc.Get("Station"),
                        CarCode = doc.Get("CarCode"),
                        SettlementDate = doc.Get("SettlementData"),
                        WeekDay = doc.Get("WeekDay"),
                        WeekNum = doc.Get("WeekNum")
                    };

                    modelList.Add(model);

                }
                watch.Stop();
                RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, true, watch.ElapsedMilliseconds, null, tds.TotalHits.ToString(), 2));

            }
            return modelList;
        }
        public override List<ICCardRecord> Search(ICCardRecord record,out int recordCount, int pageindex = 1, int pagesize = 100)
        {
            List<ICCardRecord> modelList = new List<ICCardRecord>();
            using (ParallelMultiSearcher searcher = new ParallelMultiSearcher(new IndexSearcher(directory, true)))
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                #region 采用综合分析查询器进行分词查询
                QueryParser parser = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, "CardCode", analyzer);

                StringBuilder sb = new StringBuilder();
                //把用户输入的关键字进行分词
                //  sb.Append(string.Format("Name:{0} ", keyword));
                if (!string.IsNullOrEmpty(record.CardCode))
                    sb.Append(string.Format("(CardCode:{0}) ", record.CardCode));
                string[] items = null;
                if (!string.IsNullOrEmpty(record.TransactionDate))
                {
                    items = record.TransactionDate.Split(new char[] { '，', ',', ' ' });
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND TransactionDate:[{0} TO {1}] ", items[0], items[1]));
                    }
                    else
                    {
                        sb.Append(string.Format(" TransactionDate:[{0} TO {1}] ", items[0], items[1]));
                    }
                }

                if (!string.IsNullOrEmpty(record.Time))
                {
                    var timespans = record.Time.Split('|');
                    foreach (string item in timespans)
                    {
                        items = item.Split(new char[] { '，', ',', ' ' });
                        if (sb.Length > 0)
                        {
                            sb.Append(string.Format("AND Time:[{0} TO {1}] ", items[0], items[1]));
                        }
                        else
                        {
                            sb.Append(string.Format("Time:[{0} TO {1}] ", items[0], items[1]));
                        }
                    }

                }

                if (!string.IsNullOrEmpty(record.CompanyName))
                {
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND CompanyName:{0} ", record.CompanyName));
                    }
                    else
                        sb.Append(string.Format("CompanyName:{0} ", record.CompanyName));
                }
                if (!string.IsNullOrEmpty(record.DeviceCode))
                {
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND DeviceCode:{0} ", record.DeviceCode));
                    }
                    else
                        sb.Append(string.Format("DeviceCode:{0} ", record.DeviceCode));
                }

                if (!string.IsNullOrEmpty(record.SettlementDate))
                {
                    items = record.SettlementDate.Split(new char[] { '，', ',', ' ' });
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND SettlementData:[{0} TO {1}] ", items[0], items[1]));
                    }
                    else
                        sb.Append(string.Format("SettlementData:[{0} TO {1}] ", items[0], items[1]));
                }
                if (!string.IsNullOrEmpty(record.Station))
                {
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND Station:{0} ", record.Station));
                    }
                    else
                        sb.Append(string.Format("Station:{0} ", record.Station));
                }


                if (!string.IsNullOrEmpty(record.CarCode))
                {
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND CarCode:{0} ", record.CarCode));
                    }
                    else
                        sb.Append(string.Format("CarCode:{0} ", record.CarCode));
                }
                if (!string.IsNullOrEmpty(record.WeekNum))
                {
                    if (sb.Length > 0)
                    {

                        sb.Append(string.Format("AND WeekNum:{0} ", record.WeekNum));
                    }
                    else
                        sb.Append(string.Format(" WeekNum:{0} ", record.WeekNum));

                }
                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, false, 0, messageType: 2, error: "请输入要查询的内容"));
                    recordCount = 0;
                    return modelList;
                }
              
               
              
                Query query = parser.Parse(sb.ToString());
                #endregion

                #region MultiFieldQueryParse

                //MultiFieldQueryParser parser = new MultiFieldQueryParser(global::Lucene.Net.Util.Version.LUCENE_30, new string[] { "Name", "CargoID", "CargoCode", "CargoOwner" }, analyzer);
                //Query query = parser.Parse(keyword);

                #endregion
              

                TopDocs tds = searcher.Search(query, pageindex*pagesize);
                recordCount = tds.TotalHits;
                
                // Console.WriteLine("TotalHits: " + tds.TotalHits);
                
                foreach (ScoreDoc sd in tds.ScoreDocs.Skip(pageindex*pagesize-pagesize))
                {

                    Document doc = searcher.Doc(sd.Doc);
                    ICCardRecord model = new ICCardRecord()
                    {
                        CardCode = doc.Get("CardCode"),
                        TransactionDate = doc.Get("TransactionDate"),
                        Time = doc.Get("Time"),
                        TransactionType = doc.Get("TransactionType"),
                        AmountOfTransaction = doc.Get("AmountOfTransaction"),

                        DeviceCode = doc.Get("DeviceCode"),
                        CompanyName = doc.Get("CompanyName"),
                        Station = doc.Get("Station"),
                        CarCode = doc.Get("CarCode"),
                        SettlementDate = doc.Get("SettlementData"),
                        WeekDay = doc.Get("WeekDay"),
                        WeekNum = doc.Get("WeekNum")
                    };

                    modelList.Add(model);

                }
                watch.Stop();
                RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, true, watch.ElapsedMilliseconds, null, tds.TotalHits.ToString(), 2));
            }

            return modelList;
        }
        public List<ICCardRecord> BusinessSearch(ICCardRecord record)
        {
            List<ICCardRecord> modelList = new List<ICCardRecord>();
            using (ParallelMultiSearcher searcher = new ParallelMultiSearcher(new IndexSearcher(directory, true)))
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                #region 采用综合分析查询器进行分词查询
                QueryParser parser = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, "CardCode", analyzer);

                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(record.CardCode))
                    sb.Append(string.Format("(CardCode:{0}) ", record.CardCode));
                string[] items = null;
                if (!string.IsNullOrWhiteSpace(record.TransactionDate))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    items = record.TransactionDate.Trim().Split('|');
                    foreach (string item in items)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        var temps = item.Split(new char[] { '，', ',', ' ', '-' });
                         if(string.Equals(temps[0],temps[1]))
                         {
                             sb.Append(string.Format("  TransactionDate:{0} ", temps[0]));
                         }
                        else
                            sb.Append(string.Format("  TransactionDate:[{0} TO {1}] ", temps[0], temps[1]));
                    }
                    sb.Append(" ) ");
                }

                if (!string.IsNullOrWhiteSpace(record.Time))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var timespans = record.Time.Trim().Split('|');
                    foreach (string item in timespans)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        items = item.Split(new char[] { '，', ',', ' ','-' });
                      
                        sb.Append(string.Format("  Time:[{0} TO {1}] ", items[0], items[1]));
                       
                    }
                    sb.Append(") ");

                }

                if (!string.IsNullOrWhiteSpace(record.CompanyName))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var CompanyNames = record.CompanyName.Trim().Split('|');
                    foreach (string item in CompanyNames)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        sb.Append(string.Format(" CompanyName:{0} ", record.CompanyName));

                    }
                    sb.Append(" ) ");

                   
                }
                if (!string.IsNullOrWhiteSpace(record.DeviceCode))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var DeviceCodes = record.DeviceCode.Trim().Split('|');
                    foreach (string item in DeviceCodes)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        sb.Append(string.Format(" DeviceCode:{0} ", item));

                    }
                    sb.Append(" ) ");
                   
                }

                if (!string.IsNullOrWhiteSpace(record.SettlementDate))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var timespans = record.SettlementDate.Trim().Split('|');
                    foreach (string item in timespans)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        items = item.Split(new char[] { '，', ',', ' ', '-' });

                        sb.Append(string.Format(" SettlementData:[{0} TO {1}] ", items[0], items[1]));

                    }
                    sb.Append(" ) ");
                   
                }
                if (!string.IsNullOrWhiteSpace(record.Station))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var Stations = record.Station.Trim().Split('|');
                    foreach (string item in Stations)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;

                        sb.Append(string.Format(" Station:{0} ", item));

                    }
                    sb.Append(" ) ");
                  
                }


                if (!string.IsNullOrWhiteSpace(record.CarCode))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var CarCodes = record.CarCode.Trim().Split('|');
                    foreach (string item in CarCodes)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;

                        sb.Append(string.Format(" CarCode:{0} ", item));

                    }
                    sb.Append(" ) ");
                   
                }
                if (!string.IsNullOrWhiteSpace(record.WeekNum))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var WeekNums = record.WeekNum.Trim().Split('|');
                    foreach (string item in WeekNums)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;

                        sb.Append(string.Format(" WeekNum:{0} ", item));

                    }
                    sb.Append(" ) ");

                }
                if (string.IsNullOrWhiteSpace(sb.ToString()))
                {
                    RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, false, 0, messageType: 2, error: "请输入要查询的内容"));
                    return modelList;
                }
              //  var str = " (   TransactionDate:20160822  )  AND (   Time:[073000 TO 080000] )  AND (  Station:木棉湾  )";
                Query query = parser.Parse(sb.ToString());
              //  Query query = parser.Parse(str);
                #endregion

                #region MultiFieldQueryParse

                //MultiFieldQueryParser parser = new MultiFieldQueryParser(global::Lucene.Net.Util.Version.LUCENE_30, new string[] { "Name", "CargoID", "CargoCode", "CargoOwner" }, analyzer);
                //Query query = parser.Parse(keyword);

                #endregion

                TopDocs tds = searcher.Search(query, 10);
                if (tds.TotalHits > 0)
                {
                    try
                    {
                        tds = searcher.Search(query, tds.TotalHits);
                    }
                    catch (Exception ex)
                    {
                        watch.Stop();
                        RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, false, watch.ElapsedMilliseconds, ex.Message, tds.TotalHits.ToString(), 2));
                        return modelList;
                    }
                 
                }
                // Console.WriteLine("TotalHits: " + tds.TotalHits);
                foreach (ScoreDoc sd in tds.ScoreDocs)
                {
                   
                    Document doc = searcher.Doc(sd.Doc);
                    ICCardRecord model = new ICCardRecord()
                    {
                        CardCode = doc.Get("CardCode"),
                        TransactionDate = doc.Get("TransactionDate"),
                        Time = doc.Get("Time"),
                        TransactionType = doc.Get("TransactionType"),
                        AmountOfTransaction = doc.Get("AmountOfTransaction"),

                        DeviceCode = doc.Get("DeviceCode"),
                        CompanyName = doc.Get("CompanyName"),
                        Station = doc.Get("Station"),
                        CarCode = doc.Get("CarCode"),
                        SettlementDate = doc.Get("SettlementData"),
                        WeekNum = doc.Get("WeekNum"),
                        WeekDay = doc.Get("WeekDay")
                    };

                    modelList.Add(model);

                }
                watch.Stop();
                RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, true, watch.ElapsedMilliseconds, null, tds.TotalHits.ToString(), 2));
            }

            return modelList;
        }
        public List<ICCardRecord> BusinessExportSearch(ICCardRecord record)
        {
            List<ICCardRecord> modelList = new List<ICCardRecord>();
            using (ParallelMultiSearcher searcher = new ParallelMultiSearcher(new IndexSearcher(directory, true)))
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                #region 采用综合分析查询器进行分词查询
                QueryParser parser = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, "CardCode", analyzer);

                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(record.CardCode))
                    sb.Append(string.Format("(CardCode:{0}) ", record.CardCode));
                string[] items = null;
                if (!string.IsNullOrEmpty(record.TransactionDate.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    items = record.TransactionDate.Split('|');
                    foreach (string item in items)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        var temps = item.Split(new char[] { '，', ',', ' ', '-' });

                        sb.Append(string.Format("  TransactionDate:[{0} TO {1}] ", temps[0], temps[1]));
                    }
                    sb.Append(" ) ");
                }

                if (!string.IsNullOrEmpty(record.Time.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var timespans = record.Time.Split('|');
                    foreach (string item in timespans)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        items = item.Split(new char[] { '，', ',', ' ', '-' });

                        sb.Append(string.Format("  Time:[{0} TO {1}] ", items[0], items[1]));

                    }
                    sb.Append(") ");

                }

                if (!string.IsNullOrEmpty(record.CompanyName.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var CompanyNames = record.CompanyName.Split('|');
                    foreach (string item in CompanyNames)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        sb.Append(string.Format(" CompanyName:{0} ", record.CompanyName));

                    }
                    sb.Append(" ) ");


                }
                if (!string.IsNullOrEmpty(record.DeviceCode.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var DeviceCodes = record.DeviceCode.Split('|');
                    foreach (string item in DeviceCodes)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        sb.Append(string.Format(" DeviceCode:{0} ", item));

                    }
                    sb.Append(" ) ");

                }

                if (!string.IsNullOrEmpty(record.SettlementDate.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var timespans = record.SettlementDate.Split('|');
                    foreach (string item in timespans)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;
                        items = item.Split(new char[] { '，', ',', ' ', '-' });

                        sb.Append(string.Format(" SettlementData:[{0} TO {1}] ", items[0], items[1]));

                    }
                    sb.Append(" ) ");

                }
                if (!string.IsNullOrEmpty(record.Station.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var Stations = record.Station.Split('|');
                    foreach (string item in Stations)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;

                        sb.Append(string.Format(" Station:{0} ", item));

                    }
                    sb.Append(" ) ");

                }


                if (!string.IsNullOrEmpty(record.CarCode.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var CarCodes = record.CarCode.Split('|');
                    foreach (string item in CarCodes)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;

                        sb.Append(string.Format(" CarCode:{0} ", item));

                    }
                    sb.Append(" ) ");

                }
                if (!string.IsNullOrEmpty(record.WeekNum.Trim()))
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append("  ( ");
                    else
                        sb.Append(" AND ( ");
                    var WeekNums = record.WeekNum.Split('|');
                    foreach (string item in WeekNums)
                    {
                        if (string.IsNullOrEmpty(item))
                            continue;

                        sb.Append(string.Format(" WeekNum:{0} ", item));

                    }
                    sb.Append(" ) ");

                }
                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, false, 0, messageType: 2, error: "请输入要查询的内容"));
                    return modelList;
                }
                Query query = parser.Parse(sb.ToString());
                #endregion

                #region MultiFieldQueryParse

                //MultiFieldQueryParser parser = new MultiFieldQueryParser(global::Lucene.Net.Util.Version.LUCENE_30, new string[] { "Name", "CargoID", "CargoCode", "CargoOwner" }, analyzer);
                //Query query = parser.Parse(keyword);

                #endregion

                TopDocs tds = searcher.Search(query, 10);
                if (tds.TotalHits > 0)
                {
                    tds = searcher.Search(query, tds.TotalHits);
                }
                // Console.WriteLine("TotalHits: " + tds.TotalHits);
                foreach (ScoreDoc sd in tds.ScoreDocs)
                {

                    Document doc = searcher.Doc(sd.Doc);
                    ICCardRecord model = new ICCardRecord()
                    {
                        CardCode = doc.Get("CardCode"),
                        TransactionDate = doc.Get("TransactionDate"),
                        //Time = doc.Get("Time"),
                        //TransactionType = doc.Get("TransactionType"),
                        //AmountOfTransaction = doc.Get("AmountOfTransaction"),

                        //DeviceCode = doc.Get("DeviceCode"),
                        //CompanyName = doc.Get("CompanyName"),
                        //Station = doc.Get("Station"),
                        //CarCode = doc.Get("CarCode"),
                        //SettlementDate = doc.Get("SettlementData")
                    };

                    modelList.Add(model);

                }
                watch.Stop();
                RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, true, watch.ElapsedMilliseconds, null, tds.TotalHits.ToString(), 2));
            }

            return modelList;
        }

        protected override void CRUDIndex()
        {

            // FSDirectory directory = FSDirectory.Open(new System.IO.DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory, indexPath)), new NativeFSLockFactory());
            bool isExist = IndexReader.IndexExists(directory);
            if (isExist)
            {
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }
            IndexWriter writer = new IndexWriter(directory, analyzer, !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            while (cardRecordQueue.Count > 0)
            {
                Document document = new Document();
                ICCardRecord Model = cardRecordQueue.Dequeue();
                try
                {

                    if (Model != null)
                    {

                        document.Add(new Field("CardCode", Model.CardCode, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("TransactionDate", Model.TransactionDate, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("Time", Model.Time, Field.Store.YES, Field.Index.NOT_ANALYZED,
                                                Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("TransactionType", Model.TransactionType, Field.Store.YES, Field.Index.NOT_ANALYZED,
                                                Field.TermVector.NO));
                        document.Add(new Field("AmountOfTransaction", Model.AmountOfTransaction.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("DeviceCode", Model.DeviceCode, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("CompanyName", Model.CompanyName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("Station", Model.Station, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("CarCode", Model.CarCode, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("SettlementData", Model.SettlementDate, Field.Store.YES, Field.Index.NOT_ANALYZED));

                        writer.AddDocument(document);

                    }
                }
                catch (Exception ex)
                {
                    RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(Model, false, null, ex.Message, this.GetDocCount().ToString()));
                }
            }
            writer.Dispose();
            directory.Dispose();
        }

        protected override int GetDocCount()
        {
            using (var search = new IndexSearcher(FSDirectory.Open(indexPath), true))
            {

                return search.IndexReader.NumDocs();
            }
        }
        public override void Init()
        {
            if (!directory.Directory.Exists)
            {
                directory.Directory.Create();
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            directory.Dispose();
            cardRecordQueue.Clear();
            writer.Dispose();


        }

        
        public void Insert(List<string[]> lst, bool IsEOF)
        {
            if (writer == null)
            {
                writer = new IndexWriter(directory, analyzer, !IndexReader.IndexExists(directory), IndexWriter.MaxFieldLength.UNLIMITED);
                writer.MergeFactor = 100;

            }
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            if (lst != null && lst.Any())
            {
                try
                {
                   
                    System.Globalization.CultureInfo curCI = new System.Globalization.CultureInfo("zh-CN");
                    foreach (string[] dr in lst)
                    {
                        if (isStop) break;

                        Document document = new Document();

                        document.Add(new Field("CardCode", dr[0].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("TransactionDate", dr[1].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("Time", dr[2].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("TransactionType", dr[3].ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
                        document.Add(new Field("AmountOfTransaction", dr[4].ToString().ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("DeviceCode", dr[5].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("CompanyName", dr[6].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Station", dr[7].ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("CarCode", dr[8].ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
                        document.Add(new Field("SettlementData", dr[9].ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        DayOfWeek dw =curCI.Calendar.GetDayOfWeek(DateTime.ParseExact(dr[1].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture));
                        int weeknum = (int)dw;
                        document.Add(new Field("WeekDay", dw.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("WeekNum", weeknum.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        writer.AddDocument(document);

                        
                    }
                   
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, false, watch.ElapsedMilliseconds, ex.Message));
                    if (writer != null)
                    {
                        writer.Dispose();
                        writer = null;
                    }
                    return;
                }

            }
            if (IsEOF)
            {
                try
                {
                    writer.Optimize();
                    writer.Commit();
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, false, watch.ElapsedMilliseconds, ex.Message));
                    writer.Dispose();
                    writer = null;
                }


            }
            watch.Stop();

            RaiseIndexOptionMsgCallBack(new IndexOptionEventArgs<ICCardRecord>(null, true, watch.ElapsedMilliseconds));

        }

      

       
    }
}
