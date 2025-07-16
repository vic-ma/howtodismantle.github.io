---
layout: post
title: From Factory Floor to Chat Window - Peakboard Meets Microsoft Teams
date: 2023-03-01 00:00:00 +0000
tags: office365
image: /assets/2025-07-15/title.png
image_header: /assets/2025-07-15/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All Office 365 articles
    url: /category/office365
  - name: Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 ToDo
    url: /Elevate-Your-Workflow-Building-a-Next-Gen-Continuous-Improvement-Board-with-Office-365-ToDo.html
downloads:
  - name: TeamsDemo.pbmx
    url: /assets/2025-07-15/TeamsDemo.pbmx
---
We've published many articles on how to integrate [Office 365 with Peakboard](/category/office365). In today's article, we'll look at Office 365 again---but this time, we'll take a closer look at Microsoft Teams.

Companies often use Microsoft Teams to connect factory workers to office workers. So it makes sense to integrate Teams into a Peakboard application, in order to further improve communication between the office and factory. 

Another use case for Teams integration is to let factory machines communicate with office workers. Suppose a machine---connected via OPC UA or other protocols---runs into an error. Then, it can use a Teams channel to notify the office workers automatically. Then, the workers can analyze and discuss the problem, all within the same Teams channel.

And these are just two examples. In today's article we discuss just a simple chat client for factory workers who can submit new messages or respond on messages.

Before we start just a reminder: Every Office 365 data source handles authentication in the same way. To learn more about how to authenticate against the Office 365 backend, see our [getting started guide](/Getting-started-with-the-new-Office-365-Data-Sources.html).

## Setting up the data source

For setting up the data source we need to authenticate first. Then we choose the Team and the Channel to get access to. There are three different modes available:

1. Get all messages, the channel message and the replies
2. Get only the channel messages
3. Get only the replies for one dedicated channel message.

In our use case we go for the first option. So we will retrieve all message of channel including their replies. We will then seperate channel messages and replies later through data sources.

![image](/assets/2025-07-15/010.png)

## Building the data flows

In our first data flow we beautify the raw output a little bit. Set a good date format, filter away all message rows with empty messages and also create a new column which we can use later for displaying the messages in a styled list.

![image](/assets/2025-07-15/020.png)

For the selection of the channel messages we add another data flow and filter away all messages that have a parent. If the message dowsn't have a parent, it's a channel message.

![image](/assets/2025-07-15/030.png)

The same principle works for the replies. We have a variable called "ActiveChannelMessageID". It is filled when the user clicks on a button to view the replies of this dedicated channel message. We filter out only the the messages that have this message ID as their parent value.

![image](/assets/2025-07-15/040.png)

## Building the UI

The central element is a styled list to display the channel messages. It's bound to the data flow for the channel messages.

![image](/assets/2025-07-15/050.png)

The Building Blocks behind the button for opening the replies is just setting the variable for the message ID and then reloading the data flows for the replies. That's a second styled list to show the replies. It is automatically set to visible through a condtional formatting as soon as the channel message ID is set. We don't have a screenshot for every step, but we can easily look follow this principle by checking the downloadable pbmx file.

![image](/assets/2025-07-15/055.png)

Let's have a look on how to submit a new channel message. As soon as the user click on the "New post" button, a group of controls are set to visible to form a pop up for submitting the new message.

![image](/assets/2025-07-15/060.png)

The actual creation of the new channel message happens through a Building Block that is provided in the context of the data source. We can either post a channel message (see screenshot) or a reply. If we want to send a reply we need to provide the channel message ID.

![image](/assets/2025-07-15/065.png)

## result

In last image we can see the application in action. The data is qeuried from Teams. Then the user clicks on the replies button and the thread opens on the right side. We can then just compose a new reply and submit it to the backend.

![image](/assets/2025-07-15/result.gif)

