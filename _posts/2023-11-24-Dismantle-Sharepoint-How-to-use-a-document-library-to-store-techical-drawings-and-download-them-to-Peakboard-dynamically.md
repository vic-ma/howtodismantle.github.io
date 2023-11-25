---
layout: post
title: Dismantle SharePoint - How to use a document library to store technical drawings and download them in Peakboard dynamically
date: 2023-11-24 12:00:00 +0200
tags: bestpractice office365
image: /assets/2023-11-24/title.png
read_more_links:
  - name: What is a document library?
    url: https://support.microsoft.com/en-us/office/what-is-a-document-library-3b5976dd-65cf-4c9e-bf5a-713c10ca2872
---

We often see screens in factories with technical drawings of their products. This is most common in manual/semi-manual production and quality management. 

Besides purely technical drawings, other unstructured data is also displayed: step-by-step guides, quality check instructions, and other documentation. The de facto standard format for all these documents is PDF.

This article explains how to use a SharePoint document library to store these PDFs and download them in Peakboard Designer, so you can present them to end users.

## The SharePoint document library

The following screenshot shows a standard document library in a typical Office 365 environment. Within the library, there is a subdirectory called "Current," which contains all up-to-date documents. The old ones are stored in "Archive."

Let's pretend our company produces three different products, named P01-P03. So we create a technical drawing for each product and name the PDF accordingly. The file `x.pdf` just contains a red cross, and it is used as a placeholder, for when no useful data is presented.

![image](/assets/2023-11-24/010.png)

## Linking the Peakboard project to SharePoint

After creating a new Peakboard project, we add a new resource to the project (right click on resources -> cloud resource -> SharePoint). You need to authenticate against your O365 account and have enough permissions to access the document storage.

The hierarchical storage might get complicated in large organisations with many SharePoint sites or OneDrive instances. As you can see in the screenshot, if you dig deep enough, you will find the desired document library. You can select  `x.pdf` as the placeholder file for the project.

![image](/assets/2023-11-24/020.png)

Now we just drag and drop the PDF resource into the canvas.

![image](/assets/2023-11-24/030.png)

## Building the data structure for dynamic loading

Let's assume we want to let the factory worker choose the PDF from a list of document numbers. Therefore, we create a simple list, with those numbers as a "List" data type in the project explorer.

![image](/assets/2023-11-24/040.png)

Now, we need to give the user the ability to choose a product number from the list. We bind it to a combo box that we place above the PDF control.

![image](/assets/2023-11-24/050.png)

To use the combo box in the script, we need to give it a proper name.

![image](/assets/2023-11-24/060.png)

## The script for dynamic loading

All the magic happens in the "Selection changed" event. The logic is super simple:

1. Create the file path of the desired PDF, relative to the document library root, using the combo box value. For example, `"/Current/" + "p01" + ".pdf"`.
2. Set the source of the PDF control to the newly created file path.
3. This triggers a reload and lets the PDF control download and display the document from SharePoint.

![image](/assets/2023-11-24/070.png)

Here's an animated sequence of the result. Every time the value in the combo box is changed, the reload process is triggered and the PDF changes. Of course, we have a fixed list of products here. In real life, the list of products might be fetched dynamically from an ERP system like SAP.

![image](/assets/2023-11-24/080.gif)

## Conclusion

This article shows how easy it is to use Peakboard to present unstructured data like technical drawings. A SharePoint library is only one of the many options to store these kinds of documents. Other common ways include shared directories, cloud-based storage blobs, SAP, etc. The list is endless, but the pattern for how to handle the Peakboard side of things is always similar.
