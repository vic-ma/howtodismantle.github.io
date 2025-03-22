---
layout: post
title: SharePoint Lists in Beast Mode – Powered by Peakboard
date: 2023-03-01 00:00:00 +0000
tags: office365
image: /assets/2025-03-25/title.png
image_header: /assets/2025-03-25/title_landscape.png
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
In the first major update of 2025, the Peakboard dev team added a new series of Office 365 data sources. Office 365 has become increasingly important as a backend for a lot of companies, with many different use cases. In today's article, we'll discuss how to read and write SharePoint lists from Peakboard applications. For other Office 365 related topics, see this [Office 365 overview](/category/office365).

SharePoint lists may be a wise choice for data storage, especially when the data is used or processed within the Office 365 universe (e.g., Power Automate).

Every Office 365 data source handles authentication in the same way. To learn more about how to authenticate against the Office 365 back end, see our [authentication guide](/Getting-started-with-the-new-Office-365-Data-Sources.html).

For this article, we'll use an issue tracker list in SharePoint as an example. This issue tracker list lets anyone add and track problems in the factory. It has a title and description text, along with some more interesting columns:
* `Assigned To` - a link to a SharePoint user.
* `Date Reported` - the date that the report was made.
* `Status` - a status string.

It this article, we'll discuss how to read, process, and write all of these special columns.

![image](/assets/2025-03-25/010.png)

## Configure the data source

After successfully authenticating to the Office 365 backend, we provide the name of the SharePoint site and the SharePoint list using the two combo boxes. Below these combo boxes, there's a **Show user display name** options. We'll discuss this later.

The first column of each SharePoint list always `ID`. It contains a unique number representing the row. We need this in order to modify or delete the data.  

![image](/assets/2025-03-25/020.png)

## Date values

The first special column is `DateReported`. The original date format depends on the language settings of the site and the users (e.g. `1/30/2025 8:00:00 AM"`). To properly format the date in a control, we can use the default Peakboard formatter. Peakboard automatically detects the date format and can handle it without any issues. The following screenshot shows a German format used in the table:

![image](/assets/2025-03-25/030.png)

![image](/assets/2025-03-25/035.png)

The same will work when setting a date during record crateation through a building block. In the example we use a technical format: YYYY-MM-DD.

![image](/assets/2025-03-25/036.png)

## Links to users

Columns that contains references to SharePoint users by default come with a large JSON string to describe the user, including the name, email address and lots of other things. 
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

To set a value of a SharePoint list we can it through the email address, the display name or the internal ID. In most cases the email address is the easiest way to do:

![image](/assets/2025-03-25/045.png)

## Columns with symbols and functions

In our example list there are columns, which are translated into symbols or have other functions. The Status value "Blocked" let the row turn red in SharePoint.

![image](/assets/2025-03-25/050.png)

These columns are pure string columns internally and will be handled like this by the Peakboard data source. So only the pure value will be displayed in the table output.

![image](/assets/2025-03-25/051.png)

The same principle is applied when setting the content. It's important to match the value exactly to trigger the translation into symbols or other effects on the SharePoint side.

![image](/assets/2025-03-25/055.png)

## conclusion

With the new data source, SharePoint lists can be easily read and modified. With certain columns types we need to understand the internal principals and use them correctly.


