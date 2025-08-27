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

In Peakboard Designer, you can create a modal window by using a *custom dialog*. In this article, we'll explain the best practices for custom dialogs.

## The core idea

The core idea is to always keep the dialog on the screen---but make it invisible. Then, when the dialog is supposed to pop up, make the dialog visible. Let's go through this step by step with an example.

We'll make a simple app that prompts the user to enter their name. Here's what the finished app looks like:

![image](/assets/2025-08-24/result.gif)

Here's how it works:
1. The user clicks on the *Call me by my name* button.
1. A modal window pops up and asks the user to enter their name.
1. The user enters their name into the modal window and clicks *OK*.
1. The modal window goes away.
1. The user's name is displayed.

Now, let's build the app.

## Add the controls

First, we add the following controls to the app:
* The button that initiates the modal dialog. We label it, *Call me by my name*. This is the button that initiates the dialog.
* Controls for the modal window:
  * A rectangle shape for the background of the modal window.
  * A text box that prompts the user, *Please type in your name*.
  * A text input for the user to enter their name in.
  * A button for the user to submit their name, labelled *OK*.

![image](/assets/2025-08-24/010.png)

Next, we group together the modal window controls. We do this by dragging and dropping one control on top of another in the control tree on the left side. This automatically creates a new control group. Then, we rename the group to `MyDialogGroup`.

![image](/assets/2025-08-24/020.png)

Next, we right click the control group and select *Hide*. 

![image](/assets/2025-08-24/030.png)

This makes the control group invisible by default. We will use Building Blocks to make it visible.

## Add the Building Blocks

We add two Building Blocks scripts---one for each button.

### The *Call me by my name* button

The *Call me by my name* button initiates the modal dialog. So, we use a Building Block that shows `MyDialogGroup`, when the button is clicked:

![image](/assets/2025-08-24/040.png)

### The *OK* button

The *OK* button displays the user's name, and it re-hides the modal dialog. So, this is what the Building Blocks look like:

![image](/assets/2025-08-24/050.png)

## Result

Let's take a look again at the end result. With the techniques we showed off in this article, you can build all kinds of user input and alert dialogs!

![image](/assets/2025-08-24/result.gif)