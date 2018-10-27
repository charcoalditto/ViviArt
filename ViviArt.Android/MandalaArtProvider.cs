using System;
using System.Collections.Generic;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace ViviArt.Droid
{
    [BroadcastReceiver(Label = "MandalaArt")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/mandala_appwidget")]
    public class MandalaArtProvider : AppWidgetProvider
    {
        public const string EXTRA_CORE_POSITION = "EXTRA_CORE_POSITION";
        public const string EXTRA_CORE_ID = "EXTRA_CORE_ID";
        public const string EXTRA_MIDDLE_POSITION = "EXTRA_MIDDLE_POSITION";
        public const string EXTRA_MIDDLE_ID = "EXTRA_MIDDLE_ID";

        public const string ACTION_MANDALA_ROW_CLICKED = "ACTION_MANDALA_ROW_CLICKED";
        public const string ACTION_MANDALA_ALARM = "ACTION_MANDALA_ALARM";
        public const string ACTION_MANDALA_DATE_TYPE = "ACTION_MANDALA_DATE_TYPE";
        public const string ACTION_MANDALA_MODE = "ACTION_MANDALA_MODE";
        public const string ACTION_MANDALA_DATE_MENU = "ACTION_MANDALA_DATE_MENU";
        public const string ACTION_MANDALA_REFRESH_FONTSIZE = "ACTION_MANDALA_REFRESH_FONTSIZE";
        public const string ACTION_MANDALA_IMPORT = "ACTION_MANDALA_IMPORT";

        public const string PREF_TODAY = "PREF_TODAY";
        public const string PREF_MODE = "PREF_MODE";
        public const string PREF_DATE_TYPE = "PREF_DATE_TYPE";

        public const string MODE_TYPE_DELETE = "MODE_DELETE";
        public const string MODE_TYPE_CHECK = "MODE_CHECK";
        public const string MODE_TYPE_EDIT = "MODE_EDIT";
        public const string MODE_TYPE_CHART = "MODE_CHART";

        public const string DATE_MENU_REFRESH = "DATE_MENU_REFRESH";
        public const string DATE_MENU_BEFORE = "DATE_MENU_BEFORE";
        public const string DATE_MENU_NEXT = "DATE_MENU_NEXT";

        public const string OPEN_MANDALA_CORE_EDIT = "OPEN_MANDALA_CORE_EDIT";
        public const string OPEN_MANDALA_CORE_ADD = "OPEN_MANDALA_CORE_ADD";
        public const string OPEN_MANDALA_CORE_CHART = "OPEN_MANDALA_CORE_CHART";
        public const string OPEN_MANDALA_MIDDLE_EDIT = "OPEN_MANDALA_MIDDLE_EDIT";
        public const string OPEN_MANDALA_MIDDLE_ADD = "OPEN_MANDALA_MIDDLE_ADD";
        public const string OPEN_MANDALA_MIDDLE_CHART = "OPEN_MANDALA_MIDDLE_CHART";

        public const string EXTRA_IS_CORE_GOAL = "EXTRA_GOALTYPE";
        public const string EXTRA_COUNT = "EXTRA_COUNT";
        public const string EXTRA_TODAY = "EXTRA_TODAY";

        public const string MANDALA_GOALTYPE_MIDDLE = "MANDALA_MIDDLE";

        public static Dictionary<string, int> modeResourceMap = new Dictionary<string, int>()
        {
            {MODE_TYPE_DELETE, Resource.Id.mandalaBtnDelete},
            {MODE_TYPE_CHECK, Resource.Id.mandalaBtnCheck},
            {MODE_TYPE_EDIT, Resource.Id.mandalaBtnEdit},
            {MODE_TYPE_CHART, Resource.Id.mandalaBtnChart}
        };
        public static Dictionary<string, int> dateTypeResourceMap = new Dictionary<string, int>()
        {
            {DateType.Day, Resource.Id.mandalaBtnDay},
            {DateType.Week, Resource.Id.mandalaBtnWeek},
            {DateType.Month, Resource.Id.mandalaBtnMonth},
            {DateType.Year, Resource.Id.mandalaBtnYear},
        };
        public static Dictionary<string, int> dateMenuResourceMap = new Dictionary<string, int>()
        {
            {DATE_MENU_REFRESH, Resource.Id.mandalaBtnDate},
            {DATE_MENU_BEFORE, Resource.Id.mandalaBtnDateBefore},
            {DATE_MENU_NEXT, Resource.Id.mandalaBtnDateNext},
        };

        private static AlarmManager alarmManager;
        private static PendingIntent alarmSender;

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            DateTime today = DateTime.Now.Date;

            foreach (int appWidgetId in appWidgetIds)
            {
                SetModePref(context, appWidgetId, MODE_TYPE_CHECK);
                SetDateTypePref(context, appWidgetId, DateType.Day);
                SetTodayPref(context, appWidgetId, today);

                RemoteViews remoteViews = GetWidgetRemoteView();
                // 메뉴 클릭 이벤트
                SetIntentMode(context, appWidgetId, remoteViews);
                SetIntentDateType(context, appWidgetId, remoteViews);
                SetIntentDateMenu(context, appWidgetId, remoteViews);
                SetIntentRows(context, appWidgetId, remoteViews);
                //
                SetViewDateTypeCheck(context, appWidgetId, remoteViews);
                SetViewModeCheck(context, appWidgetId, remoteViews);
                SetViewDateText(context, appWidgetId, remoteViews);
                SetViewRows(context, appWidgetId, remoteViews);

                SetViewCellFontSize(context, appWidgetId, remoteViews);

                appWidgetManager.PartiallyUpdateAppWidget(appWidgetId, remoteViews);
            }
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }


        public override void OnReceive(Context context, Intent intent)
        {
            AppWidgetManager mgr = AppWidgetManager.GetInstance(context);
            int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, -1);

            string action = intent.Action;
            switch (action)
            {
                case ACTION_MANDALA_ROW_CLICKED:
                    OnReceiveRowClick(context, appWidgetId, intent.GetIntExtra(EXTRA_CORE_POSITION, -1), intent.GetIntExtra(EXTRA_MIDDLE_POSITION, -1));
                    break;
                case ACTION_MANDALA_MODE:
                    SetModePref(context, appWidgetId, intent.GetStringExtra(ACTION_MANDALA_MODE));
                    ChangeMode(context, appWidgetId);
                    break;
                case ACTION_MANDALA_DATE_TYPE:
                    SetDateTypePref(context, appWidgetId, intent.GetStringExtra(ACTION_MANDALA_DATE_TYPE));
                    UpdateMandalaColors(context, appWidgetId);
                    break;
                case ACTION_MANDALA_ALARM:
                    OnReceiveAlarm(context);
                    break;
                case ACTION_MANDALA_DATE_MENU:
                    ChangeTodayPref(context, appWidgetId, intent.GetStringExtra(ACTION_MANDALA_DATE_MENU));
                    UpdateMandalaColors(context, appWidgetId);
                    break;
                case ACTION_MANDALA_REFRESH_FONTSIZE:
                    RefreshFontSize(context);
                    break;
                case ACTION_MANDALA_IMPORT:
                    ImportedMandala(context);
                    break;
                default:
                    break;
            }

            base.OnReceive(context, intent);
        }

        /* 
        ██████╗ ██████╗ ███████╗███████╗███████╗██████╗ ███████╗███╗   ██╗ ██████╗███████╗███████╗
        ██╔══██╗██╔══██╗██╔════╝██╔════╝██╔════╝██╔══██╗██╔════╝████╗  ██║██╔════╝██╔════╝██╔════╝
        ██████╔╝██████╔╝█████╗  █████╗  █████╗  ██████╔╝█████╗  ██╔██╗ ██║██║     █████╗  ███████╗
        ██╔═══╝ ██╔══██╗██╔══╝  ██╔══╝  ██╔══╝  ██╔══██╗██╔══╝  ██║╚██╗██║██║     ██╔══╝  ╚════██║
        ██║     ██║  ██║███████╗██║     ███████╗██║  ██║███████╗██║ ╚████║╚██████╗███████╗███████║
        ╚═╝     ╚═╝  ╚═╝╚══════╝╚═╝     ╚══════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═══╝ ╚═════╝╚══════╝╚══════╝
        설정을 저장해놓고 버튼 UI를 바꾼다 */
        public static void SetModePref(Context context, int appWidgetId, string modeType)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString($"{PREF_MODE}/{appWidgetId}", modeType);
            editor.Commit();
        }
        public static string GetModePref(Context context, int appWidgetId)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            return prefs.GetString($"{PREF_MODE}/{appWidgetId}", MODE_TYPE_CHECK);
        }
        public static void SetDateTypePref(Context context, int appWidgetId, string dateType)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString($"{PREF_DATE_TYPE}/{appWidgetId}", dateType);
            editor.Commit();
        }
        public static string GetDateTypePref(Context context, int appWidgetId)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            return prefs.GetString($"{PREF_DATE_TYPE}/{appWidgetId}", DateType.Day);
        }
        public static void SetTodayPref(Context context, int appWidgetId, DateTime today)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString($"{PREF_TODAY}/{appWidgetId}", today.ToString2());
            editor.Commit();
        }
        public static DateTime GetTodayPref(Context context, int appWidgetId)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            return prefs.GetString($"{PREF_TODAY}/{appWidgetId}", DateTime.Now.ToString2()).ToDateTime2();
        }

        /*
         ██████╗ ██████╗ ███████╗███╗   ██╗    ███████╗ ██████╗██████╗ ███████╗███████╗███╗   ██╗
        ██╔═══██╗██╔══██╗██╔════╝████╗  ██║    ██╔════╝██╔════╝██╔══██╗██╔════╝██╔════╝████╗  ██║
        ██║   ██║██████╔╝█████╗  ██╔██╗ ██║    ███████╗██║     ██████╔╝█████╗  █████╗  ██╔██╗ ██║
        ██║   ██║██╔═══╝ ██╔══╝  ██║╚██╗██║    ╚════██║██║     ██╔══██╗██╔══╝  ██╔══╝  ██║╚██╗██║
        ╚██████╔╝██║     ███████╗██║ ╚████║    ███████║╚██████╗██║  ██║███████╗███████╗██║ ╚████║
         ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═══╝    ╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═══╝
        */
        ////Toast.MakeText(context, "핵심목표를 먼저 만들어 주세요", ToastLength.Short).Show();

        public void OpenMandalaChart(Context context, int appWidgetId, int corePosition, int middlePosition)
        {
            Intent intent = new Intent(context, typeof(MainActivity));

            var cg = CoreGoal.GetItem(corePosition);
            var mg = MiddleGoal.GetItem(cg?.ID ?? -1, middlePosition);
            var today = GetTodayPref(context, appWidgetId);
            intent.PutExtra(EXTRA_MIDDLE_ID, mg?.ID ?? -1);
            intent.PutExtra(EXTRA_CORE_ID, cg?.ID ?? -1);
            intent.PutExtra(EXTRA_MIDDLE_POSITION, middlePosition);
            intent.PutExtra(EXTRA_CORE_POSITION, corePosition);
            intent.PutExtra(EXTRA_TODAY, today.ToString2());

            if (IsCoreGoal(middlePosition))
            {
                if (cg == null)
                {
                    Toast.MakeText(context, "핵심목표가 없습니다", ToastLength.Short).Show();
                    return;
                } 
                else
                {
                    intent.SetAction(OPEN_MANDALA_CORE_CHART);
                }
            } 
            else
            {
                if (mg == null)
                {
                    Toast.MakeText(context, "중간목표가 없습니다", ToastLength.Short).Show();
                    return;
                }
                else
                {
                    intent.SetAction(OPEN_MANDALA_MIDDLE_CHART);
                }
            }
            context.StartActivity(intent);
        }
        public void PartiallyReloadRow(Context context, int appWidgetId, int corePosition, int middlePosition)
        {
            var remoteViews = GetWidgetRemoteView();
            var today = GetTodayPref(context, appWidgetId);
            SetGoal(context, appWidgetId, remoteViews, corePosition, middlePosition, today);
            AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
        }
        public void EditMandala(Context context, int appWidgetId, int corePosition, int middlePosition)
        {
            Intent intent = new Intent(context, typeof(MainActivity));

            var cg = CoreGoal.GetItem(corePosition);
            var mg = MiddleGoal.GetItem(cg?.ID ?? -1, middlePosition);
            var today = GetTodayPref(context, appWidgetId);
            intent.PutExtra(EXTRA_MIDDLE_ID, mg?.ID ?? -1);
            intent.PutExtra(EXTRA_CORE_ID, cg?.ID ?? -1);
            intent.PutExtra(EXTRA_MIDDLE_POSITION, middlePosition);
            intent.PutExtra(EXTRA_CORE_POSITION, corePosition);
            intent.PutExtra(EXTRA_TODAY, today.ToString2());

            if (IsCoreGoal(middlePosition))
            {
                if (cg == null)
                {
                    intent.SetAction(OPEN_MANDALA_CORE_ADD);
                }
                else
                {
                    intent.SetAction(OPEN_MANDALA_CORE_EDIT);
                }
                MandalaCoreEdit.SuccessCallback = () =>
                {
                    PartiallyReloadRow(context, appWidgetId, corePosition, middlePosition);
                    Xamarin.Forms.DependencyService.Get<ICloseApplication>().Close();
                };
                context.StartActivity(intent);
            }
            else
            {
                if (cg == null)
                {
                    Toast.MakeText(context, "핵심목표를 먼저 만들어주세요", ToastLength.Short).Show();
                    return;
                }
                if (mg == null)
                {
                    intent.SetAction(OPEN_MANDALA_MIDDLE_ADD);
                } 
                else
                {
                    intent.SetAction(OPEN_MANDALA_MIDDLE_EDIT);
                }

                MandalaMiddleEdit.SuccessCallback = () =>
                {
                    PartiallyReloadRow(context, appWidgetId, corePosition, middlePosition);
                    Xamarin.Forms.DependencyService.Get<ICloseApplication>().Close();
                };
                context.StartActivity(intent);
            }
        }
        /*
        ██████╗  ██████╗     ██╗████████╗
        ██╔══██╗██╔═══██╗    ██║╚══██╔══╝
        ██║  ██║██║   ██║    ██║   ██║   
        ██║  ██║██║   ██║    ██║   ██║   
        ██████╔╝╚██████╔╝    ██║   ██║   
        ╚═════╝  ╚═════╝     ╚═╝   ╚═╝
        */
        public void DeleteMiddleGoal(int middleGoalId)
        {
            //TODO: 삭제확인 팝업 띄우
            MiddleGoal goal = DatabaseAccess.Current.GetItem<MiddleGoal>(middleGoalId);
            goal.DeleteDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DatabaseAccess.Current.SaveItem(goal);
        }
        public void DeleteCoreGoal(int coreGoalId)
        {
            //TODO: 삭제확인 팝업 띄우
            CoreGoal goal = DatabaseAccess.Current.GetItem<CoreGoal>(coreGoalId);
            goal.DeleteDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DatabaseAccess.Current.SaveItem(goal);
        }
        public void DeleteMandala(Context context, int appWidgetId, int corePosition, int middlePosition)
        {
            var cg = CoreGoal.GetItem(corePosition);
            var mg = MiddleGoal.GetItem(cg?.ID ?? -1, middlePosition);

            if (IsCoreGoal(middlePosition))
            {
                if (cg == null)
                {
                    Toast.MakeText(context, "삭제할게 없습니다", ToastLength.Short).Show();
                    return;
                }
                DeleteCoreGoal((int)cg.ID);
                for (int j = 0; j < 9; j++)
                {
                    PartiallyReloadRow(context, appWidgetId, corePosition, j);
                }
            }
            else
            {
                if (mg == null)
                {
                    Toast.MakeText(context, "삭제할게 없습니다", ToastLength.Short).Show();
                    return;
                }
                DeleteMiddleGoal((int)mg.ID);
                PartiallyReloadRow(context, appWidgetId, corePosition, middlePosition);
            }

        }

        public void CheckMandala(Context context, int appWidgetId, int corePosition, int middlePosition)
        {
            string dateType = GetDateTypePref(context, appWidgetId);
            var cg = CoreGoal.GetItem(corePosition);
            var mg = MiddleGoal.GetItem(cg?.ID ?? -1, middlePosition);

            if (dateType != DateType.Day)
            {
                Toast.MakeText(context, "체크는 일간만달라로 ~...", ToastLength.Short).Show();
                SetDateTypePref(context, appWidgetId, DateType.Day);
                UpdateMandalaColors(context, appWidgetId);
                return;
            }


            if(IsCoreGoal(middlePosition))
            {
                Toast.MakeText(context, "핵심목표는 체크가 안됩니당", ToastLength.Short).Show();
                return;
            }
            else
            {
                if (mg == null)
                {
                    Toast.MakeText(context, "중간목표가 없습니다", ToastLength.Short).Show();
                    return;
                }
                Toast.MakeText(context, "중간목표 클릭!", ToastLength.Short).Show();

                DateTime today = GetTodayPref(context, appWidgetId);
                var stat = MandalaArtStatistics.GetItem((int)mg.ID, DateType.Day, today.Date);
                MandalaArtStatistics.MakeStatSet((int)mg.ID, today, (stat?.Count ?? 0) == 0);
            } 
            PartiallyReloadRow(context, appWidgetId, corePosition, middlePosition);
        }
        /*
         ██████╗ ███╗   ██╗██████╗ ███████╗ ██████╗███████╗██╗██╗   ██╗███████╗
        ██╔═══██╗████╗  ██║██╔══██╗██╔════╝██╔════╝██╔════╝██║██║   ██║██╔════╝
        ██║   ██║██╔██╗ ██║██████╔╝█████╗  ██║     █████╗  ██║██║   ██║█████╗  
        ██║   ██║██║╚██╗██║██╔══██╗██╔══╝  ██║     ██╔══╝  ██║╚██╗ ██╔╝██╔══╝  
        ╚██████╔╝██║ ╚████║██║  ██║███████╗╚██████╗███████╗██║ ╚████╔╝ ███████╗
         ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝╚══════╝ ╚═════╝╚══════╝╚═╝  ╚═══╝  ╚══════╝
        */
        public void OnReceiveRowClick(Context context, int appWidgetId, int corePosition, int middlePosition)
        {
            string clickType = GetModePref(context, appWidgetId);

            switch (clickType)
            {
                case MODE_TYPE_EDIT:
                    EditMandala(context, appWidgetId, corePosition, middlePosition);
                    break;
                case MODE_TYPE_CHECK:
                    CheckMandala(context, appWidgetId, corePosition, middlePosition);
                    break;
                case MODE_TYPE_DELETE:
                    DeleteMandala(context, appWidgetId, corePosition, middlePosition);
                    break;
                case MODE_TYPE_CHART:
                    OpenMandalaChart(context, appWidgetId, corePosition, middlePosition);
                    break;
            }
        }

        public void OnReceiveAlarm(Context context)
        {
            DateTime today = DateTime.Now.Date;
            // 알람을 새로 만든다
            RemovePreviousAlarm();
            RegisterNextAlarm(context, today.AddDays(1));

            // 마지막통계 생성일 ~ 오늘의 새 통계를 만든다.
            DateTime lastStatDt = Setting.ValueOf(SettingKey.MandalaStaticsUpdateDate)?.ToDateTime2() ?? today;

            foreach(var tmpDate in DateTimeLib.StatDtRange(DateType.Day, lastStatDt, today.AddDays(1)))
            {
                MandalaArtStatistics.MakeStatAll(tmpDate);
                Setting.Save(SettingKey.MandalaStaticsUpdateDate, tmpDate.ToString2());
            }
            // 통계를 새로 만들었으니까 앱을 업데이트 하자
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            ComponentName me = new ComponentName(context, Java.Lang.Class.FromType(typeof(MandalaArtProvider)).Name);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(me);
            foreach (int appWidgetId in appWidgetIds)
            {
                SetTodayPref(context, appWidgetId, lastStatDt);
                UpdateMandalaColors(context, appWidgetId);
            }
        }

        public void ChangeMode(Context context, int appWidgetId)
        {
            RemoteViews remoteViews = GetWidgetRemoteView();
            SetViewModeCheck(context, appWidgetId, remoteViews);
            AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
        }
        public void ChangeTodayPref(Context context, int appWidgetId, string dateMenu)
        {
            DateTime today = GetTodayPref(context, appWidgetId);
            switch(dateMenu)
            {
                case DATE_MENU_NEXT:
                    SetTodayPref(context, appWidgetId, today.AddDays(1));
                    break;    
                case DATE_MENU_BEFORE:
                    SetTodayPref(context, appWidgetId, today.AddDays(-1));
                    break;
                case DATE_MENU_REFRESH:
                    SetTodayPref(context, appWidgetId, today);
                    break;
            }
        }
        public void SetViewRows(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            DateTime today = GetTodayPref(context, appWidgetId);
            string dateType = GetDateTypePref(context, appWidgetId);

            for (int corePosition = 0; corePosition < 9; corePosition++)
            {
                var cg = CoreGoal.GetItem(corePosition);
                for (int middlePosition = 0; middlePosition < 9; middlePosition++)
                {
                    int resourceId = context.Resources.GetIdentifier($"mandalaGoal{corePosition}{middlePosition}", "id", context.PackageName);
                    var mg = MiddleGoal.GetItem(cg?.ID ?? -1, middlePosition);

                    var color = Setting.GetColor(corePosition);

                    float alpha = 1f;
                    if (IsCoreGoal(middlePosition))
                    {
                        remoteViews.SetTextViewText(resourceId, $"{cg?.Title ?? "*"}");
                    }
                    else
                    {
                        remoteViews.SetTextViewText(resourceId, $"{mg?.Title ?? "-"}");
                        alpha = GetGoalAlpha(mg, dateType, today);
                    }
                    remoteViews.SetInt(
                        resourceId,
                        "setBackgroundColor",
                        Xamarin.Forms.Color.FromRgba(color.R, color.G, color.B, alpha).ToAndroid()
                    );
                }
            }
        }

        public void UpdateMandalaColors(Context context, int appWidgetId)
        {
            RemoteViews remoteViews = GetWidgetRemoteView();
            string dateType = GetDateTypePref(context, appWidgetId);
            SetViewDateTypeCheck(context, appWidgetId, remoteViews);
            SetViewDateText(context, appWidgetId, remoteViews);
            SetViewRows(context, appWidgetId, remoteViews);
            AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
        }

        public void RefreshFontSize(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            ComponentName me = new ComponentName(context, Java.Lang.Class.FromType(typeof(MandalaArtProvider)).Name);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(me);

            foreach(int appWidgetId in appWidgetIds)
            {
                RemoteViews remoteViews = GetWidgetRemoteView();
                SetViewCellFontSize(context, appWidgetId, remoteViews);
                AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
            }
        }
        public void ImportedMandala(Context context)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            ComponentName me = new ComponentName(context, Java.Lang.Class.FromType(typeof(MandalaArtProvider)).Name);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(me);
            foreach (int appWidgetId in appWidgetIds)
            {
                RemoteViews remoteViews = GetWidgetRemoteView();
                SetViewRows(context, appWidgetId, remoteViews);
                AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
            }
        }

        /*
        ██████╗ ██╗   ██╗██╗██╗     ██████╗     ██╗   ██╗██╗███████╗██╗    ██╗
        ██╔══██╗██║   ██║██║██║     ██╔══██╗    ██║   ██║██║██╔════╝██║    ██║
        ██████╔╝██║   ██║██║██║     ██║  ██║    ██║   ██║██║█████╗  ██║ █╗ ██║
        ██╔══██╗██║   ██║██║██║     ██║  ██║    ╚██╗ ██╔╝██║██╔══╝  ██║███╗██║
        ██████╔╝╚██████╔╝██║███████╗██████╔╝     ╚████╔╝ ██║███████╗╚███╔███╔╝
        ╚═════╝  ╚═════╝ ╚═╝╚══════╝╚═════╝       ╚═══╝  ╚═╝╚══════╝ ╚══╝╚══╝ 
        */
        public RemoteViews GetWidgetRemoteView()
        {
            return new RemoteViews(Android.App.Application.Context.PackageName, Resource.Layout.mandala_layout);
        }
        private PendingIntent GetMenuPendingIntent(Context context, int requestCode, int appWidgetId, string action, string v)
        {
            Intent intent = new Intent(context, typeof(MandalaArtProvider));
            intent.SetAction(action);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            intent.PutExtra(action, v);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, requestCode, intent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private float GetGoalAlpha(MiddleGoal middleGoal, string dateType, DateTime today)
        {
            if (middleGoal == null)
            {
                return 0f;
            }

            var timeSet = DateTimeLib.StatDtSet(today, dateType);
            DateTime middleGoalCreateDt = middleGoal.CreateDt.ToDateTime1();
            /*
             * 나중에 생성한 목표일경우 그날짜에 맞춰서 색깔을 만들어달라는 요청이 있었으나 
             * 맞지 않는것같으니까 일단 보류
            if (timeSet.StartDt < middleGoalCreateDt)
            {
                timeSet.StartDt = middleGoalCreateDt;
            }*/
            int totalDays = (timeSet.EndDt - timeSet.StartDt).Days;


            MandalaArtStatistics st = MandalaArtStatistics.GetItem((int)middleGoal.ID, dateType, today);

            return (st?.Count ?? 0) * 1f / totalDays;
        }

        public bool IsCoreGoal(int middlePosition) => middlePosition == 4;

        public PendingIntent GetRowPendingIntent(Context context, int requestCode, int appWidgetId, int corePosition, int middlePosition)
        {
            Intent intent = new Intent(context, typeof(MandalaArtProvider));
            intent.SetAction(ACTION_MANDALA_ROW_CLICKED);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            intent.PutExtra(EXTRA_CORE_POSITION, corePosition);
            intent.PutExtra(EXTRA_MIDDLE_POSITION, middlePosition);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, requestCode, intent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private void SetGoal(Context context, int appWidgetId, RemoteViews remoteViews, int corePosition, int middlePosition, DateTime today)
        {
            int resourceId = context.Resources.GetIdentifier($"mandalaGoal{corePosition}{middlePosition}", "id", context.PackageName);

            var dateType = GetDateTypePref(context, appWidgetId);
            var cg = CoreGoal.GetItem(corePosition);
            var mg = MiddleGoal.GetItem(cg?.ID ?? -1, middlePosition);

            var color = Setting.GetColor(corePosition);
            // 타이틀 붙임
            float alpha = 1f;
            if (IsCoreGoal(middlePosition))
            {
                remoteViews.SetTextViewText(resourceId, $"{cg?.Title ?? "*"}");
            }
            else
            {
                remoteViews.SetTextViewText(resourceId, $"{mg?.Title ?? "-"}");
                alpha = GetGoalAlpha(mg, dateType, today);
            }
            remoteViews.SetInt(
                resourceId,
                "setBackgroundColor",
                Xamarin.Forms.Color.FromRgba(color.R, color.G, color.B, alpha).ToAndroid()
            );
        }
        /*
            int i = position / 9;
            int j = position % 9;
            int corePosition = (i / 3) * 3 + (j / 3);
            int middlePosition = (i % 3) * 3 + (j % 3);
        */

        private void SetIntentRows(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            for (int corePosition = 0; corePosition < 9; corePosition++)
            {
                for (int middlePosition = 0; middlePosition < 9; middlePosition++)
                {
                    int resourceId = context.Resources.GetIdentifier($"mandalaGoal{corePosition}{middlePosition}", "id", context.PackageName);
                    remoteViews.SetOnClickPendingIntent(resourceId, GetRowPendingIntent(context, resourceId, appWidgetId, corePosition, middlePosition));
                }
            }
        }
        /*
         * 
        ██╗   ██╗██╗███████╗██╗    ██╗    ██╗   ██╗██╗
        ██║   ██║██║██╔════╝██║    ██║    ██║   ██║██║
        ██║   ██║██║█████╗  ██║ █╗ ██║    ██║   ██║██║
        ╚██╗ ██╔╝██║██╔══╝  ██║███╗██║    ██║   ██║██║
         ╚████╔╝ ██║███████╗╚███╔███╔╝    ╚██████╔╝██║
          ╚═══╝  ╚═╝╚══════╝ ╚══╝╚══╝      ╚═════╝ ╚═╝
         */
        public void SetViewDateText(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            DateTime today = GetTodayPref(context, appWidgetId);
            remoteViews.SetTextViewText(Resource.Id.mandalaBtnDate, today.ToString2());
        }
        public void SetViewDateTypeCheck(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            string dateType = GetDateTypePref(context, appWidgetId);
            foreach (KeyValuePair<string, int> pair in dateTypeResourceMap)
                remoteViews.SetInt(pair.Value, "setBackgroundResource", Resource.Color.colorTransparent);

            remoteViews.SetInt(dateTypeResourceMap[dateType], "setBackgroundResource", Resource.Drawable.round_shape);
        }

        public void SetViewModeCheck(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            string modeType = GetModePref(context, appWidgetId);
            foreach (KeyValuePair<string, int> pair in modeResourceMap)
                remoteViews.SetInt(pair.Value, "setBackgroundResource", Resource.Color.colorTransparent);
            remoteViews.SetInt(modeResourceMap[modeType], "setBackgroundResource", Resource.Drawable.round_shape);
        }

        private void SetIntentDateType(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            // DateType 클릭
            foreach (KeyValuePair<string, int> pair in dateTypeResourceMap)
                remoteViews.SetOnClickPendingIntent(
                    pair.Value, GetMenuPendingIntent(context, pair.Value, appWidgetId, ACTION_MANDALA_DATE_TYPE, pair.Key));
        }

        private void SetIntentMode(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            // Mode 클릭
            foreach (KeyValuePair<string, int> pair in modeResourceMap)
                remoteViews.SetOnClickPendingIntent(
                    pair.Value, GetMenuPendingIntent(context, pair.Value, appWidgetId, ACTION_MANDALA_MODE, pair.Key));
        }

        private void SetIntentDateMenu(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            // Mode 클릭
            foreach (KeyValuePair<string, int> pair in dateMenuResourceMap)
                remoteViews.SetOnClickPendingIntent(
                    pair.Value, GetMenuPendingIntent(context, pair.Value, appWidgetId, ACTION_MANDALA_DATE_MENU, pair.Key));
        }

        public void RegisterNextAlarm(Context context, DateTime next)
        {
            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long firstTime = (long)(next.ToUniversalTime() - unixTime).TotalMilliseconds;

            Intent intent = new Intent(context, typeof(MandalaArtProvider));
            intent.SetAction(ACTION_MANDALA_ALARM);

            alarmSender = PendingIntent.GetBroadcast(context, 0, intent, 0);
            alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.RtcWakeup, firstTime, alarmSender);

            DateTime unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        }

        public void RemovePreviousAlarm()
        {
            if (alarmManager != null && alarmSender != null)
            {
                alarmSender.Cancel();
                alarmManager.Cancel(alarmSender);
            }
        }
        public override void OnEnabled(Context context)
        {
            RemovePreviousAlarm();
            RegisterNextAlarm(context, DateTime.Now.Date.AddDays(1));
            base.OnEnabled(context);
        }
        public override void OnDisabled(Context context)
        {
            RemovePreviousAlarm();
            base.OnDisabled(context);
        
        }
        public void SetViewCellFontSize(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            Bundle options = appWidgetManager.GetAppWidgetOptions(appWidgetId);

            int minWidth = options.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
            int minHeight = options.GetInt(AppWidgetManager.OptionAppwidgetMinHeight);
            float fontSize = (minWidth * minHeight) / 13000;
            float ratio = float.Parse(Setting.ValueOf(SettingKey.MandalaHomeWidgetFontSize) ?? "1.0");;
            fontSize = fontSize * ratio;

            for (int corePosition = 0; corePosition < 9; corePosition++)
            {
                for (int middlePosition = 0; middlePosition < 9; middlePosition++)
                {
                    int resourceId = context.Resources.GetIdentifier($"mandalaGoal{corePosition}{middlePosition}", "id", context.PackageName);
                    remoteViews.SetFloat(resourceId, "setTextSize", fontSize);
                }
            }
        }

        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            RemoteViews remoteViews = GetWidgetRemoteView();
            SetViewCellFontSize(context, appWidgetId, remoteViews);
            AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
            base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
        }
    }
}

