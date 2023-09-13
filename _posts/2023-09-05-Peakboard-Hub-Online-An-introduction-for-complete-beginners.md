---
layout: post
title: Peakboard Hub Online - An introduction for complete beginners
date: 2023-09-05 12:00:00 +0200
tags: peakboardhub
image: /assets/2023-09-05/title.png
read_more_links:
  - name: Try Peakboard Hub Online
    url: https://hub.peakboard.com
  - name: Peakboard Hub product information
    url: https://peakboard.com/en/product/peakboard-hub/
  - name: Peakboard Hub on-premises documentation (some parts may not apply to PBHO)
    url: https://help.peakboard.com/hub/en-hub_connectpbdesigner.html
---

Peakboard recently launched [Peakboard Hub Online](https://hub.peakboard.com), a SaaS version of Peakboard Hub. Peakboard Hub used to be only available as an on-premises product. But with the launch of Peakboard Hub Online, you can now choose which version you want to use.

In this article, we will learn how to get started with Peakboard Hub Online. Then, we will quickly go over the features of Peakboard Hub: Peakboard Boxes, connections, lists, alerts, and organizations and user groups. Note that this is only a general overview. It will not go in-depth into any particular feature.


{% comment %}
## On-premises and online differences

What are the differences between the on-premises and online versions of Peakboard Hub?

With the on-premises version, you set up and manage the software yourself, on your own hardware. With the online version, Peakboard handles all the infrastructure and management, and you access the software online.

Another difference is that Peakboard Hub Online is included in a Peakboard Box subscription, while Peakboard Hub requires a separate subscription.

Take a look at this chart from [Peakboard's website](https://peakboard.com/en/product/peakboard-hub/) for more information:

![image](/assets/2023-09-05/010.png)
{% endcomment %}


## Getting started

1. Go to [hub.peakboard.com](https://hub.peakboard.com/) and log in.

2. Get your user group key.
![image](/assets/2023-09-05/020.png)

3. Open Peakboard Designer. Click the Peakboard Hub button on the menu bar. Then, set the type to *Online*. Enter your user group key. Finally, click *Connect*.
![image](/assets/2023-09-05/021.png)

4. Click *Synchronize*.

Now, Peakboard Designer is connected to Peakboard Hub Online. This gives Peakboard Designer access to the Peakboard Boxes, connections, and lists associated with the Hub.

Now, let's take a look at the features of Peakboard Hub Online.


## Peakboard Boxes

You can connect **Peakboard Boxes** to Peakboard Hub Online. This lets you manage and monitor your Peakboard Boxes using Peakboard Hub Online.

With a connected Peakboard Box, you can do things like change its network settings, view its current dashboard, monitor its resource usage, and much more.

![image](/assets/2023-09-05/024.png)


## Connections

In Peakboard Hub Online, you can use a **connection** to store the information needed to connect to a data system. A data system is something like an SAP system, an Oracle Database server, or a web resource. The information stored includes things like an IP address, a port, and importantly, credentials.

Once you have a connection, you can use it to automatically fill out information about the data system, when adding a data source in Peakboard Designer.

Connections make it convenient to create multiple data sources that rely on the same data system. With connections, you don't have to manually enter the same information every time. But more importantly, connections allow IT departments to grant access to protected data systems without sharing the actual credentials.

Here, I use a connection to fill out information for an SAP data source, in Peakboard Designer:

![image](/assets/2023-09-05/022.png)


## Lists

In Peakboard Hub Online, you can use **lists** to store tabular data and share it to Peakboard Boxes. The Boxes can also send changes back to the List. You can also make manual changes to a list in Peakboard Hub Online. 

A Peakboard Box can use a list by adding a Peakboard Hub List data source.

A Peakboard Box can modify a list by adding a script that modifies the list.

Here's what a list looks like in Peakboard Hub Online:

![image](/assets/2023-09-05/025.png)


## Alerts

Peakboard Boxes can send **alerts** to Peakboard Hub Online. An alert is a notification that contains a severity level (`info`, `warning`, or `problem`) and a message.

Peakboard Boxes can send alerts with a script. In Peakboard Hub Online, you can view all your alerts. You can also choose to receive new alerts by email.

![image](/assets/2023-09-05/023.png)


## Organizations and user groups

In Peakboard Hub Online, users belong to **organizations** and **user groups**. An organization represents a company, and a user group represents a team within that company. So, an organization contains one or more user groups.

A user belongs to one or more organizations. And for each organization, a user belongs to a single user group within that organization.

For example, let's say John Doe works for Acme Corporation, on the American team. Then, he would belong to the "Acme Corporation" organization, as well as the "American Team" user group.

The purpose of user groups is to provide access control for Peakboard Boxes, connections, and lists.

![image](/assets/2023-09-05/030.png)
