---
layout: post
title: MS Graph API - Send messages to Teams group
date: 2023-03-01 12:00:00 +0200
tags: msgraph
image: /assets/2023-07-11/title.png
---
Teams is one of the ultimate communication tools in companies. This article explains how to send a Teams message from a Peakboard application by using the MS Graph API.

Please make sure to read through the basics of using MS Graph API in Peakboard: [MS Graph API - Understand the basis and get started]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %})

# Finding the group

Later on, we will use an API call to send the message to Teams. For this, we need to know the ID of the group and the ID of the channel wehre the message should be posted in. The easiest way to get these, is to use the Graph Explorer. So first call this URL to get a list and metadata of all available teams / groups:

{% highlight url %}
https://graph.microsoft.com/v1.0/groups
{% endhighlight %}

Here's the answer from the Graph Explorer. The ID is the first element of each list entry.

![image](/assets/2023-07-11/010.png)

# Getting the channel

With the help of the group ID, we can build another call to get the channels of a group listed. If you want to do it on your own, don't forget to replace the group ID and use your own:

{% highlight url %}
https://graph.microsoft.com/v1.0/teams/f2f256ca-7d65-410f-8b57-2fa0499e087a/allChannels
{% endhighlight %}

![image](/assets/2023-07-11/020.png)

# Build the call data source

Now let's switch to the Peakboard side and add a User Function data source from the MS Graph extension to the board. We need to provide Client ID and Tenant ID and get the authentification for the delegated user ready. Please don't forget to add the _ChannelMessage.Send_ permission.

Then we click on _Add function_ to add a function to this URL. As you see, we must provide the group ID and channel ID within the URL.

{% highlight url %}
https://graph.microsoft.com/v1.0/teams/f2f256ca-7d65-410f-8b57-2fa0499e087a/channels/19:3f11d999d7674dc78fbad32e242869bc@thread.tacv2/messages
{% endhighlight %}

The actual message is sent in a JSon formatted body. It must looks like this sample (feel free to check Microsoft's [documentation](https://learn.microsoft.com/en-us/graph/api/channel-post-messages?view=graph-rest-1.0&tabs=http)).

{% highlight json %}
{
    "body": {
        "content": "I ain't got a lotta money but I got a lotta style."
    }
}
{% endhighlight %}

However we want to make a dynamic message that is defined by the end user. That's why we replace the actual message with a variable $s_message$. How to use this variable, we learn in the next step.

![image](/assets/2023-07-11/030.png)

# Build the actual call

We finally place a text box and a _Send_ button on the canvas. Don't forget to give the text box a proper name (e.g. _MyMessage_) otherwise we can't address it in our code.

![image](/assets/2023-07-11/040.png)

Here are the Building Blocks behind the button. The Graph function can be found on the right side and dragged on the BB canvas. Because of the variable we placed in the JSon body of the Graph call earlier, the signature of the function can be easily accessed. Just drag and drop the text box to the _message_ parameter of the function call. That's it....

![image](/assets/2023-07-11/050.png)

Here's the final result:

![image](/assets/2023-07-11/060.gif)


read_more_links:
  - name: MS Graph API Documentation - Send chatMessage in channel
    url: https://learn.microsoft.com/en-us/graph/api/channel-post-messages?view=graph-rest-1.0&tabs=http
  - name: MS Graph API - Understand the basis and get started
    url: /2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started.html
downloads:
  - name: GraphSendMessageToTeamsGroup.pbmx
    url: /assets/2023-07-11/GraphSendMessageToTeamsGroup.pbmx

