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
Office 365 is often used as a data backend for different applications within a company.
In previous articles, we explained how to combine [Peakboard and Office 365 apps](/category/office365). In this article, we explain how to use SharePoint lists to build a newsstand app that shows the latest company news.

Before continuing, make sure that you have read our [Office 365 data sources guide](/Getting-started-with-the-new-Office-365-Data-Sources.html). The authentication process is tricky, so it's important that you understand it.

We also have another [SharePoint lists guide](/SharePoint-Lists-in-Beast-Mode-Powered-by-Peakboard.html) that explains how to use SharePoint lists in Peakboard. That article is more general, while this article covers the following specialized topics:
* How to handle text with rich formatting.
* How to combine structured data from a SharePoint list with unstructured data (like images) from a SharePoint document library. 

## The big picture

We will create a Peakboard app that showcases the most recent company news. Here's what the final result looks like:

![image](/assets/2025-06-29/010.png)

Notice:
* There's a list of the most recent news articles on the left.
* There's a full article in the middle.
* There's an image for the full article on the right.

The full article and associated image changes automatically every 10 seconds, cycling through the list of news articles. Users can also manually select an article to display, if the app is running on a touch screen.

The app pulls the news articles from a SharePoint list. The idea is that this SharePoint list lets anyone take the news articles and publish them somewhere. For example, in a newsletter, an intranet portal, or a Peakboard app (either as part of other dashboards, or as a standalone tool).

The app pulls the images from a SharePoint document library.

## Configure SharePoint

Here's what we need to do on the SharePoint side of things.

### Store images

The first step is to create a document library to store the images for our news articles. A document library is like a standard SharePoint file library---but with one additional column: `Media ID`. This column contains a string that identifies the file (for example, a file name). We need this column to make it easy for users to select an image to use with their news article.

![image](/assets/2025-06-29/020.png)

### Store news articles

Next, we create a standard SharePoint list to store our news articles. We need the following columns:

| Column name   | Column type            |
| ------------- | ---------------------- |
| `Title`       | Single line of text    |
| `Subtitle`    | Single line of text    |
| `ArticleText` | Multiple lines of text |
| `PublishDate` | Date and Time          |
| `Media`       | Lookup                 |

<!-- | `NewsType` | Choice | -->

![image](/assets/2025-06-29/030.png)

There are two important things to do when creating the columns.

First, for the `ArticleText` column, we enable [*Enhanced rich text* ](https://support.microsoft.com/en-us/office/edit-a-rich-text-list-column-6ba62e7e-ee63-4716-9f95-f626770c3fff). That way, we support articles with formatted text, like italics or pictures or hyperlinks.

![image](/assets/2025-06-29/040.png)

For the `Media` [lookup column](https://support.microsoft.com/en-us/office/create-list-relationships-by-using-lookup-columns-80a3e0a6-8016-41fb-ad09-8bf16d490632), we set the source to the `MediaID` column of our image document library (`CompanyNewsMedia.MediaID`).

![image](/assets/2025-06-29/050.png)

Here's our list with some example data. You can see how we're able to use bold text, because we enabled *enhanced rich text*.

![image](/assets/2025-06-29/060.png)

## Build the Peakboard app

Now, let's take a look at the Peakboard side.

### Add the document library data source

First, we create a SharePoint list data source that pulls images from our document library. We need this data source in order to translate a lookup ID into a filename.

![image](/assets/2025-06-29/070.png)

### Add the news articles data source

Next, we create a SharePoint list data source that pulls news articles from our SharePoint list. 

![image](/assets/2025-06-29/080.png)

In the above screenshot, you can see that the `Media` column contains the `ID`  of an image in our document library. That's why we need to translate this ID into the actual filename of the image.

To do this, we create a data flow with a single *join* step. This step joins the list of news articles with the list of images:

![image](/assets/2025-06-29/090.png)

We use a second data flow to filter the current list, to only show the active news entry. So each time we need to get the active news article, we use this data flow.

![image](/assets/2025-06-29/100.png)

### Build the user interface

We add a *styled list* to show the list of news articles, on the left side of the screen:

![image](/assets/2025-06-29/110.png)

We add an *image control* to show the image for the active article, on the right side of the screen. This image controls gets its image from our document library data source. We also set a default image.

![image](/assets/2025-06-29/120.png)

### Implement article switching

There are two ways that the active article can switch:
* The 10 second timer expires.
* A user taps one of the news articles.

We set the variable `VarS_Selected_ID` to the `ID` of the new active article (the next one in the list, or the one a user tapped). the data flow to filter the news lines to the selected one is triggered. 

![image](/assets/2025-06-29/130.png)

This action in turn triggers the reload event of the data flow. It sets the actual news text and the file name of the image to be shown. The other news attributes (title and subtitle) don't need to be handled here. Because they are shown through data binding. We need this manual coding on for the formatted HTML text and the image source only.

![image](/assets/2025-06-29/140.png)

## Result and conclusion

It's easy to use basic SharePoint lists and documents. But in this article, we learned some more sophisticated tricks:
* How to combine list entries with files, and how to handle this relationship in Peakboard.
* How to handle rich HTML text from SharePoint.

![image](/assets/2025-06-29/result.gif)
