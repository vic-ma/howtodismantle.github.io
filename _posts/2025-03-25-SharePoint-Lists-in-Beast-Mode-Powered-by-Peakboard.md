---
layout: post
title: SharePoint Lists in Beast Mode – Powered by Peakboard
date: 2023-03-01 00:00:00 +0000
tags: office365
image: /assets/2025-03-25/title.jpg
image_header: /assets/2025-03-25/title_landscape.jpg
bg_alternative: true
read_more_links:
  - name: All Office 365 articles
    url: /category/office365
  - name: Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 ToDo
    url: /Elevate-Your-Workflow-Building-a-Next-Gen-Continuous-Improvement-Board-with-Office-365-ToDo.html
---
With the first big update in 2025 the Peakboard dev team launched a new series of Office 365 data sources. Office 365 has become more and more important as a backend for lots of different use cases in companies. In today's article we will discuss how to read and write and Sharepoint lists from Peakboard applications. For other Office 365 related topics we can check [this overview](/category/office365).

Sharepoint lists might by a wise choice to store data especially when the data is used or processed within the Office 365 universe, e.g. Power Automate.

Every Office 365 data source has the same options and principles of authentifction against the Office 365 backend. We won't discuss the details about aunthentification here as it's already handled in [this article](/Getting-started-with-the-new-Office-365-Data-Sources.html). Is you're not familiar, please read in advance.

As an example we're using an issue tracker list in Sharepoint. This issue tracker list has the purpose that everyone can add and track problems within the factory. Beside a title and descirption text, it comes with some more interesting columns to discuss. An "AssignedTo" column that contains a link to a SharePoint user, "Date Reported" column with a date, and a STatus column that is translated into a symbol. It this article we will discuss how to read, process and write all of these special columns.

![image](/assets/2025-03-25/010.png)

