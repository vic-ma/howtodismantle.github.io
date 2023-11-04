---
layout: post
title: Dismantle Sharepoint - How to use a document library to store techical drawings and download them to Peakboard dynamically
date: 2023-03-01 12:00:00 +0200
tags: bestpractice peakboardhub
image: /assets/2023-11-24/title.png
read_more_links:
  - name: SQL Joins
    url: https://www.w3schools.com/sql/sql_join.asp
---

We often see screens in factories with techical drawings of the related products. The most common use cases for this is either in manual or semi-manual production or quality management. Beside a pure techical drawing other unstructured data is also presented: step-by-step guides, quality check instructions or other documentations. The de-facto standard for all these documents is pdf.
This article shows the basic pattern how to use a Sharepoint document library to store these pdfs and download them to Peakboard for presenting them to the end users.

## The Sharepoint document library

The screenshot shows just a regular document library in a typical Office 365 environment. Within the linbrary we see a subdirectory called "Current" that contains all up-to-date documents. The old ones are somewhere in "Archive". Let's pretend our comapny is producing three different products named P01-P03, so we create a techical drawing for each product and name the pdf accordingly. The pdf x.pdf just contains a red cross and is used as a placeholder for situations, when no useful data is presented.

![image](/assets/2023-11-24/010.png)

## Linking the Peakboard project to Sharepoint

After creating a new Peakboard project we add a new resource to project (right click on resources -> cloud resource -> Sharepoint). You need to authentitcate against your O365 account and have enough rights to access the document storage. The hierarchical storage might get complicated in very large organisations with many Sharepoint sites or OneDrive instances. As you see in the screenshot, if you drill deep enough you will find the needed document library and can select the x.pdf as the ultimate placeholder for the project.

![image](/assets/2023-11-24/020.png)

Now we just drag and drop the pdf resource to the canvas.

![image](/assets/2023-11-24/030.png)

## Building the data structure for dynamic loading

Let's assume we want to let the worker choose the pdf from a list of article numbers. Therefor we create a simple list with those numbers as a "List" data type in the project explorer.

![image](/assets/2023-11-24/040.png)

For giving the user the opportunity to chooses a product number from the list, we bind it to a combo box that we have placed above the pdf control.

![image](/assets/2023-11-24/050.png)

For using the combo in the script, it needs a proper name.

![image](/assets/2023-11-24/060.png)

## The script for dynamic loading

Al the magic happens in the "Selection changed" event. The logic is supersimpel. From the choosen value of the combo box a string is concatenated containing the path and name of the pdf document relativ to the document libary root. In our case seomthing like "/Current/MyArticleNo.pdf".
The second step is then to set the property "Source" of the pdf control with the newly crated document name. Setting this property triggers a reload and let's the pdf control download and show the document from Sharepoint.

![image](/assets/2023-11-24/070.png)

Here's a animated sequence of the result. Every time the value in the combo box is changed the reload process is triggered and the pdf changes. Of course we have a fixed list of products here. In real life the list of products might be fetched dynamically from an ERP system like SAP.

![image](/assets/2023-11-24/080.gif)

## Conclusion

This article shows how easy it is to use Peakboard to present unstructured data like techincal drawings. A Sharepoint library is only one of many option to store these documents. Other common way would be shared directories, cloud based storage blobs, SAP... the list is endless but the pattern how to to address it from Peakboard is always similiar.
