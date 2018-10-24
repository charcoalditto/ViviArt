using System;
using System.Collections.Generic;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace ViviArt.Droid
{
	[BroadcastReceiver (Label = "TODO")]
	[IntentFilter (new string [] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	[MetaData ("android.appwidget.provider", Resource = "@xml/todo_appwidget")]
    public class TodoProvider : AppWidgetProvider
    {
        public const string MODE_TYPE_DELETE = "MODE_TODO_DELETE";
        public const string MODE_TYPE_CHECK = "MODE_TODO_CHECK";
        public const string MODE_TYPE_EDIT = "MODE_TODO_EDIT";
        public const string MODE_TYPE_ADD = "MODE_TODO_ADD";

        public const string OPEN_TODO_EDIT = "OPEN_TODO_EDIT";
        public const string OPEN_TODO_ADD = "OPEN_TODO_ADD";

        public const string ACTION_TODO_MODE = "ACTION_TODO_MODE";
        public const string ACTION_TODO_LIST_CLICKED = "ACTION_TODO_CLICKED";
        public const string ACTION_TODO_ALARM = "ACTION_TODO_ALARM";

        public const string ACTION_MANDALA_DATE_MENU = "ACTION_MANDALA_DATE_MENU";
        public const string DATE_MENU_REFRESH = "DATE_MENU_REFRESH";

        public const string EXTRA_POSITION = "EXTRA_POSITION";
        public const string EXTRA_ID = "EXTRA_ID";

        public const string PREF_TODAY = "PREF_TODAY";
        public const string PREF_MODE_TYPE = "PREF_MODE_TYPE";

        private static AlarmManager alarmManager;
        private static PendingIntent alarmSender;


        public static Dictionary<string, int> modeResourceMap = new Dictionary<string, int>(){
            {MODE_TYPE_DELETE, Resource.Id.todoBtnDelete},
            {MODE_TYPE_CHECK, Resource.Id.todoBtnCheck},
            {MODE_TYPE_EDIT, Resource.Id.todoBtnEdit},
            {MODE_TYPE_ADD, Resource.Id.todoBtnAdd},
        };

        public static Dictionary<string, int> dateMenuResourceMap = new Dictionary<string, int>(){
            {DATE_MENU_REFRESH, Resource.Id.todoBtnDate},
        };

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            DateTime today = DateTime.Now;
            foreach(int appWidgetId in appWidgetIds)
            {
                SetTodayPref(context, appWidgetId, today);

                RemoteViews remoteViews = GetWidgetRemoteView();
                SetViewListView(context, appWidgetId, remoteViews);
                SetViewMenuView(context, appWidgetId, remoteViews);
                SetViewToday(context, appWidgetId, remoteViews);
                appWidgetManager.PartiallyUpdateAppWidget(appWidgetId, remoteViews);
            }
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            AppWidgetManager mgr = AppWidgetManager.GetInstance(context);
            int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, -1);
            int position = intent.GetIntExtra(EXTRA_POSITION, -1);

            string action = intent.Action;
            switch (action)
            {
                case ACTION_TODO_LIST_CLICKED:
                    OnReceiveClick(context, appWidgetId, intent.Extras);
                    break;
                case ACTION_TODO_MODE:
                    if(intent.GetStringExtra(ACTION_TODO_MODE) == MODE_TYPE_ADD)
                    {
                        AddTodo(context, appWidgetId, intent.Extras);
                    } else
                    {
                        ChangeTodoMode(context, appWidgetId, intent.Extras);    
                    }
                    break;
                case ACTION_TODO_ALARM:
                    OnReceiveAlarm(context);
                    break;
                default:
                    break;
            }
            base.OnReceive(context, intent);
        }
        public void OnReceiveClick(Context context, int appWidgetId, Bundle extras)
        {
            string action = GetModePref(context, appWidgetId);
            switch (action)
            {
                case MODE_TYPE_DELETE:
                    DeleteTodo(context, appWidgetId, extras.GetInt(EXTRA_ID, -1));
                    break;
                case MODE_TYPE_CHECK:
                    CheckTodo(context, appWidgetId, extras.GetInt(EXTRA_ID, -1));
                    break;
                case MODE_TYPE_EDIT:
                    EditTodo(context, appWidgetId, extras.GetInt(EXTRA_ID, -1));
                    break;
            }
        }
        public void OnReceiveAlarm(Context context)
        {
            RemovePreviousAlarm();
            DateTime now = DateTime.Now;
            RegisterNextAlarm(context, now);

            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            ComponentName me = new ComponentName(context, Java.Lang.Class.FromType(typeof(TodoProvider)).Name);
            int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(me);

            foreach (int appWidgetId in appWidgetIds)
            {
                SetTodayPref(context, appWidgetId, now);
                RemoteViews remoteViews = GetWidgetRemoteView();
                SetViewToday(context, appWidgetId, remoteViews);
                appWidgetManager.PartiallyUpdateAppWidget(appWidgetId, remoteViews);
                appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.todoListView);
            }
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
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            Bundle options = appWidgetManager.GetAppWidgetOptions(appWidgetId);
            options.PutString(PREF_MODE_TYPE, modeType);
            appWidgetManager.UpdateAppWidgetOptions(appWidgetId, options);
        }
        public static string GetModePref(Context context, int appWidgetId)
        {
            Bundle options = AppWidgetManager.GetInstance(context).GetAppWidgetOptions(appWidgetId);
            return options.GetString(PREF_MODE_TYPE, MODE_TYPE_CHECK);
        }
        public static void SetTodayPref(Context context, int appWidgetId, DateTime today)
        {
            AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
            Bundle options = appWidgetManager.GetAppWidgetOptions(appWidgetId);
            options.PutString(PREF_TODAY, today.ToString2());
            appWidgetManager.UpdateAppWidgetOptions(appWidgetId, options);
        }
        public static DateTime GetTodayPref(Context context, int appWidgetId)
        {
            Bundle options = AppWidgetManager.GetInstance(context).GetAppWidgetOptions(appWidgetId);
            return options.GetString(PREF_TODAY).ToDateTime2();
        }

        /*
         * 
        ███████╗███████╗████████╗    ███╗   ███╗ ██████╗ ██████╗ ███████╗
        ██╔════╝██╔════╝╚══██╔══╝    ████╗ ████║██╔═══██╗██╔══██╗██╔════╝
        ███████╗█████╗     ██║       ██╔████╔██║██║   ██║██║  ██║█████╗  
        ╚════██║██╔══╝     ██║       ██║╚██╔╝██║██║   ██║██║  ██║██╔══╝  
        ███████║███████╗   ██║       ██║ ╚═╝ ██║╚██████╔╝██████╔╝███████╗
        ╚══════╝╚══════╝   ╚═╝       ╚═╝     ╚═╝ ╚═════╝ ╚═════╝ ╚══════╝
         */


        public void ChangeTodoMode(Context context, int appWidgetId, Bundle extras)
        {
            string tmpModeType = extras.GetString(ACTION_TODO_MODE);
            SetModePref(context, appWidgetId, tmpModeType);
            string modeType = GetModePref(context, appWidgetId);
            RemoteViews remoteViews = GetWidgetRemoteView();
            ChangeButtonsCheck(remoteViews, modeType);
            AppWidgetManager.GetInstance(context).PartiallyUpdateAppWidget(appWidgetId, remoteViews);
        }
        /*
        ██████╗  ██████╗     ██╗████████╗
        ██╔══██╗██╔═══██╗    ██║╚══██╔══╝
        ██║  ██║██║   ██║    ██║   ██║   
        ██║  ██║██║   ██║    ██║   ██║   
        ██████╔╝╚██████╔╝    ██║   ██║   
        ╚═════╝  ╚═════╝     ╚═╝   ╚═╝
        */
        public void DeleteTodo(Context context, int appWidgetId, int todoID)
        {
            TodoItem todo = DatabaseAccess.Current.GetItem<TodoItem>(todoID);
            todo.DeleteDt = DateTime.Now.ToString1();
            DatabaseAccess.Current.SaveItem(todo);
            AppWidgetManager.GetInstance(context).NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.todoListView);
        }
        public void CheckTodo(Context context, int appWidgetId, int todoID)
        {
            TodoItem todo = DatabaseAccess.Current.GetItem<TodoItem>(todoID);
            todo.CompleteDt = (todo.CompleteDt == null)? DateTime.Now.ToString1() : null;
            DatabaseAccess.Current.SaveItem(todo);
            AppWidgetManager.GetInstance(context).NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.todoListView);
        }

        /*
         ██████╗ ██████╗ ███████╗███╗   ██╗    ███████╗ ██████╗██████╗ ███████╗███████╗███╗   ██╗
        ██╔═══██╗██╔══██╗██╔════╝████╗  ██║    ██╔════╝██╔════╝██╔══██╗██╔════╝██╔════╝████╗  ██║
        ██║   ██║██████╔╝█████╗  ██╔██╗ ██║    ███████╗██║     ██████╔╝█████╗  █████╗  ██╔██╗ ██║
        ██║   ██║██╔═══╝ ██╔══╝  ██║╚██╗██║    ╚════██║██║     ██╔══██╗██╔══╝  ██╔══╝  ██║╚██╗██║
        ╚██████╔╝██║     ███████╗██║ ╚████║    ███████║╚██████╗██║  ██║███████╗███████╗██║ ╚████║
         ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═══╝    ╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═══╝
        */

        public void EditTodo(Context context, int appWidgetId, int todoID)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            intent.SetAction(OPEN_TODO_EDIT);
            intent.PutExtra(EXTRA_ID, todoID);


            TodoItemEdit.SuccessCallback = () =>
            {
                AppWidgetManager.GetInstance(context).NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.todoListView);
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().Close();
            };
            context.StartActivity(intent);
        }

        public void AddTodo(Context context, int appWidgetId, Bundle extras)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            intent.SetAction(OPEN_TODO_ADD);
            intent.PutExtras(extras);

            TodoItemEdit.SuccessCallback = () =>
            {
                AppWidgetManager.GetInstance(context).NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.todoListView);
                Xamarin.Forms.DependencyService.Get<ICloseApplication>().Close();
            };
            context.StartActivity(intent);
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
            return new RemoteViews(Android.App.Application.Context.PackageName, Resource.Layout.todo_layout);    
        }
        public void ChangeButtonsCheck(RemoteViews remoteViews, string modeType)
        {
            foreach (KeyValuePair<string, int> pair in modeResourceMap)
                remoteViews.SetInt(pair.Value, "setBackgroundResource", Resource.Color.colorTransparent);

            remoteViews.SetInt(modeResourceMap[modeType], "setBackgroundResource", Resource.Drawable.round_shape);
        }
        private void SetViewToday(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            DateTime today = GetTodayPref(context, appWidgetId);
            remoteViews.SetTextViewText(Resource.Id.todoBtnDate, today.ToString2());
        }
        private void SetViewListView(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            // ㅇㅣ 부분에서 TodoService가 생성되는게 아니라 앱위젯매니저에서 관리하는듯
            Intent svcIntent = new Intent(context, typeof(TodoService));
            svcIntent.SetPackage(context.PackageName);
            svcIntent.SetAction(ACTION_TODO_LIST_CLICKED);
            svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(Android.Content.IntentUriType.AndroidAppScheme)));
            svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            remoteViews.SetEmptyView(Resource.Id.todoListView, Resource.Id.todoEmptyView);
            remoteViews.SetRemoteAdapter(Resource.Id.todoListView, svcIntent);

            // 리스트에 클릭을 추가할 준비
            Intent toastIntent = new Intent(context, typeof(TodoProvider));
            toastIntent.SetAction(ACTION_TODO_LIST_CLICKED);
            toastIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            PendingIntent toastPendingIntent = PendingIntent.GetBroadcast(context, 0, toastIntent, PendingIntentFlags.UpdateCurrent);
            remoteViews.SetPendingIntentTemplate(Resource.Id.todoListView, toastPendingIntent);
        }
        private void SetViewMenuView(Context context, int appWidgetId, RemoteViews remoteViews)
        {
            // + 버튼에 액션 추가
            string modeType = GetModePref(context, appWidgetId);
            ChangeButtonsCheck(remoteViews, modeType);

            foreach (KeyValuePair<string, int> pair in modeResourceMap)
                remoteViews.SetOnClickPendingIntent(pair.Value, GetButtonPendingIntent(context, pair.Value, pair.Key, appWidgetId));
        }

        private PendingIntent GetButtonPendingIntent(Context context, int requestCode, string modeType, int appWidgetId)
        {
            Intent intent = new Intent(context, typeof(TodoProvider));
            intent.SetAction(ACTION_TODO_MODE);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            intent.PutExtra(ACTION_TODO_MODE, modeType);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, requestCode, intent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }


        public void RegisterNextAlarm(Context context, DateTime now)
        {
            var next = now.Date.AddDays(1);
            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long firstTime = (long)(next.ToUniversalTime() - unixTime).TotalMilliseconds;

            Intent intent = new Intent(context, typeof(TodoProvider));
            intent.SetAction(ACTION_TODO_ALARM);

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
            RegisterNextAlarm(context, DateTime.Now);
            base.OnEnabled(context);
        }

        public override void OnDisabled(Context context)
        {
            RemovePreviousAlarm();
            base.OnDisabled(context);
        }
    }
}
