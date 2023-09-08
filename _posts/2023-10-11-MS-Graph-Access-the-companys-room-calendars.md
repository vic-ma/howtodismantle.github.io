---
layout: post
title: MS Graph API - Access the company's room calendars
date: 2023-03-01 12:00:00 +0200
tags: msgraph
image: /assets/2023-10-11/title.png
read_more_links:
  - name: 
    url: 
  - name: MS Graph API - Understand the basis and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
downloads:
  - name: 
    url: 
---
In this article, we will learn how to visualize a company's O365 room calendars, in Peakboard.

We will create a dashboard that can display the events of three different room calendars. Only one calendar's events will be displayed at a time, and the user can switch between them by clicking on a room selector.

Here's what the finished dashboard looks like. Notice how clicking on the different rooms changes the data in the table.

![image](/assets/2023-10-11/010.gif)

Here is an overview of the steps we will take to create this dashboard:

1. **Create a variable for the active room.**
1. **Add the MS Graph data source for all the rooms.**
1. **Add the MS Graph data source for the events of a room.**
1. **Create the table control which displays the events of a room.**
1. **Create the list control which displays all the rooms and lets the user switch between them.**

To learn the basics of using the MS Graph API in Peakboard, see [this article]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %}).

This article covers room calendars, and not group calendars. To learn how to use group calendars in Peakboard, see [this article]({% post_url 2023-08-12-Dismantle-O365-group-calendars-with-MS-Graph %}).









* ApplicationGetEventsFromRoom -> MSGraphAppOnlyCustomList
* UserGetAllRooms              -> MSGraphUserAuthCustomList