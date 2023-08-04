---
layout: post
title: Farm like a boss - monitoring soil humidity with the new Peakboard Edge 2 box
date: 2023-08-21 12:00:00 +0200
tags: hardware
image: /assets/2022-08-21/title.png
read_more_links:
  - name: How to build a perfect RFC function module to be used in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
downloads:
  - name: EdgePlantMonitoring.pbmx
    url: /assets/2023-08-21/EdgePlantMonitoring.pbmx
---
Peakboard supports a lot of different object types that you can access in SAP. Besides RFC function modules, queries, tables, and MDX commands, it is also possible to execute and process ABAP reports. In order to do so, it's necessary to install a small, generic function module in the SAP system. This article explains how to do this.

First, log in to SAP with a user who has development rights and who can install ABAP code. During the process, you will need to provide a development class, and eventually, a transport request to put your new objects in. If you don't know how to do this, please ask a coworker or Google.

Feel free to adjust the name of the function module or the name of the DDIC structure as needed (e.g. according to a company's namespace).

## First steps with the sensor

![image](/assets/2022-08-21/010.png)

