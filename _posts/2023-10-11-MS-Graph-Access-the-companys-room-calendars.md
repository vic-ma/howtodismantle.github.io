---
layout: post
title: MS Graph API - Access the company's room calendars
date: 2023-03-01 12:00:00 +0200
tags: msgraph
image: /assets/2023-10-11/title.png
read_more_links:
  - name: MS Graph API - Understand the basis and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
  - name: Dismantle O365 group calendars with MS Graph API
    url: /Dismantle-O365-group-calendars-with-MS-Graph.html
  - name: MS Graph API findRooms documentation
    url: https://learn.microsoft.com/en-us/graph/api/user-findrooms
downloads:
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

Finally, note that this article covers room calendars, and not group calendars. To learn how to use group calendars in Peakboard, see [this article]({% post_url 2023-08-12-Dismantle-O365-group-calendars-with-MS-Graph %}).


## Create a variable for the active room

We will use a variable to store the name of the currently selected room. Then, the list control will write to it when the user selects on a different room from the current one. And, the data source will read from it to construct the room-specific MS Graph API call.

So, we create a new variable.

![image](/assets/2023-10-11/020.png)


We name it `ActiveRooms`, and we make sure its data type is set to *String*.

![image](/assets/2023-10-11/030.png)


## Create a data source to get all the rooms

We need a data source to get a list of all the rooms using the MS Graph API. We create a *Microsoft Graph User-Delegated Access* data source.

We set the permissions to `user.read offline_access User.Read.All`.

We set the call to `https://graph.microsoft.com/beta/me/findRooms`. Check out the [official documentation](https://learn.microsoft.com/en-us/graph/api/user-findrooms) for more information about this endpoint.


![image](/assets/2023-10-11/040.png)


## Create a data source to get all the events of a room

We need a data source to get the events of a room. The room to get the events from will be determined by our `ActiveRooms` variable.


{% comment %}

* ApplicationGetEventsFromRoom -> MSGraphAppOnlyCustomList
* UserGetAllRooms              -> MSGraphUserAuthCustomList

{% endcomment %}