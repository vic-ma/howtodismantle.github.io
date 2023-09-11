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
  - name: MS Graph API list events documentation
    url: https://learn.microsoft.com/en-us/graph/api/user-list-events
downloads:
---
In this article, we will learn how to integrate Microsoft 365 room calendars into Peakboard, using Microsoft's Graph API.

We will create a dashboard that lists the events of a room calendar. We will also add a selector that lets the user select which room they want to view.

Here's what the finished dashboard looks like. Notice how the list of events changes when I click on the different rooms.

![image](/assets/2023-10-11/010.gif)

Here is an overview of the steps we will take to create this dashboard:

1. **Create a variable for the selected room.** This variable holds the email address of the currently selected room.
1. **Add an MS Graph data source to get a list of rooms.** This data source gets a list of rooms, in the form of their email address.
1. **Add an MS Graph data source to get the events of a room.** This data source gets the events of the currently selected room, using our variable.
1. **Create a room selector with a styled list control** When the user selects a room, we update our variable.
1. **Create a table control that displays the events of the selected room.**
1. **Add a text control that displays the currently selected room.**

To learn the basics of using the MS Graph API in Peakboard, see [this article]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %}).

Also note that this article covers room calendars, not group calendars. To learn how to integrate group calendars into Peakboard, see [this article]({% post_url 2023-08-12-Dismantle-O365-group-calendars-with-MS-Graph %}).


## Create a variable for the selected room

We need some way of keeping track of the currently selected room. To do this, we will use a variable. We will update this variable when the user selects a room, and we will read this variable to know which room's events we should display.

So, we create a new variable.

![image](/assets/2023-10-11/020.png)


We name it `ActiveRooms`, and we make sure its data type is set to *String*.

![image](/assets/2023-10-11/030.png)


## Create a data source to get a list of rooms

We need to get a list of rooms that our API user has access to.

So, we create a new *Microsoft Graph User-Delegated Access* data source.

We set the permissions to `user.read offline_access User.Read.All`.

And we set the custom call to:

{% highlight url %}
https://graph.microsoft.com/beta/me/findRooms
{% endhighlight %}

This API call returns a list of rooms, in the form of their email address.  Check out the [official documentation](https://learn.microsoft.com/en-us/graph/api/user-findrooms) for more information about this endpoint.

![image](/assets/2023-10-11/040.png)


## Create a data source to get all the events of a room

Next, we need a data source that gets the events of our currently selected room. So, we create a *Microsoft Graph App-Only Access* data source.

The room to get the events from is determined by our `ActiveRooms` variable.

In order to have the API call return the events of our selected room, we will embed our `ActiveRooms` variable inside the API call itself.

We set the custom call to:

{% highlight url %}
https://graph.microsoft.com/v1.0/users/#[ActiveRoom]#/events
{% endhighlight %}

Note that `#[ActiveRoom]#` will be replaced by the value of `ActiveRoom`.

Check out the [official documentation](https://learn.microsoft.com/en-us/graph/api/user-list-events) for more information about this endpoint.


## Create a room selector with a styled list control

Now, let's add a room selector so the user can choose the room they want to see events for.

We create a new styled list control. The data source for the list is our `findRooms` data source.

For the template, we add two text controls. One displays the `root_name` column, which is the name of the room's occupant. The other displays the `root_address` column, which is the email address of the room's occupant.

![image](/assets/2023-10-11/060.png)

We'll use the `root_name` text control to host our tapped event. We resize it to cover most of the template, so it's easily pressable. Finally, we add a tapped event that switches the variable to the appropriate one.

![image](/assets/2023-10-11/070.png)

Here is the script:

![image](/assets/2023-10-11/080.png)

It sets the `ActiveRoom` variable to the `root_address` column of the current row number. The current row number is the row number of the room in the styled list control that is being pressed.


## Create the table control which displays the events of a room

We add a table to display all the events of the selected room. We set it to our room events data source. We select the columns we want.

![image](/assets/2023-10-11/050.png)


## Add text control to display the current room

Finally, we add a simple text control with its text set to the `ActiveRoom` data source.

![image](/assets/2023-10-11/090.png)

## Conclusion

And that's it! We've learned how to use the MS Graph API to show the different rooms we have available, and display the events of the room we select.