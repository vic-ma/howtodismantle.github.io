using Peakboard.ExtensionKit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace PushMessageExtension
{
    [CustomListIcon("PushMessageExtension.PushMessageExtension.png")]
    public class PushMessageCustomList : CustomListBase
    {
        protected override CustomListDefinition GetDefinitionOverride()
        {
            return new CustomListDefinition
            {
                ID = "PushMessageCustomList",
                Name = "Push Message List",
                Description = "A custom list for push messages",
                PropertyInputPossible = true,
                PropertyInputDefaults = { new CustomListPropertyDefinition { Name = "Port", Value = "3112" } },
                SupportsPushOnly = true
            };
        }


        protected override CustomListColumnCollection GetColumnsOverride(CustomListData data)
        {
            var columns = new CustomListColumnCollection();
            columns.Add(new CustomListColumn("TimeStamp", CustomListColumnTypes.String));
            columns.Add(new CustomListColumn("Message", CustomListColumnTypes.String));
            return columns;
        }

        protected override CustomListObjectElementCollection GetItemsOverride(CustomListData data)
        {
            return new CustomListObjectElementCollection();
        }

        static private Timer? _timer;

        protected override void SetupOverride(CustomListData data)
        {
            this.Log.Info("SetupOverride");
            _timer = new Timer(new TimerCallback(OnTimer), data, 0, 1000);
        }

        protected override void CleanupOverride(CustomListData data)
        {
            this.Log.Info("CleanupOverride");
            _timer?.Dispose();
        }

        private void OnTimer(object? state)
        {
            this.Log.Info("OnTimer");

            if (state is CustomListData data)
            {
                var item = new CustomListObjectElement();
                item.Add("TimeStamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                item.Add("Message", "Test Message");
                var items = new CustomListObjectElementCollection();
                items.Add(item);
                this.Data.Push(data.ListName).Add(item);
            }
        }

    }
}
