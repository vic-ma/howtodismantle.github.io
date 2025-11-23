---
layout: post
title: From Folder to Fame – How Hub Files Became a Dynamic Infoboard
date: 2023-03-01 00:00:00 +0000
tags: peakboardhub usecase
image: /assets/2025-11-27/title.png
image_header: /assets/2025-11-27/title.png
bg_alternative: true
read_more_links:
  - name: Infoboard template
    url: https://templates.peakboard.com/Company_Information_PDF/en
---
More and more people have been using Peakboard Hub as a file management system. And with [Peakboard version 4.1](/Peakboard-4.1-Is-Here-And-Its-a-Game-Changer.html), you can now pull Hub files (and all associated metadata) directly into your Peakboard applications, by using the new *Hub files data source*---opening the door to wide array of exciting applications.

We will discuss these options in today's article along with a detailed look into the data source. We will build a dynamic information board. The end user can upload PDF documents to the Hub file system, and the application dynamically shows the PDFs in different categories depending on the folder they are placed in. So the file system and the uploaded PDFs actually shape the appearance of the information board.

This project is also an official Peakboard template which can be downloaded [here](https://templates.peakboard.com/Company_Information_PDF/en).

## Setting up the backend

For the backend we will set up a corresponding file structure. Let's assume we have three different categories: general announcements, lunch menu, and a shift overview to inform the people who have to work in which shift during a certain week. As we see in the screenshot we build three directories for each of those categories: Announcements, Lunch, Shifts. Because we might want to have a bilingual board we can do the same in a different language, e.g. German: Ankuendigungen, Kantinenplan, Schichtplan.

Each directory contains the information to be displayed as a PDF file. The naming of the PDF files can also be used later so we follow a certain pattern.

![image](/assets/2025-11-27/peakboard-hub-category-folder-structure.png)

## Building the data backend

In our app we use a Peakboard Hub files data source and let it point to the main folder we created earlier. With the option "Check subfolders" we let it scan all subfolders. The sample data lists all files that are in or below our main folder.

![image](/assets/2025-11-27/peakboard-hub-files-data-source.png)

All the data magic happens in data flows. The screenshot shows that we get rid of useless columns and sort the data.

![image](/assets/2025-11-27/peakboard-data-flow-cleanup.png)

In an Update Column step we extract the month from the file name, e.g. the file `2025-05-MyAnnouncement.pdf` can be extracted to the month `2025-05`, so it can be used later to be displayed.

![image](/assets/2025-11-27/peakboard-update-column-extract-month.png)

So the month metadata and other rubbish like `_` are also removed from the file name, generating a nice human-readable file name as shown in the screenshot.

![image](/assets/2025-11-27/peakboard-human-readable-file-names.png)

Below the first data flow we add additional data flows for each of our categories. The screenshots show how only the lunch-related files are filtered and sorted in the lunch file-related data flow.

![image](/assets/2025-11-27/peakboard-lunch-data-flow-filter.png)

## Building the visuals

In the left pane of our application we place a styled list for each of the categories to present the file list to the user. The main part in the right center is a PDF control that points to the empty `placeholder.pdf` which is located in our main directory.

![image](/assets/2025-11-27/peakboard-infoboard-layout-preview.png)

The last thing we need is the tapped event in case the user taps on a file to view it. It's in the template editor of our styled list and just sets the path of the central PDF control. Storing the index of the document ID is only a nice-to-have detail. It is used to format the file name element with a different color after clicking by using conditional formatting.

![image](/assets/2025-11-27/peakboard-tapped-event-conditional-formatting.png)

## Result

In the video we can see our board in action. It's very important to understand that the structure and the naming of the files in the file system are used to structure the actual visual. That reduces complexity for those people who maintain the files dramatically.

![image](/assets/2025-11-27/result.gif)
