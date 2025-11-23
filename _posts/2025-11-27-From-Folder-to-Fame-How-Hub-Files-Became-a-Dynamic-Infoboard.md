---
layout: post
title: From Folder to Fame – How Hub Files Became a Dynamic Bulletin Board
date: 2023-03-01 00:00:00 +0000
tags: peakboardhub usecase
image: /assets/2025-11-27/title.png
image_header: /assets/2025-11-27/title.png
bg_alternative: true
read_more_links:
  - name: Bulletin board app template
    url: https://templates.peakboard.com/Company_Information_PDF/en
---
Many people have been using Peakboard Hub as a file management system. And with [Peakboard version 4.1](/Peakboard-4.1-Is-Here-And-Its-a-Game-Changer.html), you can now use Hub files directly in your Peakboard applications, with the new *Hub files data source*. This opens the door to a wide array of exciting possibilities!

In today's article, we're going to explain how to use the new Hub files data source. We'll build a dynamic bulletin board application that lets the user view informational PDFs---similar to how a bulletin board works in the real world.

If you want to follow along, you can download the official Peakboard [bulletin board template](https://templates.peakboard.com/Company_Information_PDF/en).

## Add the PDFs to Peakboard Hub

Our first step is to upload our informational PDFs to Peakboard Hub. But we don't want to put our files directly in the root folder, because that would be messy. So instead, we create a folder to store all our bulletin board PDFs:
```
Template_Company_Information_PDF/
```

Next, we create sub-folders inside `Template_Company_Information_PDF/`, to group our PDFs by topic. This makes things more organized. It also lets our Peakboard app figure out which category each PDF belongs to, and use that information to show the user the different PDF files in each category.

Suppose we have three categories of PDFs:
* General announcements
* Lunch menus
* Shift schedules

We create one sub-folder for each PDF category:
```
Template_Company_Information_PDF/
├── Announcements/
├── Lunch/
└── Shifts/
```

### Another language

Now, suppose that we work in a bilingual workplace and all our PDFs have a German version too. We want to store these in Peakboard Hub as well, so that users who prefer German can choose the German versions of the PDFs. So, we create a German version of each sub-folder, where we will add the German PDFs:

```
Template_Company_Information_PDF/
├── Announcements/
├── Lunch/
├── Shifts/
├── Ankuendigungen/
├── Kantinenplan/
└── Schichtplan/
```

For simplicity, however, our demo app will not actually use these German PDFs.

### Add the PDFs

Now, we add all our PDFs into the appropriate sub-folders:
![image](/assets/2025-11-27/peakboard-hub-category-folder-structure.png)


### PDF file names

We give all our PDFs a name like this:
```
YYYY_MM_TITLE.pdf
```
That way, our app can figure out the date and title of each PDF by reading the file name.

## Create the Peakboard app

Now, let's create the Peakboard app.

### Add a Hub files data source
First, we add a Hub files data source. We set the *Path* to the PDFs folder we created in Peakboard Hub, `Template_Company_Information_PDF/`. We enable *Check subfolders*, so that the data source includes all our sub-folders. We click the preview reload button on the top-right, and it shows us all our PDFs.

![image](/assets/2025-11-27/peakboard-hub-files-data-source.png)


### Add a data flow

Next, we create a data flow to process data source's output. We'll use the data flow to clean up the data, parse the file names for the dates and titles, and sort and organize all the PDFs. Here's what the finished data flow looks like:

![image](/assets/2025-11-27/peakboard-data-flow-cleanup.png)

The first few steps of the flow are just basic cleanup. The last two steps are the interesting ones, where we parse the file names for the date and title.

#### Get the title

In the *Update column* step, we get the title from the file name and put it in the `Name` column. For example, we get `My Announcement` from the  `2025-05-MyAnnouncement.pdf` file. We do this by removing the date and underscores from the file name:

![image](/assets/2025-11-27/peakboard-update-column-extract-month.png)

This results in nice, human-readable titles:

![image](/assets/2025-11-27/peakboard-human-readable-file-names.png)

#### Add category data flows

Under the first data flow, we add an additional data flow for each PDF category. These data flows filter the output of the main data flow, in order to return only the files for their specific category.

![image](/assets/2025-11-27/peakboard-lunch-data-flow-filter.png)

## Build the dashboard

In the left pane of our application dashboard, we create three separate styled lists---one for each category.

In the right pane of our dashboard, we add a PDF control, which displays the PDFs. By default, it displays the empty `placeholder.pdf` file, which is located in our main directory.

![image](/assets/2025-11-27/peakboard-infoboard-layout-preview.png)

## Handle the styled list tapped events

Finally, we need to handle the tapped events of our styled lists. To do this, we open the template editor for each styled list and add a simple script that updates the PDF control.

We also store the index of the document ID, which is just a small enhancement. We use it to format the file name element with a different color after 

create the scripts to handle the tapped events of our styled lists. Each styled list needs its own script. It's in the template editor of our styled list and just sets the path of the central PDF control. Storing the index of the document ID is only a nice-to-have detail. It is used to format the file name element with a different color after clicking by using conditional formatting.

![image](/assets/2025-11-27/peakboard-tapped-event-conditional-formatting.png)

## Result

In this video, you can see our bulletin board app in action:
![image](/assets/2025-11-27/result.gif)

When building Peakboard apps that use Hub files, remember sure to name the Hub files according to some consistent pattern. This lets the app read metadata directly from the file name, and it reduces the complexity in maintaining the files dramatically.