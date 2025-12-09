---
layout: post
title: It's Christmas - time to look back
date: 2023-03-01 01:00:00 +0200
tags: basics
image: /assets/2025-12-21/title.png
image_header: /assets/2025-12-21/title_landscape.png
---
Dear readers, wherever you are in the world: 2025 is almost over! So, let's take a step back and reflect on all the articles that we published this year. So many topics and so many new ideas. This has been the best year for Peakboard (at least so far)!

## Hub Flows enter the scene

If this blog had a mascot for 2025, it would probably be a little Hub Flow diagram scribbled on a napkin. The powerful Hub Flows feature came out earlier this year. And on this blog, we spent an entire series turning background automation from "cool idea" into actual production-ready patterns:
* We started with the basics of Hub Flows in [Hub Flows I](https://how-to-dismantle-a-peakboard-box.com/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html).
* Then, we immediately dived into the SAP world in [Hub Flows II](https://how-to-dismantle-a-peakboard-box.com/Hub-Flows-II-Cache-Me-If-You-Can-Data-Distribution-for-SAP-Capacity-Data.html).
* From there, we showed how one PLC can feed an entire factory, in [Hub Flows III](https://how-to-dismantle-a-peakboard-box.com/Hub-Flows-III-One-PLC-to-Feed-Them-All-Using-Peakboard-Flows-to-Share-and-distribute-Siemens-S7-Values.html).
* Next, we worked on asynchronous SAP confirmation queues in [Hub Flows IV](https://how-to-dismantle-a-peakboard-box.com/Hub-Flows-IV-Peakboard-Flows-in-Production-Asynchronous-SAP-Confirmation-Processing.html).
* And finally, we tackled high-volume telemetry with pre-aggregation and archiving in [Hub Flows V](https://how-to-dismantle-a-peakboard-box.com/Hub-Flows-V-Condense%2C-Archive-Optimize-Use-Hub-Flows-to-Pre-Aggregate-and-Archive-High-Volume-Transaction-Data.html).

This series turned Hub Flows from a "cool feature" into a toolbox of robust, reusable patterns for real factories.

## Peakboard Hub API everywhere

Another big topic we covered this year is the Peakboard Hub API. We learned what's possible when you treat your Hub like a programmable control plane, rather than just a nice admin dashboard:
* In [Cracking the code - Part II](https://how-to-dismantle-a-peakboard-box.com/Cracking-the-code-Part-II-Calling-functions-remotely.html), we used the Hub API to send alarms to Peakboard Boxes from outside the local network.
* Then, we went full Python in [Sssslithering Through APIs](https://how-to-dismantle-a-peakboard-box.com/Sssslithering-Through-APIs-Python-Unleashed-for-Peakboard-Hub.html), where we turned the Hub into a playground for scripts, tables, and ad-hoc SQL over lists.
* In [Building an Azure Logic App to Access Peakboard Boxes](https://how-to-dismantle-a-peakboard-box.com/From-Cloud-to-Factory-Building-an-Azure-Logic-App-to-Access-Peakboard-Boxes-via-the-Peakboard-Hub.html), we bridged Microsoft's cloud workflows with shop-floor Peakboard Boxes.
* And in [Cracking the code - Part III](https://how-to-dismantle-a-peakboard-box.com/Cracking-the-code-Part-III-Reading-and-writing-lists-with-Peakboard-Hub-API.html), we learned how to treat Hub lists like a lightweight database.
* Finally, in [Revealing Peakboard Hub List Data in your Power BI Dashboards](https://how-to-dismantle-a-peakboard-box.com/Peak-a-Boo-Revealing-Peakboard-Hub-List-Data-in-your-Power-BI-Dashboards.html), we piped Hub list data into BI dashboards---turning the Hub API into the central backbone between cloud services, factory apps, and analytics.

## SAP: from inventory to BTP

Of course, no year is complete without a couple of SAP articles! This year was a little different, however. Instead of discussing individual SAP connectors, we instead focused on how to make SAP behave nicely in a hybrid, Hub-centric architecture:
* In [How to handle SAP Inventory](https://how-to-dismantle-a-peakboard-box.com/Because-Nobody-Likes-MI05-How-to-handle-SAP-Inventory.html), we replaced the paper-based physical inventory process with a MI05 workflow that uses a tablet app to talk to BAPIs directly.
* In [SAP Integration Suite and Peakboard Hub](https://how-to-dismantle-a-peakboard-box.com/Breaking-the-Ice-How-SAP-Integration-Suite-and-Peakboard-Hub-Became-Best-Friends.html), we showed how SAP BTP and Integration Suite fit in a future where everything is cloud-based, and Peakboard Hub is just another peer in the landscape.
* In [SAP Hana Meets Peakboard](https://how-to-dismantle-a-peakboard-box.com/SAP-Hana-Meets-Peakboard-Mastering-ODBC-Integration-Step-by-Step.html) we showed how to talk to SAP Hana directly through ODBC. 
* In [Mission RFC Possible](https://how-to-dismantle-a-peakboard-box.com/Mission-RFC-Possible-Executing-Custom-Function-Modules-with-Integration-Flows.html), we explained how to wrap classic RFCs in modern HTTP endpoints, with Integration Suite.

## Hardware, sensors, and an army of printers

On the hardware side, 2025 was the year of "things that blink, beep, or spit out labels":
* In [Getting started with label printing](https://how-to-dismantle-a-peakboard-box.com/The-Art-of-Printing-Getting-started-with-label-printing-on-Seiko-SLP720RT.html), we explained the basics of label printing with Peakboard.
* In [Mastering Bixolon SRP-Q300 Series](https://how-to-dismantle-a-peakboard-box.com/The-Art-of-Printing-Mastering-Bixolon-SRP-Q300-Series-receipt-printer-with-enhanced-ESC-POS-commands-and-tables.html), we took a deep dive into ESC/POS.
* In [How ZPL Turns Your Bixolon XD5-40 printer into a Label Wizard](https://how-to-dismantle-a-peakboard-box.com/Print-Charming-How-ZPL-Turns-Your-Bixolon-XD5-40-into-a-Label-Wizard.html), we went all-in on label printing.
* In [Pick-by-Light System with Captron and Peakboard](https://how-to-dismantle-a-peakboard-box.com/Lights-On-A-Step-by-Step-Guide-to-Designing-a-Pick-by-Light-System-with-Captron-and-Peakboard.html), we explored pick-by-light systems, as part of intralogistics.
* In [Building a Smart Dashboard for Tracking Temperature and Humidity](https://how-to-dismantle-a-peakboard-box.com/Peakboard-Meets-Shelly-Building-a-Smart-Dashboard-for-Tracking-Temperature-and-Humidity.html), we revisited Shelly devices with a more grown-up, production-ready dashboard.
* In [Transform Your Peakboard Box into an MQTT Server](https://how-to-dismantle-a-peakboard-box.com/DIY-Guide-Transform-Your-Peakboard-Box-into-an-MQTT-Server.html), we showed how to turn a Peakboard Box into a hub for sensors and gadgets.

## Office 365, extensions, and a 4.1-sized jump

The last major topic we covered this year was "everything around the Box." That is, the parts that make Peakboard feel like a complete platform, rather than a single tool.

We released a bunch of articles about Office 365:
* In [Getting started with the new Office 365 Data Sources](https://how-to-dismantle-a-peakboard-box.com/Getting-started-with-the-new-Office-365-Data-Sources.html), we moved collaboration data closer to the shop floor
* In [Reading and writing SharePoint lists with Graph extension](https://how-to-dismantle-a-peakboard-box.com/Reading-and-writing-Sharepoint-lists-with-Graph-extension.html), we extended the Graph approach.
* In [SharePoint Lists in Beast Mode](https://how-to-dismantle-a-peakboard-box.com/SharePoint-Lists-in-Beast-Mode-Powered-by-Peakboard.html), we pushed list automation further.
* In [Elevate Your Workflow - Building a Next-Gen Continuous Improvement Board with Office 365 ToDo](https://how-to-dismantle-a-peakboard-box.com/Elevate-Your-Workflow-Building-a-Next-Gen-Continuous-Improvement-Board-with-Office-365-ToDo.html), we took a more process-driven perspective followed.

We also released two articles aimed at developers:
* In [Plug-in, Baby - The Basics](https://how-to-dismantle-a-peakboard-box.com/Plug-in-Baby-The-ultimate-guide-to-build-your-own-Peakboard-extensions-The-Basics.html), we gave an introduction on how to create custom extensions for Peakboard.
* In [Writing and Querying Time Series Data with Peakboard](https://how-to-dismantle-a-peakboard-box.com/Influx-and-Chill-Writing-and-Querying-Time-Series-Data-with-Peakboard.html), we shared a clean, reuseable pattern for time-series workloads.

All of this culminated in the [Peakboard 4.1 release overview](https://how-to-dismantle-a-peakboard-box.com/Peakboard-4.1-Is-Here-And-Its-a-Game-Changer.html), where we went over all the new features in Peakboard 4.1. This included BACnet, AI helpers, the new drawing tool, improved debugging, and smarter Hub internals.

Altogether, this topic turned 2025 into the year where Peakboard grew sideways just as much as it grew up---broader integrations, richer platform features, and a much more powerful toolbox for the people who build with it.

## See you next year!

Finally, here's a poem to end us off:

*Boxes sleep, screens go dim.*  
*Dashboards dream of a festive hymn.*  
*Cables coiled and Flows tucked tight---*  
*Have a very merry Christmas, and may next year be just as bright!*

To all our readers out there, enjoy the holidays and see you next year!

Love, Michelle

![image](/assets/2025-12-21/title.png)


