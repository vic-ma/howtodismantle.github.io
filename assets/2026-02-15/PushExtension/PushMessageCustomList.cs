using Peakboard.ExtensionKit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace PushExtension
{
    [CustomListIcon("PushExtension.PushExtension.png")]
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
                PropertyInputDefaults = { new CustomListPropertyDefinition { Name = "MyMessages", Value = "Today is a great day to start something new.\nKeep going — you're closer than you think.\nSmall steps lead to big results.\nBelieve in the process — you're on track.", TypeDefinition = TypeDefinition.String.With(multiLine: true) } },
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

        private Timer? _timer;

        protected override void SetupOverride(CustomListData data)
        {
            this.Log.Info("Initializing...");
            _timer = new Timer(new TimerCallback(OnTimer), data, 0, 1000);
        }

        protected override void CleanupOverride(CustomListData data)
        {
            this.Log.Info("Cleaning up....");
            _timer?.Dispose();
        }

        private void OnTimer(object? state)
        {
            this.Log.Info("OnTimer");

            if (state is CustomListData data)
            {
                var item = new CustomListObjectElement();
                item.Add("TimeStamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var MyMessages = data.Properties["MyMessages"].Split('\n');
                var random = new Random();
                item.Add("Message", MyMessages[random.Next(MyMessages.Length)]);
                var items = new CustomListObjectElementCollection();
                items.Add(item);
                this.Data.Push(data.ListName).Update(0,item);
            }
        }
    }
}
