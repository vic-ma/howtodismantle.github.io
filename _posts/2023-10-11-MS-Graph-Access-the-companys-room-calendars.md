---
layout: post
title: MS Graph API - Access the company's room calendars
date: 2023-03-01 12:00:00 +0200
tags: msgraph
image: /assets/2023-10-11/title.png
read_more_links:
  - name: MS Graph API - Understand the basis and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
  - name: MS Graph API findRooms documentation
    url: https://learn.microsoft.com/en-us/graph/api/user-findrooms
  - name: MS Graph API list events documentation
    url: https://learn.microsoft.com/en-us/graph/api/user-list-events
downloads:
  - name: GraphRooms.pbmx
    url: /assets/2023-10-11/GraphRooms.pbmx
---
In this article, we will learn how to use Microsoft's Graph API to integrate room calendars into Peakboard.

We will create a dashboard that lets the user select a room, and then displays the events of that room.

Here's what the finished dashboard looks like. Notice how the list of events changes after clicking on a room.

![image](/assets/2023-10-11/010.gif)

Here is an overview of the steps we will take to create this dashboard:

1. **Add a variable for the selected room.** This lets us keep track of the selected room.
1. **Add a data source to get a list of all the rooms.** This lets us know which rooms the user can select.
1. **Add a data source to get the events of the selected room.**
1. **Add a room selector using a styled list control.** This lets the user choose the room they want to view.
1. **Add a table control that displays the events of the selected room.**
1. **Add a text control that indicates the selected room.**

To learn the basics of using the MS Graph API in Peakboard, see [this article]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %}).

Note that this article covers room calendars, not group calendars. To learn how to integrate group calendars into Peakboard, see [this article]({% post_url 2023-08-12-Dismantle-O365-group-calendars-with-MS-Graph %}).


## Add a variable for the selected room

We need some way of keeping track of the selected room. To do this, we will use a variable. We will update this variable when the user selects a room, and we will read this variable to know which room's events we should display.

So, we create a new variable.

![image](/assets/2023-10-11/020.png)

We name it `ActiveRooms`, and we make sure its data type is set to *String*.

![image](/assets/2023-10-11/030.png)


## Add a data source to get a list of all the rooms

We need to get a list of all the rooms that our API user has access to.

We create a new *Microsoft Graph User-Delegated Access* data source.

We set the permissions to `user.read offline_access User.Read.All`.

And we set the custom call to:

{% highlight url %}
https://graph.microsoft.com/beta/me/findRooms
{% endhighlight %}

This API call returns a list of rooms, in the form of their email address. The MS Graph API identifies rooms by their email addresses, which is why it works this way.

Check out the [official documentation](https://learn.microsoft.com/en-us/graph/api/user-findrooms) for more information about this endpoint.

![image](/assets/2023-10-11/040.png)


## Add a data source to get all the events of the selected room

Next, we need a data source that gets the events of the selected room.

We create a *Microsoft Graph App-Only Access* data source.

We set the custom call to:

{% highlight url %}
https://graph.microsoft.com/v1.0/users/#[ActiveRoom]#/events
{% endhighlight %}

In order to have the API call return the events of our selected room, we embed our `ActiveRooms` variable inside the API call itself. At runtime, `#[ActiveRoom]#` is replaced by the value of `ActiveRoom`.

Check out the [official documentation](https://learn.microsoft.com/en-us/graph/api/user-list-events) for more information about this endpoint.


## Add a room selector using a styled list control

Now, let's add a room selector, so the user can choose the room they want to view.

We create a new styled list control. The data source for the list is our `UserGetAllRooms` data source.

For the template, we add two text controls. One displays the `root_name` column, which is the name of the room. The other displays the `root_address` column, which is the email address of the room.

![image](/assets/2023-10-11/060.png)

To get the selection functionality, we add a tapped event to the `root_name` text control.  We also resize the text control to cover most of the template, so it's easily clickable.

![image](/assets/2023-10-11/070.png)

Here is the script for the tapped event:

![image](/assets/2023-10-11/080.png)

It sets the `ActiveRoom` variable to the email address of the room that's clicked.


## Add the table control which displays the events of a room

Now, we add a table control to display all the events of the selected room. We set its data source to our `ApplicationGetEventsFromRoom` data source. We select the columns we want.

![image](/assets/2023-10-11/050.png)


## Add text control to display the current room

Finally, we add a simple text control with its text set to the `ActiveRoom` data source. This lets the user know what the selected room is.

![image](/assets/2023-10-11/090.png)

## Conclusion

We've learned how to use the MS Graph API to list all the rooms we have, and display the events of the room that's selected.

You can download the [completed dashboard](/assets/2023-10-11/GraphRooms.pbmx) and try it out for yourself.