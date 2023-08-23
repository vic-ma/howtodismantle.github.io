---
layout: post
title: Card Caravan - Bringing Peakboard and Trello together
date: 2023-03-01 12:00:00 +0200
tags: trello
image: /assets/2023-10-04/title.png
read_more_links:
  - name: Learn the basics of Trello
    url: https://trello.com/guide/trello-101
  - name: Trello extension for Peakboard
    url: https://templates.peakboard.com/extensions/Trello/en
---

Trello is an online work management tool based around lists. In this article, we will learn how to create a Peakboard dashboard that can display, edit, and create Trello lists.

This article assumes that you are already familiar with the [basics of Trello](https://trello.com/guide/trello-101).

Here is an overview of the steps we will take:

1. **Get our `AppKey` and `UserToken` from Trello.** Our Peakboard Box will use these to authenticate itself to Trello
1. **Create a data source for each of our Trello lists.** These data sources will let us display the lists in our dashboard.
1. **Add table controls to visualize our Trello lists.**
1. **Add screen for new list, etc.**


## Get an API key and token from Trello

In order to get an API key and token, we first need to create a new Power-Up in Trello. We can do this in the [Power-Ups admin portal](https://trello.com/power-ups/admin).

![image](/assets/2023-10-04/010.png)

Once our Power-Up is created, we can generate an API key for it.

![image](/assets/2023-10-04/020.png)

We copy down our API key. Then, we generate a token for our Power-Up and copy it down too.

![image](/assets/2023-10-04/030.png)

See [Trello's documentation](https://developer.atlassian.com/cloud/trello/guides/rest-api/api-introduction/#managing-your-api-key) for a more detailed guide to these steps.


## Create a data source for each of our Trello lists

Here's what our sample Trello board looks like:

![image](/assets/2023-10-04/040.png)

We will add a data source for each of the lists in our board.

To do this, we need to have Peakboard's [Trello extension](https://templates.peakboard.com/extensions/Trello/en) installed.


### Add a data source

Let's add a data source for our *To Do* list. We create a new *Trello Cards (TrelloCustomList)* data source.

![image](/assets/2023-10-04/060.png)

And we fill out the properties of the data source. `AppKey` is our API key from before. `UserToken` is our token from before. `BoardName` is the name of our Trello board, and `ListName` is the name of our Trello list.

![image](/assets/2023-10-04/050.png)

Then, we repeat for the other two lists.


## The finished dashboard

Here is what we want in our dashboard:

* It should display our three lists, with a similar Layout to Trello.
* Each card should have a dot that indicates its label color.
* Each card in the first two lists should have a button that moves that card to the next list.
* Each card in the third list should have a button that deletes the card.
* There should be a button that refreshes all the lists on screen, by reloading the data sources.
* There should be a button that opens a separate screen to create a new card.
	* This separate screen should include a text box for a subject and description for the new card.

Here's what the finished dashboard looks like:
![image](/assets/2023-10-04/070.png)

Instead of going through everything step-by-step, we will instead focus on the Trello-specific parts. Knowledge about different Peakboard Designer features like conditional formatting and scripts is assumed.


## The table control

For our table control, there are two important things to cover:

1. How to create the label color indicator.
2. How to create the move and delete buttons.

Let's take a look at both these things.

### Label color indicator

SKIP FOR NOW

![image](/assets/2023-10-04/080.png)


### Move and delete buttons

We make our buttons functional by adding a tapped event. A tapped event is a script that executes when the button is tapped.

![image](/assets/2023-10-04/090.png)

Here is the script for the move button:

{% highlight lua %}

local mycard = data.MyTrelloToDo[screens['Screen1'].MyToDoListView.getindex(this)].CardId

data.MyTrelloToDo.movecard(
   mycard, -- String - The ID of the card
   'Work in Progress' -- String - The name of the list that the card should be moved to
)

data.MyTrelloToDo.reload()
data.MyTrelloWiP.reload()

{% endhighlight %}

It first gets the id of the card being moved and stores it in `mycard`. Then, it calls the `movecard` function to move that card to the next list. Finally, it reloads both affected lists.

Here is the script for the delete button:

{% highlight lua %}

local mycard = data.MyTrelloDone[screens['Screen1'].MyDoneListView.getindex(this)].CardId

data.MyTrelloDone.delete(
   mycard -- String - The ID of the card that should be deleted.
)

data.MyTrelloDone.reload()

{% endhighlight %}

Like before, it first gets and stores the id of the card to be deleted. Then, it calls the `delete` function to delete the card. Finally, it reloads the affected list.


## The refresh all button

This button just has a simple tapped event that reloads all three data sources.

{% comment %}
![image](/assets/2023-10-04/110.png)
{% endcomment %}

![image](/assets/2023-10-04/100.png)


## The create new button

First, we create a new screen for our create new dialog. We name it `CreateNew`.

Then, we will add a tapped event for our button that just switches to the new screen.

![image](/assets/2023-10-04/120.png)

![image](/assets/2023-10-04/130.png)


## The create new screen

In our new screen, we add a text box control for both the subject and description of our new card. We select the *Used in scripting* option, and we give it a control name. The control name is used to identify the control in our script.

![image](/assets/2023-10-04/140.png)


Here is the script for the *Create Card* button. It runs the `addcard` function, taking the name and description from our text boxes, and using our first list as the target list. Then, it switches screens and reloads the affected list.

![image](/assets/2023-10-04/150.png)

Our cancel button has a simple tapped event that just switches the Peakboard Box back to the main screen.

![image](/assets/2023-10-04/160.png)

