---
layout: post
title: Peakboard UI Hacks - Next-Level Custom Dialogs
date: 2023-03-01 05:00:00 +0300
tags: ui bestpractice
image: /assets/2025-08-24/title.png
image_header: /assets/2025-08-24/title_landscape.png
bg_alternative: true
read_more_links:
  - name: UI - User Interface
    url: /category/ui
downloads:
  - name: CustomDialogs.pbmx
    url: /assets/2025-08-24/CustomDialogs.pbmx
---
Every Peakboard application is built around one or more screens. These screens are like the windows in a traditional desktop application. However, we sometimes want to have a [modal window](https://en.wikipedia.org/wiki/Modal_window) that forces the user to interact with it, before they can do anything else.

For example, we could use a modal window to make the user read a warning and acknowledge that they have read it---before proceeding.

In Peakboard Designer, you can create a modal window by using a *custom dialog*. In this article, we will discuss the best practices for building custom dialogs. The trick is to place all necessary controls for the dialog on the screen, then hide then and let them pop up when needed. We will go through this step by step.

## Preparing the screen

We need one button to initiate the dialog. Furtermore we need a shape as some background and place a text box, text input and another button on top of the shape.

![image](/assets/2025-08-24/010.png)

Then we group the dialog related controls together. We can do that just by dragging and dropping one control on top of another in the control tree on the left side. In that case a new group is automatically created. We can give the group a proper name.

![image](/assets/2025-08-24/020.png)

Then right click on the group and hide it to make sure, our dialog is not shown to the user.

![image](/assets/2025-08-24/030.png)

## Building the logic

The actual process logic to show the dialog is happening behind the first button. It's nothing else than switching the group back to `Show`.

![image](/assets/2025-08-24/040.png)

The logic behind the `OK` button is to prcoess the entry values from the user and then just hide group to let the dialog dissapear.

![image](/assets/2025-08-24/050.png)

## Result

The result shows how the dialog works in practise. WIth this kind of technique we can build any type of complex user inut or alert dialogs.

![image](/assets/2025-08-24/result.gif)