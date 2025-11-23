---
layout: post
title: From Folder to Fame – How Hub Files Became a Dynamic Bulletin Board
date: 2023-03-01 00:00:00 +0000
tags: peakboardhub usecase
image: /assets/2025-11-27/title.png
image_header: /assets/2025-11-27/title.png
bg_alternative: true
read_more_links:
  - name: Infoboard template
    url: https://templates.peakboard.com/Company_Information_PDF/en
---
Many people have been using Peakboard Hub as a file management system. And with [Peakboard version 4.1](/Peakboard-4.1-Is-Here-And-Its-a-Game-Changer.html), you can now use Hub files directly in your Peakboard applications, with the new *Hub files data source*. This opens the door to wide array of exciting possibilities!

In today's article, we're going to explain how to use the new Hub files data source. To do this, we'll build a dynamic bulletin board application, where the user can view and upload PDFs. The PDFs are all stored in Peakboard Hub, and we use folders to organize the PDFs into separate categories and languages:
```
Bulletin_Board_Files/
  Announcements/
  Lunch/
  Shifts/
  Ankuendigungen/
  Kantinenplan/
  Schichtplan/
```

This lets our application organize the PDFs by category, just like how bulletin boards in real life.

This project is also an official Peakboard template, [which you can downloaded](https://templates.peakboard.com/Company_Information_PDF/en).

## Set up the PDFs in Peakboard Hub

First, we need to create the folder structure in Peakboard Hub and add the PDFs.

Let's assume that we have three different categories:
* General announcements
* Lunch menu
* Shift schedules

Let's also assume that we have a bilingual workplace, and we want 
Because we might want to have a bilingual board we can do the same in a different language, e.g. German: Ankuendigungen, Kantinenplan, Schichtplan.

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

In this video we can see our board in action. It's very important to understand that the structure and the naming of the files in the file system are used to structure the actual visual. That reduces complexity for those people who maintain the files dramatically.

![image](/assets/2025-11-27/result.gif)
