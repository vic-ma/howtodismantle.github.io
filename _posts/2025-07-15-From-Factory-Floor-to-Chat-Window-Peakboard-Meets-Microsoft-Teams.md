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

## Example use cases

Companies often use Microsoft Teams to connect factory workers to office workers. So it makes sense to integrate Teams into a Peakboard application, in order to further improve communication between the office and factory. 

Another use case for Teams integration is to let factory machines communicate with office workers. Suppose a machine---connected via OPC UA or other protocols---runs into an error. Then, it can use a Teams channel to notify the office workers automatically. Then, the workers can analyze and discuss the problem, all within the same Teams channel.

And these are just two examples. The combination of Peakboard and Teams makes for an effective communications solution, useful in a wide range of scenarios.

In today's article we'll explain how to build a simple chat client that integrates into Teams. This chat client lets factory workers send messages, view responses, and respond to messages.

Before you continue, make sure you understand how to handle authenticate in the Office 365 data source. 

## Set up the data source

To set up the data source, we first need to authenticate ourselves. If you're not sure how to do this, check out our [Office 365 getting started guide](/Getting-started-with-the-new-Office-365-Data-Sources.html).

Next we select the team and the channel that we want to connect to. In our example, we select the *Dismantle Team* team and the *Frontline Operations* channel.

Then, we set the *Retrieve messages from* setting. There are three modes that we can choose from:
1. Get all messages, including top-level messages and replies.
2. Get only the top-level messages.
3. Get only the replies for a specific top-level message.

For our example, we choose the first option. That way, the factory workers can see all the messages in the channel, including the replies.

![image](/assets/2025-07-15/010.png)

## Build the data flows

Next, we build some data flows to process the data source's output.

First, we build a data flow that cleans up the raw output a little bit:
* Format the date.
* Filter away all message rows with empty messages.
* Create a new column that we can use later to display the messages in a styled list.

![image](/assets/2025-07-15/020.png)

We want to be able to view only the top-level messages. To do this, we create another data flow. This data flow filters out all the replies, by removing any message that contains a parent.

![image](/assets/2025-07-15/030.png)

We also want to be able to view only the replies to a message. To do this, we create a variable called `ActiveChannelMessageID`. Whenever the user clicks on a button to view the replies of a top-level message, this variable is filled with the message's ID. Our data flow uses a filter to find the messages that have this message ID as their `Parent` value.

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

