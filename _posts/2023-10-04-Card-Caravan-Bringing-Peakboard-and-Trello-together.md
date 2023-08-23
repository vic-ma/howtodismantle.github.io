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


### Adding a data source

Let's add a data source for our *To Do* list. We create a new *Trello Cards (TrelloCustomList)* data source.

![image](/assets/2023-10-04/060.png)

And we fill out the properties of the data source. `AppKey` is our API key from before. `UserToken` is our token from before. `BoardName` is the name of our Trello board, and `ListName` is the name of our Trello list.

![image](/assets/2023-10-04/050.png)

Then, we repeat for the other two lists.


## Building the dashboard

Here is what we want in our dashboard:

* It should display our three lists, with a similar Layout to Trello.
* Each card in the first two lists should have a button that moves that card to the next list
* Each card in the third list should have a button that deletes the card.
* There should be a button that refreshes all the lists on screen, by reloading the data sources
* There should be a button that opens a separate screen to create a new card
	* This separate screen should include a text box for a subject and description for the new card

Here's what the finished dashboard looks like:
![image](/assets/2023-10-04/070.png)
