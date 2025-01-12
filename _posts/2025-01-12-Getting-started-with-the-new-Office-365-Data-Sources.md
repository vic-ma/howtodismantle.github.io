---
layout: post
title: Getting started with the new Office 365 Data Sources
date: 2025-01-12 00:00:00 +0000
tags: office365
image: /assets/2025-01-12/title.jpg
image_header: /assets/2025-01-12/title_landscape.jpg
read_more_links:
  - name: All Office 365 articles
    url: /category/office365
  - name: Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 ToDo
    url: /Elevate-Your-Workflow-Building-a-Next-Gen-Continuous-Improvement-Board-with-Office-365-ToDo.html
---
Over a year ago, we published a series of articles about the [Microsoft Graph API](https://how-to-dismantle-a-peakboard-box.com/category/msgraph). We used the Graph API to access different objects from Office 365: calendars, SharePoint lists, Teams chats, and more.

In early 2025, Peakboard introduced new data sources that allow you access Office 365 objects without having to use the Graph API. The Graph API is still being used under the hood, but it's encapsulated in the data sources, making it much easier for Peakboard users.

In today's article, we will give a basic introduction of the new Office 365 data sources, focusing on authentication and authorization. That way, we won't need to repeat this information in future articles.

## Authorization methods

There are two methods for a data source to gain access to the Office 365 backend.

The first method is multi-tenant application. With this method, Peakboard uses a valid user account to log in, read, and potentially modify data. To make this work, the Office 365 admin must allow this kind of external access. This is the easier method, because there are no additional objects in the backend system to configure.

The second method, and the one recommended by Microsoft, is to go through a single-tenant application by using a registered app. The main advantage of this is that it gives the Office 365 admin full control over which permissions are granted. This lets the admin give only the necessary permissions. The admin can also withdraw the permissions from the app at any time, for all users, without impacting any other activity within the company. So when it comes to a large enterprise solution, this might be the way to go.

![image](/assets/2025-01-12/005.png)

## Multi-tenant access

For multi-tenant access, the only thing we need to do is the typical Office 365 authorization process. This is usually with 2FA or other steps, depending on the configuration of the security backend. 

![image](/assets/2025-01-12/006.png)

After granting Peakboard the appropriate permissions, the data source can be used immediately.

## Single-tenant application

The single-tenant application method is a bit trickier and requires us to set up a registered app in the Microsoft Entra ID backend. To do this, we go to the [Azure Portal](https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps). If that link doesn't work, you can always use the regular menu to access Microsoft Entra ID.

We can find the app registration on the menu on the left and create a new entry:

![image](/assets/2025-01-12/010.png)

We give the new registration a nice name and leave all values to default (unless otherwise stated):

![image](/assets/2025-01-12/020.png)

The most important step is to give the registered app the necessary permissions. The following screenshot shows a typical API permission. `Tasks` refers to the to-do items in Office, and `ReadWrite` refers to the permissions for reading and modifying the tasks. All potential permissions can be accessed through a tree of permissions that becomes visible when adding a new permission.

Besides the Graph API permissions, which are the basis for the Office 365 data source in Peakboard, there are other permissions available, like access to Power BI.

![image](/assets/2025-01-12/030.png)

The last thing we need to do is allow "public client flows" during the authentication process. This is mandatory for any external desktop applications like Peakboard Designer.

![image](/assets/2025-01-12/040.png)

We need to put the Application ID and Client ID of the registered app into the data source and then go through the authorization process.

![image](/assets/2025-01-12/050.png)

![image](/assets/2025-01-12/060.png)

## Result and conclusion

After successfully going through the authentication process, we can use the new data source immediately. It is highly recommended to reuse the connection when creating multiple data source based on Office 365 connectivity.

