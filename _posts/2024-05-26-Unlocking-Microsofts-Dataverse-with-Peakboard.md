---
layout: post
title: Unlocking Microsoft's Dataverse with Peakboard
date: 2024-05-28 02:00:00 +0200
tags: bestpractice office365
image: /assets/2024-05-26/title.png
read_more_links:
  - name: Dismantle SharePoint - How to use a document library to store technical drawings and download them in Peakboard dynamically
    url: /Dismantle-Sharepoint-How-to-use-a-document-library-to-store-techical-drawings-and-download-them-to-Peakboard-dynamically.html
  - name: MS Graph API - Understand the basics and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
  - name: Peakboard Dynamics Extension on GitHub
    url: https://github.com/Peakboard/PeakboardExtensions/tree/master/MicrosoftDynamics365
downloads:
  - name: D365GetOrders.pbmx
    url: /assets/2024-05-26/D365GetOrders.pbmx
---
The backbone of any Dynamics 365 application is Microsoft Dataverse. This is true for standard D365 apps like CRM or Business Central, but also for any kind of Power App. So, Dataverse can be seen as a kind of cloud-based database. 

This article is a step-by-step guide on how to access Dataverse entities. The tricky part is the authentication process. In general, accessing the Dataverse data with a regular username and password is possible, but definitely not recommended.

In this article, we will register an app in a Microsoft Entra directory and then use this app to access the data. This is much more secure than using a username and password.

## Install the Dynamics extension

Peakboard doesn't natively support Dataverse, so we have to install the [Dynamics extension](https://templates.peakboard.com/extensions/Microsoft-Dynamics-365/en) first. We click on **Manage Extension** and choose the Dynamics extension. After restarting Peakboard Designer, the list is available to be used.

The extension name is a bit confusing and inaccurate, due to historical reasons. To be more accurate, we can say that the extension is a *Dataverse* extension and what we query is an *entity list*.

![image](/assets/2024-05-26/010.png)

![image](/assets/2024-05-26/020.png)

## Register an app in the Micrsoft Entra ID directory

To access the data later without using a username and password, we need to register an app in the Entra ID directory of the company's Azure portal. The following screenshot shows how to do this:

![image](/assets/2024-05-26/030.png)

Within the app details, we register a client secret:

![image](/assets/2024-05-26/040.png)

 We also need the client ID. It can be found in the **Overview** administration pane of the registered app.

![image](/assets/2024-05-26/050.png)

There's no need to apply additional rights or other authorization attributes.

## Apply the registered app to the Dataverse environment

Before we can use the registered app, we need to apply it to the Power Apps environment:

1. In the [Power Apps admin center](https://admin.powerplatform.microsoft.com/), we select the environment we want to access.
2. We go to **Settings > User + permission > Application users**.
3. We select **New App User** to add the registered app we created earlier to the list of application users.

![image](/assets/2024-05-26/055.png)

After adding the app, we have to add a security role that fits our needs. This depends on the existing roles in the Power App environment. If you don't care about roles and just want to get it working, you can assign the *System Administrator* role to give the app full access to all the data in the Dataverse.

![image](/assets/2024-05-26/060.png)

## Use Views

In general, we have two options to access the data:

1. We can access the entity of the Dataverse directly, which includes all attributes and all rows.
2. We can use a *view*.

Using a view is the preferred way. We can build a view directly in the Power Apps dev environment. The following screenshot shows a view, as seen by an end user in the CRM portal. The view depends on the `salesorder` entity in the D365 CRM system. It shows several orders for the shipment team and a very limited number of attributes (columns). This will be the basis for how our Peakboard data source accesses the data.

![image](/assets/2024-05-26/070.png)

## Set up the data source

Back in Peakboard Designer, we can now create a Dynamics 365 data source, using the extension we installed earlier. We only need to provide four values for accessing the view:

* The URL to the Dynamics 365 or Dataverse system.
* The client ID.
* The client secret.
* The name of the view. The combo box lists all views in the system. Each entry starts with the entity name, followed by a pipe, followed by the actual view name. The entries are sorted alphabetically, by entity name.

![image](/assets/2024-05-26/080.png)


## Result and conclusion

The actual output shown in the screenshot is not the interesting part. The interesting part is how we set up the registered app in Azure, bound it to a Dataverse environment, and accessed the data by using the client ID and client secret.

Once again, note the advice that it's not secure to use a username and password. Using a registered app in Azure is the proper way to do things, and it's highly recommended following this advice, especially in production environments.

![image](/assets/2024-05-26/090.png)


