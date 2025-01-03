---
layout: post
title: Getting started with the new Office 365 Data Sources
date: 2023-03-01 03:00:00 +0200
tags: office365
image: /assets/2025-01-12/title.jpg
image_header: /assets/2025-01-12/title_landscape.jpg
read_more_links:
  - name: All Office 365 articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/office365
  - name: Cracking the code - Part II - Calling functions remotely
    url: /category/msgraph
---
More than 1.5 years ago we introduced a series of articles around the [Microsoft Graph API](https://how-to-dismantle-a-peakboard-box.com/category/msgraph). We used the Graph API to access a lot of different objects from Microsoft's Office 365 offering: calendars, Sharepoint lists, Teams chats and a lot more...
In early 2025 Peakboard introduces some new data sources to meet the requirements to access these objects without knowing the details of the Graph API. The API is still used but it's encapsulated in the data sources, which makes it much easier for the Peakboard users to utilize it.
With today's article we will do some basic introduction of all Office 365 data sources - especially around athentification and authorization. So we don't need to repeat this information every time we use on the new components.

## Understanding Authorisation options

The access of the data source to the Office 365 backend can be done through two major ways. The first way is a "Multi-tenant Application" way, which means, that only one valid user account and nothing more is necessary to give Peakboard the permission to log on, read and potentially modify data. To make this work, the Office 365 admin must allow this kind of external access. Especially when we start with the first steps of extenral office 365 connectivity goind this way is easier to do as there's are no addtional objects in the backend systemto configure.

The second, and officially by Microsoft recommended way, is to go throgh a single-tenant application by using a registered App. The major advantage of using a registered app is, that it gives admins full control over any kind of permissions that are granted to one single application with giving more than necessary. And the admin can also withdraw the consent for the app at any time for all users without any impact on any other activity within the company. So when it comes to a large enterprise solution, this might be the way to go.

![image](/assets/2025-01-12/005.png)

## Multi-tenant access

For the multi tenant access the only thing we need to do is to go through a typical Office 365 authorisation process. Usually with 2FA or other additional steps - depending on the configuration of security backenend. 

![image](/assets/2025-01-12/006.png)

After confirming to give Peakboard the right to access the data the data source is authorized and can be used right away.

## Single-tenant Application

Using the second option of authentification is a bit more tricky and requires to set up a registered app in you Microsoft Entra ID backend, which can be reached though Azure Portal under this link: [https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps](https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps). If this link doesn't work we just use the regular menu to access Entry ID.

We can find the app registration on the menu on the left and create a new entry

![image](/assets/2025-01-12/010.png)

We give the new registration a nice name and leave all values to default unless otherwise stated here.

![image](/assets/2025-01-12/020.png)

The most important part is to give the registered app the necessary permission to let it do the right things in the name of the user who uses the app. The screenshot shows a typical API permission. "Tasks" stand for the ToDo items in the Office and the "ReadWrite" stands for reading and modifying the tasks. All potential permissions can be accessed through a tree of permissions that be access when adding a new one.
Beside the Graph API, which is the basis for the Office 365 data source in Peakboard, there are other permission available like access to Power BI.

![image](/assets/2025-01-12/030.png)

The last thing we need to do is to allow the "Public Client flow" for during the authenfication process. That is mandatory for any exnternal desktop application like the Peakboard designer.

![image](/assets/2025-01-12/040.png)

The Application ID and CLient ID of the registered app is what we need to put into the data source and then go through the authorisation process.

![image](/assets/2025-01-12/050.png)

![image](/assets/2025-01-12/060.png)

## result and conclusion

After sucssfully going through the process of authenfication we can use the data source right away. It is highly reommanded to re-use the connection when using multiple data source based on Office 365 connectivity.

