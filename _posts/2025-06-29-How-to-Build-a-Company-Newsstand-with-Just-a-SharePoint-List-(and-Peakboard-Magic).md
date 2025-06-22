---
layout: post
title: How to Build a Company Newsstand with a SharePoint List (and Peakboard Magic)
date: 2023-03-01 00:00:00 +0000
tags: office365
image: /assets/2025-06-29/title.png
image_header: /assets/2025-06-29/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All Office 365 articles
    url: /category/office365
  - name: Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 ToDo
    url: /Elevate-Your-Workflow-Building-a-Next-Gen-Continuous-Improvement-Board-with-Office-365-ToDo.html
downloads:
  - name: CompanyNewsFromSharepoint.pbmx
    url: /assets/2025-06-29/CompanyNewsFromSharepoint.pbmx
---
Office 365 is often used as a data backend for all kinds of applications throughout a company.
We've discussed the technical aspects of combining Peakboard and Office 365 apps in various [Office 365 articles](/category/office365).

Today's article is about how to use Sharepoint lists for company news. The idea is, that the headline and article is maintained in a Sharepoint list from which various recipients can take it to spread into the world. This could be a newsletter, some kind of Sharepoint based intranet portal and a Peakboard application for the production workers. Either as part of other dashboards or as a standalone tool. 

To jump in directly to technical aspects, we remember the [getting started guide](/Getting-started-with-the-new-Office-365-Data-Sources.html) for the Office 365 datasources. Authenfication against the O365 backend is important and needs to be considered carefully. That's why it's worth a separate article.

Also there's another article about how to handle [Sharepoint lists](/SharePoint-Lists-in-Beast-Mode-Powered-by-Peakboard.html) in general. The difference to this article will be, that we will learn how to handle rich text (formatted text with Lists and other formatting options) and also how to combine the structured information of a list with other (unstructured) media like images, that are taken from a Sharepoint document library. 

The screenshot shows how the final result looks like. We have a list of news that automatically switched to the next headline every 10 seconds. It can by interactive too, depending if it runs on a touch screen. The actual news is a title, sub title, richly formatted text and finally an image shown next to the text on the right side.

![image](/assets/2025-06-29/010.png)

## Configure Sharepoint

We start with the document library for the images. It's actually a regular default sharepoint library for files, but it needs one more column to identify the document. We call it "Media ID". It's a just a string that can be filled with any information to identify the file (e.g. some unique term to decribe the picture or just the file name). We will need this column to make it easier for the user to find it later when creating a news entry.

![image](/assets/2025-06-29/020.png)

For the actual news we need a regular Sharepoint list. The screenshot shows all columns that are necessary in the list settings.

![image](/assets/2025-06-29/030.png)

There are two important things:

1. The article text should be marked as rich text

![image](/assets/2025-06-29/040.png)

2. The media column is a Lookup column that refers to the document library we created earlier. The MediaID column is used for easier handling and finding the right files.

![image](/assets/2025-06-29/050.png)

Here's the list with some sample data. The article text can contain rich formatting.

![image](/assets/2025-06-29/060.png)

## Configure the data source

The first step in the Peakboard application is to create a Sharepoint list data source that gets a list of entries from a document library, the media. We need this list to translate the Look up ID to the filename later.

![image](/assets/2025-06-29/070.png)

The second data source we need to get the actual news. In the screenshot we can see the Media column. It only contains the ID of the row of the list it refers to. Thats why we need this first list to translate this ID to the actual file name of the image.

![image](/assets/2025-06-29/080.png)

The actual translation happens with a data flow. We just joining the list of news with the list of the images by using a Join step as shown in the screenshot.

![image](/assets/2025-06-29/090.png)

A second data flow is used to filter the current list to only one news entry. That's the active one. So every time we need to refer to currently active news, we use this data flow.

![image](/assets/2025-06-29/100.png)

## Building the User Interface

On the left side we just use a Styled List to present the news list.

![image](/assets/2025-06-29/110.png)

On the right side we use an image control that refers to the media document library. An empty default image can be used as some kind of default view.

![image](/assets/2025-06-29/120.png)

So what happens when either the user touches on one of the news lines? Or the timer automaticaly rotates through the news lists?
The variable VarS_Selected_ID is set and the data flow to filter the news lines to the selected one is triggered. 

![image](/assets/2025-06-29/130.png)

This action in turn triggers the reload event of the data flow. It sets the actual news text and the file name of the image to be shown. The other news attributes (title and subtitle) don't need to be handled here. Because they are shown through data binding. We need this manual coding on for the formatted HTML text and the image source only.

![image](/assets/2025-06-29/140.png)

## result and conclusion

Using Sharepoint lists and documents is easy. But in this article we learned some sophistacted tricks:

1. How to combine list entries with files and how to handle this relationship in Peakboard
2. How to handle rich HTML text from Sharepoint

![image](/assets/2025-06-29/result.gif)
