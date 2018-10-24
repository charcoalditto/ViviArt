using SQLite;
using System;
using DateTimeExtensions;
using System.Collections.Generic;

namespace ViviArt
{
    public class MandalaArtStatistics: DefaultModel
    {
        [Indexed]
        public int MiddleGoalID { get; set; }
        public string DateType { get; set; }
        public int Count { get; set; }
        public DateTime StatDt { get; set; }

        public static MandalaArtStatistics GetItem(int middleGoalID, string dateType, DateTime executeDt) {
            foreach(MandalaArtStatistics it in GetItems(middleGoalID, dateType, executeDt))
            {
                return it;
            }
            return null;
        }
        public static IEnumerable<MandalaArtStatistics> GetItems(int middleGoalID, string dateType, DateTime executeDt)
        {
            DateTime statDt = DateTimeLib.StatDt(executeDt, dateType);
            string query = "SELECT * FROM [MandalaArtStatistics]";
            query += " WHERE [StatDt] = ? AND [MiddleGoalID] = ? AND DateType = ?";
            lock (GlobalResources.Current.dbLocker)
            {
                return GlobalResources.Current.database.Query<MandalaArtStatistics>(query, statDt, middleGoalID, dateType).ToArray();
            }
        }
        // TODO: 지우기
        public static List<Tuple<DateTime, MandalaArtStatistics>> GetItems(int middleGoalID, string dateType, DateTime startDt, DateTime endDt)
        {
            List<Tuple<DateTime, MandalaArtStatistics>> result = new List<Tuple<DateTime, MandalaArtStatistics>>();
            foreach(DateTime statDt in DateTimeLib.StatDtRange(dateType, startDt, endDt))
            {
                var item = GetItem(middleGoalID, dateType, statDt);
                result.Add(new Tuple<DateTime, MandalaArtStatistics>(statDt, item));
            }
            return result;
        }

        public static void MakeStatSet(int middleGoalID, DateTime today, bool check)
        {
            MiddleGoal m = DatabaseAccess.Current.GetItem<MiddleGoal>(middleGoalID);
            MandalaArtStatistics stat = MandalaArtStatistics.GetItem(middleGoalID, ViviArt.DateType.Day, today);
            if (check)
            {
                if (stat == null)
                {
                    stat = new MandalaArtStatistics();
                }

                stat.MiddleGoalID = (int)m.ID;
                stat.DateType = ViviArt.DateType.Day;
                stat.Count = 1;
                stat.StatDt = DateTimeLib.StatDt(today, ViviArt.DateType.Day);
                DatabaseAccess.Current.SaveItem(stat);
            }
            else
            {
                if (stat != null)
                {
                    DatabaseAccess.Current.DeleteItem(stat);
                }
                else
                {
                    return;
                }
            }
            MakeStat(middleGoalID, ViviArt.DateType.Week, today);  // 주간 통계 재생성
            MakeStat(middleGoalID, ViviArt.DateType.Month, today);  // 월간 통계 재생성
            MakeStatYear(middleGoalID, today);  // 연간 통계 재생성
        }
        public static void MakeStat(int middleGoalID, string dateType, DateTime today)
        {
            var timeSet = today.StatDtSet(dateType);
            MiddleGoal m = DatabaseAccess.Current.GetItem<MiddleGoal>(middleGoalID);

            var stat= GetItem(middleGoalID, dateType, timeSet.StartDt);
            if (stat == null)
                stat = new MandalaArtStatistics();
            stat.StatDt = timeSet.StartDt;
            stat.DateType = dateType;    
            stat.MiddleGoalID = middleGoalID;

            string query = "select count(*)" +
                "from [MandalaArtStatistics] " +
                "where StatDt >= ? AND StatDt < ? AND DateType = \"D\" AND Count = 1 AND MiddleGoalID = ? AND DeleteDt is null ";
            int cnt = GlobalResources.Current.database.ExecuteScalar<int>(query, timeSet.StartDt, timeSet.EndDt, middleGoalID);
            stat.Count = cnt;
            DatabaseAccess.Current.SaveItem(stat);
        }

        public static void MakeStatYear(int middleGoalID, DateTime today)
        {
            var dateType = ViviArt.DateType.Year;
            var timeSet = today.StatDtSet(dateType);
            MiddleGoal m = DatabaseAccess.Current.GetItem<MiddleGoal>(middleGoalID);

            var stat = GetItem(middleGoalID, dateType, timeSet.StartDt);
            if (stat == null)
                stat = new MandalaArtStatistics();
            stat.StatDt = timeSet.StartDt;
            stat.DateType = dateType;
            stat.MiddleGoalID = middleGoalID;

            string query = "select sum([Count])" +
                "from [MandalaArtStatistics] " +
                "where [StatDt] >= ? AND [StatDt] < ? AND [DateType] = \"M\" AND MiddleGoalID = ? AND DeleteDt is null ";
            int cnt = GlobalResources.Current.database.ExecuteScalar<int>(query, timeSet.StartDt, timeSet.EndDt, middleGoalID);
            stat.Count = cnt;
            DatabaseAccess.Current.SaveItem(stat);
        }

        public static void MakeStatAll(DateTime today)
        {
            foreach(MiddleGoal mg in DatabaseAccess.Current.GetItems<MiddleGoal>())
            {
                MakeStat((int)mg.ID, ViviArt.DateType.Week, today);
                MakeStat((int)mg.ID, ViviArt.DateType.Month, today);
                MakeStatYear((int)mg.ID, today);
            }
        }
    }
}
