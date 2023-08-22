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

1. **Get our `AppKey` and `UserToken` from Trello.** Our Peakboard Box will use these to authenticate itself to Trello.
2. **Install the Peakboard Trello extension.**
3. **Create a data source for each of our Trello lists.** These data sources will let us display the lists in our dashboard.
4. **Add table controls to visualize our Trello lists.**
5. **Add screen for new list, etc.**


## Get an API key and token from Trello

In order to get an API key and token, we first need to create a new Power-Up in Trello. We can do this in the [Power-Ups admin portal](https://trello.com/power-ups/admin).

![image](/assets/2023-10-04/010.png)

Once our Power-Up is created, we can generate an API key for it.

![image](/assets/2023-10-04/020.png)

We copy down our API key. And finally, we generate a token for our Power-Up and copy it down.

![image](/assets/2023-10-04/030.png)

See [Trello's documentation](https://developer.atlassian.com/cloud/trello/guides/rest-api/api-introduction/#managing-your-api-key) for a more detailed guide for these steps.
