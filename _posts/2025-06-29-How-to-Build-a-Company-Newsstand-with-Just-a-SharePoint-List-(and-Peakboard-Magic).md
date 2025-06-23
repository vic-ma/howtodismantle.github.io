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
Office 365 is often used as a data backend for various applications within a company.
We've explained how to combine [Peakboard and Office 365 apps](/category/office365) in previous articles.
In this article, we explain how to use SharePoint lists to share company news.

Before continuing, make sure that you have read our [Office 365 data sources guide](/Getting-started-with-the-new-Office-365-Data-Sources.html). The authentication process is tricky, so it's important that you understand how to do it.

We also have another [SharePoint lists guide](/SharePoint-Lists-in-Beast-Mode-Powered-by-Peakboard.html). That article is more general, while this article covers the following specialized topics:
* How to handle rich text (formatted text with lists and other formatting options).
* How to combine structured data from a SharePoint list with unstructured data (like images) from a SharePoint document library. 

## The big picture

We will create a Peakboard app that shows the most recent company news. This screenshot shows what the final result looks like:

![image](/assets/2025-06-29/010.png)

* There's a list of news articles on the left.
* There's a full article in the middle.
* THere's an image for the news article on the right.

The article and associated image changes automatically every 10 seconds, by cycling through the list of news articles. Viewers can also manually select an article to display, if the app is running on a touch screen.

The app pulls the news articles from a SharePoint list. The idea is that this SharePoint list lets anyone take the news articles and publish them somewhere. For example, in a newsletter, an intranet portal, or a Peakboard app (either as part of other dashboards, or as a standalone tool). The app pulls the images from a SharePoint document library.

## Configure SharePoint

### Store images

The first step is to create a document library to store the images for our news articles. A document library is a standard SharePoint file library, but with one additional column: `Media ID`. This column is a string that identifies the file (for example, a file name). We need this column to make it easy for users to choose an image to use with their news article.

![image](/assets/2025-06-29/020.png)

### Store news articles

Next, we create a standard SharePoint list to store our news articles. We add the following columns:

| Column name   | Column type            |
| ------------- | ---------------------- |
| `Title`       | Single line of text    |
| `Subtitle`    | Single line of text    |
| `ArticleText` | Multiple lines of text |
| `PublishDate` | Date and Time          |
| `Media`       | Lookup                 |

<!-- | `NewsType` | Choice | -->

![image](/assets/2025-06-29/030.png)

There are two important things:

1. The article text should be marked as rich text

![image](/assets/2025-06-29/040.png)

2. The media column is a Lookup column that refers to the document library we created earlier. The MediaID column is used for easier handling and finding the right files.

![image](/assets/2025-06-29/050.png)

Here's the list with some sample data. The article text can contain rich formatting.

![image](/assets/2025-06-29/060.png)

## Configure the data source

The first step in the Peakboard application is to create a SharePoint list data source that gets a list of entries from a document library, the media. We need this list to translate the Look up ID to the filename later.

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

Using SharePoint lists and documents is easy. But in this article we learned some sophistacted tricks:

1. How to combine list entries with files and how to handle this relationship in Peakboard
2. How to handle rich HTML text from SharePoint

![image](/assets/2025-06-29/result.gif)
