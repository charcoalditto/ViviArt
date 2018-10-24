using System;
namespace ViviArt
{
    public static class TestData
    {
        public static void InsertTestDataStat()
        {
            var cg = CoreGoal.GetItem(0);
            var mg1 = MiddleGoal.GetItem((int)cg.ID, 0);
            var mg2 = MiddleGoal.GetItem((int)cg.ID, 1);
            var mg3 = MiddleGoal.GetItem((int)cg.ID, 2);

            foreach (DateTime statDt in DateTimeLib.StatDtRange(DateType.Day, new DateTime(2018, 9, 18), new DateTime(2018, 10, 19)))
            {
                MandalaArtStatistics.MakeStatSet((int)mg1.ID, statDt, true);
                MandalaArtStatistics.MakeStatSet((int)mg2.ID, statDt, true);
                MandalaArtStatistics.MakeStatSet((int)mg3.ID, statDt, true);
            }
        }

        public static void InsertTestDataGoal()
        {
            for (int i = 0; i < 5; i++)
            {
                var core0 = new CoreGoal();
                core0.Title = $"핵심목표 {i}";
                core0.Position = i;
                DatabaseAccess.Current.SaveItem<CoreGoal>(core0);
                var cg = CoreGoal.GetItem(i);
                for (int j = 0; j < 4; j++)
                {
                    var middle0 = new MiddleGoal();
                    middle0.Title = $"중간목표 {i}/{j}";
                    middle0.CoreGoalID = (int)cg.ID;
                    middle0.Position = j;
                    int middleID = DatabaseAccess.Current.SaveItem<MiddleGoal>(middle0);
                }
            }

        }
    }
}
