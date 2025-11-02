---
layout: post
title: Plug-in, Baby - The ultimate guide to build your own Peakboard extensions - Event-triggered data sources
date: 2023-03-01 00:00:00 +0000
tags: dev
image: /assets/2026-02-15/title.png
image_header: /assets/2026-02-15/title.png
bg_alternative: true
read_more_links:
  - name: Developer stuff
    url: /category/dev
downloads:
  - name: Source code for this article
    url: https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2026-02-15/PushMessageExtension
---
In the first part of the series we learned how to build the frame of a Peakboard extension. We used two classes to provide both metadata and the actual payload that is exchanged between the extension and the Peakboard application. In the second part we discussed how to form parameters to enable user interaction and let the user configure the extension. How to create functions in extenesions was the topic for the thirs part. We eve exchanged complex data types and mutliple return values:

* [Part I - The Basics](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html)
* [Part II - Parameters and User Input](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)
* [Part III - Custom-made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)
* [Part IV - Event-triggered data sources](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Event-triggered-data-sources.html)

In today's article we will talk about push extensions or event-triggered data sources. Most of the data source that are built-in with Peakboard or built through the extension kit are pull extensions. That means the data is queried from the data source pro-actively. Mostly triggered by a time period or triggered manually or through code. However there are also push sources where the data transfer is triggered from wtih the data source. A typical example would be MQTT. We don't just reguarly ask the MQTT server for new messages, instead we initially register at the MQTT server and subscribe to certain topics. When a message is comming in from one the subscribed topics the data refresh is triggered implictily. When there are no messages, no refresh is happening. Thats the nature of push data source and this bahaviour changes the internal architecure fundamentally.

To keep it as simple as possible and focus on the basics we set up a very simple example. We just use a timer for simulating a source for external events and every time the timer is ticking we push new data to the hosting environment. Of course this example actually is not very practical because we could achieve the same behaviour with the tradtional extension that runs on a time interval. But it can show the principle of an event triggered extension without disctrating too much from the pure architecture.

## Setting up the basics

In our example the user has only one input parameter called `MyMessages`. It contains a list of message that are pushed randomly to the host. The source code of whole example can be found [on github](https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2026-02-15/PushMessageExtension).

The important point is to set the attribute `SupportsPushOnly` to indicate that we're build a push extension.

{% highlight csharp %}
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
{% endhighlight %}

Just to complete the meta data, we're using two columns for the resultset to push: `TimeSTamp` and `Message`, which contains the actual message later.

{% highlight csharp %}
protected override CustomListColumnCollection GetColumnsOverride(CustomListData data)
{
    var columns = new CustomListColumnCollection();
    columns.Add(new CustomListColumn("TimeStamp", CustomListColumnTypes.String));
    columns.Add(new CustomListColumn("Message", CustomListColumnTypes.String));
    return columns;
}
{% endhighlight %}

## Imlpementing the actual push

First, we override the function `SetupOverride`. It's called once the host project is starting up and wants all data source to do initial setup activities. So we're initializing our timer object. the instance of the `CustomListData` is also submitted to the timer. We will need it later.

{% highlight csharp %}
private Timer? _timer;

protected override void SetupOverride(CustomListData data)
{
    this.Log.Info("Initializing...");
    _timer = new Timer(new TimerCallback(OnTimer), data, 0, 1000);
}
{% endhighlight %}

Second, we implement `CleanupOverride`. It's called at the end of the life cycle right before shutting down the host project. We can use the opportunity to dispose the timer object.

{% highlight csharp %}
protected override void CleanupOverride(CustomListData data)
{
    this.Log.Info("Cleaning up....");
    _timer?.Dispose();
}
{% endhighlight %}

The last major part is the actual event, in our case the ticking of the timer. We will convert the `state` object back to `CustomListData` to get access to what the user provided in the input parameter (in our case the list of random messages to push).

The `CustomListObjectElement` represents a single row of the destination table. It's is filled with a random message and time stamp. Then we use `this.Data.Push()` to pushed the prepared to the host system. The `.Update` function is replacing the data. So the behivour is to leave the table with one single row and just exchanged this. Let's assume we wanted to simply add the data at the end of the table instead if replacing it, we would need to use `this.Data.Push(...).Add(...)`.

{% highlight csharp %}
private void OnTimer(object? state)
{
    this.Log.Info("event triggered...");

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
{% endhighlight %}

## result

The video shows the extension in action after it's bound to a table control. Once again it must be clear, that this example actually doesn't make sense but it is suitable for showcasing how pushing data works instead of pulling it - without getting distracted by too much code. It can be used a lightweight template. Let's imagine a real life scenario: In the Setup phase we could open an TCP connection to a machine and close it with through CleanUp. EVerytime a TCP message is coming in, we could trigger the `Data.Push`. This would be a real world example, however it would require much more code beside the pure implmentation as shown here in this article. 

![Push messages in action](/assets/2026-02-15/result.gif)
