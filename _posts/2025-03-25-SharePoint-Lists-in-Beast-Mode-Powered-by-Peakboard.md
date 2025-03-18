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
downloads:
  - name: Office365Sharepointlist.pbmx
    url: /assets/2025-03-25/Office365Sharepointlist.pbmx
---
With the first big update in 2025 the Peakboard dev team launched a new series of Office 365 data sources. Office 365 has become more and more important as a backend for lots of different use cases in companies. In today's article we will discuss how to read and write and Sharepoint lists from Peakboard applications. For other Office 365 related topics we can check [this overview](/category/office365).

Sharepoint lists might by a wise choice to store data especially when the data is used or processed within the Office 365 universe, e.g. Power Automate.

Every Office 365 data source has the same options and principles of authentifction against the Office 365 backend. We won't discuss the details about aunthentification here as it's already handled in [this article](/Getting-started-with-the-new-Office-365-Data-Sources.html). Is you're not familiar, please read in advance.

As an example we're using an issue tracker list in Sharepoint. This issue tracker list has the purpose that everyone can add and track problems within the factory. Beside a title and description text, it comes with some more interesting columns to discuss. An "Assigned To" column that contains a link to a SharePoint user, "Date Reported" column with a date, and a Status column that is translated into a symbol. It this article we will discuss how to read, process and write all of these special columns.

![image](/assets/2025-03-25/010.png)

## Configure the datasource

After authentification to the Office 365 backend we need to provide the name of the Sharepoint site and the Sharepoint list. Below these combo boxes we find the setting "Show user display name". We'll discuss this later. The first column of each SHarepoint list always called "ID" and contains a unique number representing the row. It will be needed for modifying or deleting the data.  

![image](/assets/2025-03-25/020.png)

## Date values

The first special column is "DateReported". The original date's format depends on the language settings of the users and the site: e.g. "1/30/2025 8:00:00 AM". For the formatting in a control the regular default formatting of Peakboard can be used. Peakboard automatically realizes the date and is doing the expected formatted automatically without any flaws. The screenshot shows a German format used in the table.

![image](/assets/2025-03-25/030.png)

![image](/assets/2025-03-25/035.png)

The same will work when setting a date during record crateation through a building block. In the example we use a technical format: YYYY-MM-DD.

![image](/assets/2025-03-25/036.png)

## Links to users

Columns that contains references to Sharepoint users by default come with a large JSON string to describe the user, including the name, email address and lots of other things. 
If we need only the name, we can check the setting "Show user display name" in the data source. To get more attributes of the user we leave the setting to fales. This example show the JSON string that is contained in the column in the complex case:

{% highlight json %}
{
  "@odata.etag": "\"e8212151-de65-49b5-a2b5-073d4963d2f0,6\"",
  "Title": "Walter White",
  "Name": "i:0#.f|membership|X@X.com",
  "EMail": "o365_test@peakboard.com",
  "SipAddress": "dismantlebot@XX.com",
  "IsSiteAdmin": false,
  "Deleted": false,
  "UserInfoHidden": false,
  "Picture": {
    "Description": "https://XX-my.sharepoint.com:443/User%20Photos/Profilbilder/5afad2cc-c72b-491a-8515-a062b91d514e_MThumb.jpg?t=63838484300",
    "Url": "https://XX-my.sharepoint.com:443/User%20Photos/Profilbilder/5afad2cc-c72b-491a-8515-a062b91d514e_MThumb.jpg?t=63838484300"
  },
  "JobTitle": "X",
  "FirstName": "Walter",
[...]
  "ContentTypeDisp": "Walter White"
}
{% endhighlight %}

To get the information we want, we just use a "Parse table data from JSON" transformation step within a data flow. The sceenshot shows how to extract the "First Name" from the string and put it in a separate column.

![image](/assets/2025-03-25/040.png)

To set a value of a Sharepoint list we can it through the email address, the display name or the internal ID. In most cases the email address is the easiest way to do:

![image](/assets/2025-03-25/045.png)

## Columns with symbols and functions

In our example list there are columns, which are translated into symbols or have other functions. The Status value "Blocked" let the row turn red in Sharepoint.

![image](/assets/2025-03-25/050.png)

These columns are pure string columns internally and will be handled like this by the Peakboard data source. So only the pure value will be displayed in the table output.

![image](/assets/2025-03-25/051.png)

The same principle is applied when setting the content. It's important to match the value exactly to trigger the translation into symbols or other effects on the Sharepoint side.

![image](/assets/2025-03-25/055.png)

## conclusion

With the new data source, Sharepoint lists can be easily read and modified. With certain columns types we need to understand the internal principals and use them correctly.


