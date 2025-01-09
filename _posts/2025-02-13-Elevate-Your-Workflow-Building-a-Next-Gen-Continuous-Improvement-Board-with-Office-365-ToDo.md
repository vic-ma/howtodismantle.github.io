---
layout: post
title: Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 To Do
date: 2023-03-01 03:00:00 +0200
tags: office365
image: /assets/2025-02-13/title.png
image_header: /assets/2025-02-13/title_landscape.png
read_more_links:
  - name: All Office 365 articles
    url: /category/office365
downloads:
  - name: FactoryCITaskTracking.pbmx
    url: /assets/2025-02-13/FactoryCITaskTracking.pbmx
---

With the new Office 365 data sources, introduced by the Peakboard dev team in early 2025, you can access Microsoft To Do with just a few clicks. In this article, you will learn how to set up the Office 365 To Do data source. We will build a standard board where factory workers can track their daily tasks around improving the production processes --- a "Continuous Improvement (CI) Board."

In our example application, the end user can create new tasks or set existing tasks to "done." The data backend will be a To Do list that is located in our Office 365 space.

To learn how to authenticate with the Office 365 backend, see [Getting started with the new Office 365 Data Sources](/Getting-started-with-the-new-Office-365-Data-Sources.html).

## Set up the backend

To access the Office 365 To Do application, we use [this link](https://to-do.office.com/). For our CI board, we create a dedicated CI list:

![image](/assets/2025-02-13/010.png)

## Set up the data source

After having created a new To Do list instance, we [authorize against the Office 365 account](/Getting-started-with-the-new-Office-365-Data-Sources.html). All accessible To Do lists show up in the combo box.

The To Do list itself is a simple table with all the common attributes like title, description, due date, priority, status, etc.

![image](/assets/2025-02-13/020.png)

Later, we want to show all the open tasks in our list to the end user. So, we create a simple dataflow that filters for tasks with the status `notStarted`.

![image](/assets/2025-02-13/030.png)

## Build the main screen

For the main screen, we use a styled list to present the task. We bind this list to the `OpenTasks` dataflow.

![image](/assets/2025-02-13/040.png)

In the list item editor, we choose the attributes to be displayed. In our case, we only add the title and the due date. We also place a button on the item that lets the user set a list item to "Done."

![image](/assets/2025-02-13/050.png)

Let's look behind the code of the **Done** button. In Building Blocks, there are several blocks that manipulate To Do list items. In our case, we set "task completed" to true. The ID of the task is accessible from the menu. It refers to the current instance of the styled list item. So, we need to connect those two blocks and combine them into a single command, followed by a refresh of the list.

![image](/assets/2025-02-13/060.png)

## Create a new task

To let the user create a new task, we build a new screen and let the user jump to that screen.

![image](/assets/2025-02-13/070.png)

The screen has text boxes for the title, description, and due date of the new task.

![image](/assets/2025-02-13/080.png)

Let's look at the Building Blocks of the **Create Task** command button. The button calls a function that is offered by the To Do data source. After the creation of the task, we refresh the data source and jump back to the overview screen.

![image](/assets/2025-02-13/090.png)

## Result

This short video shows the final result. We create a new task and then set it to done.

![image](/assets/2025-02-13/result.gif)
