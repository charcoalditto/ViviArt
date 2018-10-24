using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DateTimeExtensions;

namespace ViviArt.Droid
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    public class TodoService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new TodoListFactory(ApplicationContext, intent);
        }
    }

    public class TodoListFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private List<TodoItem> listItemList = new List<TodoItem>();
        private Context context;
        private Intent intent;


        public TodoListFactory(Context contextNew, Intent intentNew)
        {
            context = contextNew;
            intent = intentNew;
            LoadData();
        }


        private void LoadData()
        {
            // 완료일이 이번달이거나
            // 완료하지 않고 // 만료일이 없는
            // 완료하지 않고 // 현재가 만료일보다 작은
            lock (GlobalResources.Current.dbLocker)
            {
                DateTime now = DateTime.Now.Date;
                DateTime firstDay = now.FirstDayOfTheMonth();
                DateTime nextMonth = firstDay.AddMonths(1);

                string query = "SELECT * FROM [TodoItem]";
                query += " WHERE [DeleteDt] IS NULL";

                string query1 = query + " AND [NoExpiryDt] = 1";
                query1 += " AND [CompleteDt] IS NULL ";

                string query2 = query + " AND [NoExpiryDt] = 1";
                query2 += " AND [CompleteDt] >= ? ";
                query2 += " AND [CompleteDt] < ?";

                string query3 = query + " AND [NoExpiryDt] = 0";
                query3 += " AND [ExpiryDt] >= ?";

                List<TodoItem> tmpList = new List<TodoItem>();

                tmpList.AddRange(GlobalResources.Current.database.Query<TodoItem>(
                    query1
                ));
                tmpList.AddRange(GlobalResources.Current.database.Query<TodoItem>(
                    query2,
                    firstDay.ToString1(),
                    nextMonth.ToString1()
                ));
                tmpList.AddRange(GlobalResources.Current.database.Query<TodoItem>(
                    query3,
                    now.ToString1()
                ));

                var tmpQuery =
                    from it in tmpList
                    orderby it.CompleteDt ascending, it.ID descending
                    select it;

                listItemList.Clear();
                listItemList.AddRange(tmpQuery);
            }
        }

        public int Count { get { return listItemList.Count; } }


        public long GetItemId(int position)
        {
            return position;
        }

        public RemoteViews GetViewAt(int position)
        {
            RemoteViews remoteView = new RemoteViews(context.PackageName, Resource.Layout.todo_row);
            TodoItem listItem = listItemList[position];
            remoteView.SetTextViewText(Resource.Id.todoTitle, $"{listItem.ID}.  {listItem.Title}");
            Bundle extras = new Bundle();
            extras.PutInt(TodoProvider.EXTRA_POSITION, position);
            extras.PutInt(TodoProvider.EXTRA_ID, listItem?.ID ?? -1);
            intent.PutExtras(extras);
            remoteView.SetViewVisibility(Resource.Id.todoRedLine, (listItem.CompleteDt != null) ? ViewStates.Visible : ViewStates.Gone);
            remoteView.SetOnClickFillInIntent(Resource.Id.todoRow, intent);
            return remoteView;
        }


        public RemoteViews LoadingView { get { return null; } }

        public int ViewTypeCount { get { return 1; } }

        public bool HasStableIds { get { return true; } }


        public void OnCreate()
        {
            // throw new NotImplementedException();
        }

        public void OnDataSetChanged()
        {
            LoadData();
        }

        public void OnDestroy()
        {
            // throw new NotImplementedException();
        }

    }
}