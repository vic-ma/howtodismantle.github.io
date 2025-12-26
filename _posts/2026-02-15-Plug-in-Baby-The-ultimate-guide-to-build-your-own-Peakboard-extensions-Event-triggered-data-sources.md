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
    url: https://github.com/HowToDismantle/howtodismantle.github.io/tree/main/assets/2026-02-15/PushExtension/
---
This article is part four of our custom Peakboard extensions series:

* [Part I - The Basics](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html)
* [Part II - Parameters and User Input](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html)
* [Part III - Custom-made Functions](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Fun-with-Functions.html)
* [Part IV - Event-triggered data sources](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Event-triggered-data-sources.html)

In the first part of this series, we explained the basics of custom Peakboard extensions.
In the second part of this series, we explained how to add configuration options to a custom data source. In the third part of this series, we explained how to create functions for a custom data source. 

In today's article, we're going to explain how to create an **event-triggered data source.** Before you continue, make sure that you have read the [first part](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html) and [second part](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-Parameters-and-User-Input.html) of this series.

## Definitions

First, let's define two terms:
* **Data source:** A component in a Peakboard app that gets data from an actual source. Examples: SAP data source, Siemens S7 data source.
* **Actual source:** A program or device that feeds data to Peakboard. Examples: an SAP system, a physical Siemens S7 controller.

In other words, an **actual source** feeds data to a **data source:**
```
┌───────────────────┐              ┌─────────────────┐
│   Actual Source   │ --- data --> │   Data Source   │
└───────────────────┘              └─────────────────┘            
   External System                    Peakboard App
```

## Event-triggered data sources

There are two types of data sources in Peakboard:
* Query-based data sources.
* Event-triggered data sources.

Most data sources are **query-based**. With a query-based data source, the data source queries the actual source for new data. These queries can be triggered manually (e.g. the user taps a button), by a timer (e.g. send a query every 10 seconds), or by scripts.

However, a few data sources are **event-triggered**. This means that the actual source decides when to send data to the data source. The data source has no control over when new data comes in---its only job is to listen and wait for the actual source to send data.

An example of an event-triggered data source is the MQTT data source. The data source never queries the MQTT server (the actual source). Instead, the data source simply registers itself with the server. And whenever the server gets a new message, it sends that message to the Peakboard app, where the MQTT data source accepts the message. (This is simplified a bit, there are also topics involved.) If there are no new messages, then nothing happens.


## Create an event-triggered data source

Now, let's create a simple event-triggered data source that accepts messages.

First, we follow the [standard steps for creating a custom data source](/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html). The only difference is that we set the `SupportsPushOnly` attribute to `true`. This turns our data source into an event-triggered data source.

We also add a multi-line text parameter called `MyMessages`. This parameter specifies the messages that the actual source can send to our data source.

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

Next, we specify the two columns that our data source returns:
1. `TimeStamp`, the time when the message was received.
1. `Message`, the message that was received.

{% highlight csharp %}
protected override CustomListColumnCollection GetColumnsOverride(CustomListData data)
{
    var columns = new CustomListColumnCollection();
    columns.Add(new CustomListColumn("TimeStamp", CustomListColumnTypes.String));
    columns.Add(new CustomListColumn("Message", CustomListColumnTypes.String));
    return columns;
}
{% endhighlight %}

## Create the actual source

In order to test our data source, we need to simulate an actual source that sends messages to our data source. To do this, we'll have our data source create a `Timer` object, which runs in a separate thread and sends random messages (chosen from the messages in `MyMessages`) to the data source.

Of course, this is just for demonstration purposes. In the real world, the actual source is always some independent, external application that already exists. The only goal of the extension developer is to develop the data source.

We create the `Timer` in the `SetupOverride()` function. This function runs during the Peakboard application start-up process. We also pass our `CustomListData` to the `Timer`, so that the `Timer` can interact with our data source.

{% highlight csharp %}
private Timer? _timer;

protected override void SetupOverride(CustomListData data)
{
    this.Log.Info("Initializing...");
    _timer = new Timer(new TimerCallback(OnTimer), data, 0, 1000);
}
{% endhighlight %}

Next, we implement `CleanupOverride()`. This function runs during the Peakboard application shut-down process. We dispose of the `Timer` in this function.

{% highlight csharp %}
protected override void CleanupOverride(CustomListData data)
{
    this.Log.Info("Cleaning up....");
    _timer?.Dispose();
}
{% endhighlight %}

The last major part is the actual event, in our case the ticking of the timer. We will convert the `state` object back to `CustomListData` to get access to what the user provided in the input parameter (in our case the list of random messages to push).

The `CustomListObjectElement` represents a single row of the destination table. It is filled with a random message and timestamp. Then we use `this.Data.Push()` to push the prepared data set to the host system. The `.Update` function replaces the data. So the behaviour is to leave the table with one single row and just exchange this entry every time the timer fires. Let's assume we wanted to simply add the data at the end of the table instead of replacing it. We would need to use `this.Data.Push(...).Add(...)`.

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
        this.Data.Push(data.ListName).Update(0, item);
    }
}
{% endhighlight %}

## Result

The video shows the extension in action after it's bound to a table control. Once again, it must be clear that this example actually doesn't make sense, but it is suitable for showcasing how pushing data works instead of pulling it - without getting distracted by too much code. It can be used as a lightweight template. Let's imagine a real-life scenario: In the setup phase we could open a TCP connection to a machine and close it through `Cleanup`. Every time a TCP message comes in, we could trigger the `Data.Push`. This would be a real-world example; however, it would require much more code beside the pure implementation as shown here in this article.

![Push messages in action](/assets/2026-02-15/result.gif)
