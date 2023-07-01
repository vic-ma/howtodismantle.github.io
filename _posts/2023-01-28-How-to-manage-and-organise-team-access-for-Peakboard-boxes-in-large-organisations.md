---
layout: post
title: How to manage and organise team access for Peakboard boxes in large organisationsa
date: 2023-01-28 12:00:00 +0200
tags: administration bestpractice
image: /assets/2023-01-28/title.png
read_more_links:
  - name: Peakboard Hub Official Documentation
    url: https://help.peakboard.com/hub/en-hub_connectpbdesigner.html
  - name: Cool video on YouTube
    url: https://www.youtube.com/watch?v=E-aYuWoDiEM&ab_channel=Peakboard
---
Depending on the organisation it might be an important question how to manage and organise Peakboard boxes throughout a large organsation, across different teams of content creators, technical admins and physical locations. Of course you don't want to send out box credentials. This article is here to help to understand some basic concepts to address these challenges.

Let's assume we have an IT department for the infrastructure of the organsation organisation.
And beside the IT, there are two teams for creating and maintaining the content for the Peakboard boxes (e.g. one in France and one in China).
Here are two possible solutions: The first option is done WITHOUT having the boxes in the company's Active Directory (AD) domain (thats the most common case). The second option is done WITH the boxes as part of the AD domain.

## Using Peakboard Hub without Active Directory

The IT department installs a single Peakboard Hub instance. All boxes througout the whole organisation are accessible through the Hub for Updates and health check. There are so called user groups to bundle boxes, the Chinese boxes are organized in the Chinese group, and the French boxes are in the French group. Every group has a group key. Now the Chinese and the French content creators use only the group key to access the boxes for uploading content. They don't have the box credentials, but only the group key. So the French can only maintain French boxes, The Chinese can only maintain the Chinese boxes, but the IT department still remains in administrative power of everything.

Here's how a group looks like in Hub portal with one single box and the key.

![image](/assets/2023-01-28/010.png)

Then the content creator is connecting their Designer instance to the hub with their group key and hub URL:

![image](/assets/2023-01-28/020.png)

And then gets access to the boxes within their group:

![image](/assets/2023-01-28/030.png)

## Organising box access in Active Directory

If all boxes are part of the AD domain, there can be AD groups called PeakboardAdmins and PeakboardUsers. Every AD member who's part of one of these groups has access to the boxes for maintaining content. To prevent the Chinese from accessing French boxes, the admins can set up special OUs (Organisation Units) and Login GPOs (Group Policy Objects) in AD. This option can only be used with on-prem ADs or hybrid ADs. Pure Azure ADs are not supported at the moment.

## Important note

Whenever you read this article please note, that as of the first half year of 2023 the second option (with AD) is not yet available, but is about to be released in he second half year of 2023.












