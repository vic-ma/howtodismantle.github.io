---
layout: post
title: How to manage and organise team access for Peakboard boxes in large organisations
date: 2023-01-28 12:00:00 +0200
tags: administration bestpractice
image: /assets/2023-01-28/title.png
read_more_links:
  - name: Peakboard Hub Official Documentation
    url: https://help.peakboard.com/hub/en-hub_connectpbdesigner.html
  - name: Cool video on YouTube
    url: https://www.youtube.com/watch?v=E-aYuWoDiEM&ab_channel=Peakboard
---
How do you manage and organise Peakboard boxes throughout a large organsation, across different teams of content creators, technical admins, and physical locations? Of course, you don't want to send out box credentials to all stakeholders. This article is here to help you understand some basic concepts in using Peakboard boxes in a large organisation.

Let's assume we have an IT department in a large organsation.
And beside the IT department, there are two teams for creating and maintaining the content of the Peakboard boxes—one in France and one in China.
Here are two possible solutions: In the first option, boxes are *not* in the company's Active Directory (AD) domain (this is the most common case). In the second option, boxes *are* in the company's AD domain.

## Without Active Directory

Without Active Directory, the IT department installs a single Peakboard Hub instance. Every box in the organisation is accessible through the Hub, and can receive updates and health checks.

Peakboard Hub lets you create user groups, in order to bundle boxes together for access control. In our example, the French boxes are a part of the French group, and the Chinese boxes are a part of the Chinese group.

Every group has a group key. The French and the Chinese content creators can use their respective group key to upload content to their group's boxes. They don't have any individual box's credentials, and they don't have access to the other group's key. So the French content creators can only maintain the French boxes, and the Chinese content creators can only maintain the Chinese boxes, and the IT department still has administrative power over everything.

Here's what a group looks like in the Hub portal. The group contains a single box, and the group key is visible in the top-right.

![image](/assets/2023-01-28/010.png)

This is how the content creator can connect their Designer instance to the Hub, with their group key and Hub URL:

![image](/assets/2023-01-28/020.png)

Then, they will have access to the boxes within their group:

![image](/assets/2023-01-28/030.png)

## With Active Directory

With an Active Directory (AD), if all boxes are part of the AD domain, there can be AD groups called `PeakboardAdmins` and `PeakboardUsers`. Every AD member who's a part of one of these groups has access to the boxes. To prevent the Chinese from accessing French boxes, the admins can set up special Organisational Units (OUs) and login Group Policy Objects (GPOs) in AD. This option can only be used with on-prem ADs or hybrid ADs. Pure Azure ADs are not supported at the moment.

## Important note

As of the first half of 2023, the second option (with AD) is not yet available, but it is planned to be released in the second half of 2023.

