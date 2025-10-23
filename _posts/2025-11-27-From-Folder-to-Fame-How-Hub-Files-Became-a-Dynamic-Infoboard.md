---
layout: post
title: From Folder to Fame – How Hub Files Became a Dynamic Infoboard
date: 2023-03-01 00:00:00 +0000
tags: peakboardhub usecase
image: /assets/2025-11-27/title.png
image_header: /assets/2025-11-27/title.png
bg_alternative: true
read_more_links:
  - name: Template
    url: https://templates.peakboard.com/Company_Information_PDF/en
---
Since the latest version update in summer 2025 Peakbard Hub became increasingly popular to be used as a file management system. With the update to version 4.1 we can use a Hub File data source to query the meta data of the stored files. That opens a Window for exciting applications. We will discuss these options in today's article along with a detailed look into the data source. We will build a dynamic information board. The end user can upload pdf documents to the Hub file system and the application is dynamiclly showing the pdf file in different categories depending on the folder it's placed in. So the file system and the uploaded pdfs actually shape the appearance of the information board.

This project is also an official Peakboard template which can be download [here](https://templates.peakboard.com/Company_Information_PDF/en).

## Setting up the backend

For the backend we will set up a corresponding file structure. Let's assume we have three different categories: general announcement, lunch menu and a shift overview for in form the people who has to work in which shift during a certain week. As we see in the screenshot we build three directories for each of those categories: Announcementments, Lunch, Shifts. Because we might want to have a bi-lingual board we can do the same in a different language, e.g. German: Ankuendigungen, Kantinenplan, Schichtplan.

Each directory contains the information to be displayed as pdf file. The naming of the pdf files can be also used later so we follow a certain pattern.

![image](/assets/2025-11-27/010.png)

## Building the data backend

In out app we use a Peakboard Hub Files data source and let it point to the main folder we created earlier. With the option "Check subfolders" we let him read down all sub folders. The sample data lists all files that are in or below our main folder.

![image](/assets/2025-11-27/020.png)

All the data magic is happening in data flows. The screenshot shows that we get rid of useless columns and sort the data.

![image](/assets/2025-11-27/030.png)

In an Update Column step we extract the month from the file name, e.g. the file `2025-05-MyAnnounement.pdf` can be extracted to the month `2025-05`, so it can used later to be displayed.

![image](/assets/2025-11-27/040.png)

SO the month meta information and other rubbiesh like `_` are also removed from the file name and generates a nice human readable file name as shown in the screenshot.

![image](/assets/2025-11-27/050.png)

Below the first data flow we add an additional dataflows for each of our categories. The screenshots show how only the lunch related files are filtered and sorted in the lunch file related data flow.

![image](/assets/2025-11-27/060.png)

## Building the visuals

In the left pane of our application we place a styled list for each of the categories to present the file list to the user. The main part in the right center is a pdf controle that points to the empty `placeholder.pdf` which is located in our main directory.

![image](/assets/2025-11-27/070.png)

The last thing we need is the tapped event in case the user taps on a file to view it. It's in the template editor of our styled list and just sets the path of the central pdf control. Storing the index of the document ID is only a nice to have point. It is used to format the file name element with a different color after clicking by using conditional formatting.

![image](/assets/2025-11-27/080.png)

## result

In the video we can see our baord in action. It's very important to understand, that the structure and the naming of the file in the file system is used to to stcurture the actual visual. That reduces complexity for those people who maintain the files dramatically.

![image](/assets/2025-11-27/result.gif)
