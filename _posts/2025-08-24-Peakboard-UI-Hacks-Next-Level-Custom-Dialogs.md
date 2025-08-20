---
layout: post
title: Peakboard UI Hacks - Next-Level Custom Dialogs
date: 2023-03-01 05:00:00 +0300
tags: ui
image: /assets/2025-08-24/title.png
image_header: /assets/2025-08-24/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Influx Docker Image
    url: https://hub.docker.com/_/influxdb
downloads:
  - name: CustomDialogs.pbmx
    url: /assets/2025-08-24/CustomDialogs.pbmx
---
All Peakboard applications are based on one or more screens. These can be compared to windows in tradional Windows based applications. However sometimes it might be necessary to have some kind of modal dialogs to ask the user for addtional confirmation or to "force" him to provide a certain value to move on in the process.
In this eeek's article we will discuss the best practice about how to build custom dialogs.

## Setting up Influx

![image](/assets/2025-08-16/010.png)
