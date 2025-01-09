---
layout: post
title: Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 ToDo
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
With the new Office 365 data sources, which were introduced early 2025 by the Peakboard dev team, access to Microsoft ToDo can be done within a few clicks. In this article we learn how to set up the Office 365 ToDo data source and build a typical board where factory workers can track their daily tasks around improving the prodcution processes - a "Continuous Improvement (CI) Board"  In our sample application we can let the end user create new tasks or set existing tasks to "done". The data backend will be a todo list that is located within the Office 365 space.

For authentification against the Office 365 backend everythig is explained in this article [Getting started with the new Office 365 Data Sources](/Getting-started-with-the-new-Office-365-Data-Sources.html), so connecting to Office 365 is not part of this article.

## Set up the backend

To access the ToDo application of Office 365 we can use [this link](https://to-do.office.com/). For our CI board we just create a dedicated CI list. 

![image](/assets/2025-02-13/010.png)

## Set up the data source

After having created a new ToDo list instance we [authorize against the Office 365 account](/Getting-started-with-the-new-Office-365-Data-Sources.html). All accessibe ToDo lists show up in the combo box. The ToDo list itself is just a simple table with all common attributes like title, description, due date, priority, status, etc....

![image](/assets/2025-02-13/020.png)

Later we want to show all open tasks in our list to the end user, so we need to filter out all completed task by using a simple dataflow with a filter for tasks with Status "notStarted".

![image](/assets/2025-02-13/030.png)

## Building the main screen

For the main screen we just use a styled list to present the task and bind this list to "OpenTasks" dataflow.

![image](/assets/2025-02-13/040.png)

In the list item editor we choose the attributes to be displayed. in our case it's only the title and the due date. Also we place a button on the item for the user to set a list item to "done".

![image](/assets/2025-02-13/050.png)

Let's look behind the code of the "done" button. Within the building blocks there are several blocks availabe to manipulate ToDo list items. In our case we just set "Task completed" to true. The ID of the task is accessible from the menu. It refers to the current instance of the styled list item. So literally we just need to connect those two blocks and combine it to one single command, followed by a refresh of the list.

![image](/assets/2025-02-13/060.png)

## Create a new task

To let the user create a new task, we just build a new screen and let the user jump to that new screen.

![image](/assets/2025-02-13/070.png)

The screen offers a text boxes for the title, description and due date of the new task.

![image](/assets/2025-02-13/080.png)

Let's jump into the Building Blocks of the "Create Task" command button. We just call another function that is offered by the ToDo data source. After the creation of the task we refresh the data source and jump back to the overview screen.

![image](/assets/2025-02-13/090.png)

## result

The short video shows the final result. First we create a new task and then set it to done.

![image](/assets/2025-02-13/result.gif)


