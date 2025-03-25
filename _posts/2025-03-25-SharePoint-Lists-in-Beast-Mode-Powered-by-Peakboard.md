---
layout: post
title: SharePoint Lists in Beast Mode – Powered by Peakboard
date: 2025-03-25 00:00:00 +0000
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
In the first major update of 2025, the Peakboard dev team added a new series of Office 365 data sources. Office 365 has become increasingly important as a backend for many companies, with many different use cases. In today's article, we'll discuss how to read and write SharePoint lists from Peakboard applications. You can also check out our other [Office 365 articles](/category/office365).

SharePoint lists may be a wise choice for data storage, especially when the data is used or processed within the Office 365 ecosystem (e.g., in Power Automate).

Every Office 365 data source handles authentication in the same way. To learn more about how to authenticate against the Office 365 backend, see our [getting started guide](/Getting-started-with-the-new-Office-365-Data-Sources.html).

For this article, we'll use an issue tracker list in SharePoint as an example. This issue tracker list lets anyone add and track problems in the factory. It has a title and description, along with some more interesting columns:
* `Assigned To` - a link to a SharePoint user.
* `Date Reported` - the date that the report was made.
* `Status` - a status string that transforms into a symbol in SharePoint.

It this article, we'll explain how to read, process, and write to all of these special columns.

![image](/assets/2025-03-25/010.png)

## Configure the data source

After successfully authenticating with the Office 365 backend, we provide the name of our SharePoint site and SharePoint list, using the combo boxes. Below these combo boxes, there's a **Show user display name** option. We'll discuss this later.

The first column of each SharePoint list always `ID`. It contains a unique number representing the row. We need the `ID` in order to modify or delete data.

![image](/assets/2025-03-25/020.png)

## Date values

The first special column is `DateReported`. The original date format depends on the language settings of the site and the users (e.g., `1/30/2025 8:00:00 AM`).

To properly format the date in a control, we use the default Peakboard formatter. Peakboard automatically detects the date format and can handle it without any issues. The following screenshot shows a German date format used in the table:

![image](/assets/2025-03-25/030.png)

![image](/assets/2025-03-25/035.png)

The same thing works when setting a date during record creation, using Building Blocks. In the following example, we use a more technical format (`YYYY-MM-DD`):

![image](/assets/2025-03-25/036.png)

## Links to users

By default, columns that contain references to SharePoint users come with a large JSON string that describes the user. This string includes the user's name, email address and many other things.

If we only need the name, we can enable the setting, **Show user display name**, in our data source. To get additional attributes of the user, we disable the setting. This is what a complete JSON string looks like:

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

To get the information we want, we add a "Parse table data from JSON" transformation step within a data flow. The following screenshot shows how to extract the `FirstName` from the JSON string and put it in a separate column.

![image](/assets/2025-03-25/040.png)

To set the value of a SharePoint list, we can use the person's email address, display name, or internal ID. In most cases, the email address is the easiest method:

![image](/assets/2025-03-25/045.png)

## Columns with symbols and functions

In our example list, there are columns that are translated into symbols, or have other functions. The Status value `Blocked` turns the row red in SharePoint.

![image](/assets/2025-03-25/050.png)

Internally, these columns are simple strings, and they will be treated as such by the Peakboard data source. So they won't have any color or formatting in the Peakboard table output:

![image](/assets/2025-03-25/051.png)

The same principle is applied when setting the content. It's important to match the value exactly, in order to trigger the translation into symbols or use other effects on the SharePoint side.

![image](/assets/2025-03-25/055.png)

## Conclusion

With the new data sources, SharePoint lists can now be easily read and modified. With certain columns types, we need to understand the internal principals and use them correctly.


