---
layout: post
title: SAP on Steroids - 10 Epic Use Cases and how to build them
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-07-21/title.png
read_more_links:
  - name: Dismantle SAP Production - How to determine workplace capacity
    url: /Dismantle-SAP-Production-How-to-determine-workplace-capacity.html
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction
    url: /Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html
---
Besides the SQL Server, SAP is the most used data source in Peakboard applications. We already have many [SAP articles](/category/sap) in this blog.
In this article, we'll list the 10 most common use cases for the SAP data source, and give a quick rundown on how to build them. Of course, the examples in this list may not fit your particular need. But in most cases, they are a good starting point or inspiration for building personalized applications.

## 1. Outbound delivery monitor

Monitoring outbound deliveries is very common and an easy use case with Peakboard. We're just downloading all active deliveries and their items and aggregate on shipping point, wharehouse number or storage locations. This is a typical way of handling the question "where are we and where are we supposed to be". On the [templates site](https://templates.peakboard.com/Warehouse-Management-Areas-Coordination-With-SAP/en) there is a nice template available that shows such a case.
There are different ways for the technical backend. One option is to download the table LIKP and LIPS directly by using the table select XQL command within Peakboard. The more sophisticated option is to create a Z function module and let the data join, aggregation and selection already take place on the sap side. In the article [How to build a perfect RFC function module to use in Peakboard](https://how-to-dismantle-a-peakboard-box.com/SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html) we already discussed exactly this case and built a function module named Z_PB_DELIVERY_MONITOR.
The third option is to create an SAP Query in transaction SQ01 to select the delivery information. Here we can also refer to [Easy access to complex SAP data - How to have fun with SAP Queries](https://how-to-dismantle-a-peakboard-box.com/Easy-access-to-complex-SAP-data-or-how-to-have-fun-with-SAP-Queries.html) to find out more.

## 2. Workplace capacity and status monitor 

The most common use case in any SAP production environment is to dermine and show the capacity and the utilization of one or multiple workplaces.
Unfortunately there's no standard BAPI to determine the workplace capacity. However there's a function module for internal usage that ca be used. So the easiest way to this is to wrap the internal module in an RFC enabled function module. There's a separate article on how to do this: [Dismantle SAP Production - How to determine workplace capacity](/Dismantle-SAP-Production-How-to-determine-workplace-capacity.html).
Beside the capacity the actual utilisation is very tricky to determine correctly. As an SAP user you would normally use transaction COOIS transactions to list all production order for a certain workplace in a certain time frame.
We once again recommend this article to find out more about how to do that practically: [Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction in Peakboard](/Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html) 
Once you have all necessary operations and the overall capacity it's easy to caclulate the utilization.

## 3. Production order confirmation 

Confirming production orders does no necessarily mean to finish them. It's also important to give a regular update on the progress of the order in terms of usage of people's time and machine time, but also the current number of goods produced (normally the number of good, scrape and yield).
SAP offers a very nice standard function module called BAPI_PRODORDCONF_GET_TT_PROP to do all kinds of confirmation activities. 
Who is doing the actual confirmation in the process can be both, a real person or the machine automatically. A sample on how to do this can be found in the article [Dismantle SAP Production - Build a Production Order Confirmation Terminal with no code](/SAP-Production-Build-a-Production-Order-Confirmation-Terminal-with-no-code.html).
It might be a consideration to store the confirmation in a database or Peakboard Hub first before submitting to SAP. This makes the system more resilent against SAP outages or other IT problems and doesn't bring the production to halt in case a confirmation can't be processed right away.

## 4. Transfer order monitor 

Transfer orders are objects of SAP's Logistic Execution (LE) or Warehouse Management (WM) module. The corresponding transaction is LT03 for creation.
The data around transfer order is often displayed at workstations of any kind of automatic material handling systems or automatic high-rack installations. We often see that that machine or sensor data is mashed up with the transfer orders. The same is true for the usage of barcode scanners. Both is possible, just displaying the order but also dynamic interactions by the user.
For displaying we can use the standard function BAPI_WHSE_TO_GET_LIST to query the keys and then BAPI_WHSE_TO_GET_DETAILto get the details. If this is too annoying to split in so many calls (because of many orders in the list overview), it makes sense to build a Z function and read the tables LTBK (headers) and LTBP (items) directly and return one single table to Peakboard. The technical situation is quite similiar to the outbound delivery use case listed under No 1.

## 5. Quality notes

Quality notes are generated in lots of different situation. In the context of a Peakboard app there are mainly three use cases that are often seen:
1. Defect Reporting 1: Directly in the production process the produced good is showing defects that can't be handled during the process or bring the process to a stand still. This can happen during production or during the quality inspection step.
2. Defect Reporting 2: In this case the material that is used for the production shows so far undiscovered defects. Usually all the material must be locked immediately to prevent other production units to use it plus a couple of other steps.
3. Health, Safety, and Environmental Incidents:  Incidents that affect health, safety, or the environment, potentially due to quality issues in processes or materials, are recorded. This includes workplace accidents related to equipment or hazardous materials.
When building the Peakboard app it's usually not necessary to do any ABAP development as the standard BAPIs BAPI_QUALNOT_CREATE, BAPI_QUALNOT_SAVE and BAPI_QUALNOT_CHANGE are used to submit the notes to SAP. 

## 6. Packing a delivery 

In a typical outbound delivery process the delivery is packed after collecting the goods. In SAP standard the transactions VL01 / VL02N are used. Depending on the use case it might be necessary to submit additional information to SAP, e.g. the size of the used packaging material. One of the most common used BAPIs here is BAPI_OUTB_DELIVERY_CHANGE together with BAPI_HU_CREATE and BAPI_HU_PACK. We also see a lot of custom function modules because usually this process is often done very indivdually.
In an even more sophisticated use case we can add a camera to the Peakboard application and let the camera help to either double check the goods packed or systematically take and store a foto of the goods within the package to be able to document the content and how it's packed.

## 7. Technical drawings and other documents

Peakboard is usually very user and user interface centric. So the main task is to bring all necessary information to the user, in most cases this is structured data. However there are a couple of use cases where documents are used, typically all kinds of technical drawings or documents containing additional instructions on certain processes. There are two situation seen very often: Assembly instructions for the user who are doing any kind of assembly or quality check instructions for user who carrying out quality checks.
The documents are handled in SAP through the CV0XX transaction (document info record). The document info record usually points to the original source of the document, but this depends on the underlying document mangement system (DMS). Most common is just an http endpoint that can be used directly in Peakboard similiar to downloading a document from Sharepoint. We already discussed this pattern in [this article](/Dismantle-Sharepoint-How-to-use-a-document-library-to-store-techical-drawings-and-download-them-to-Peakboard-dynamically.html). 

## 8. Loading gate monitor 
In the process of dispatching outbound deliveries' packages or palets are often put onto the trucks through different loading gates depending on the transport carrier. This can be a source of very annoying problems if done wrong. There is a simple version of loading gate monitors just showing the destination region, carrier or whatever helps the handling agent to cross check if his currently handled package fits to to the truck behind the loading gate (see screenshot of [this template](https://templates.peakboard.com/Overview-Truck-Loading/en)). The even more sophisticated version is to use additional hardware connected to the Peakboard app to actively cross check the process, e.g. a barcode scanner or RFID reader. The Peakboard app can then directly check and confirm with SAP the the outbound delivery is loaded onto the truck. 

## 9. Inventory
The Peakboard app is usually used on a mobile device and is replacing the printed inventory list. The most common way to do this is to use standardized BAPIs like BAPI_INVENTORYCOUNT_SUBMIT and BAPI_INVENTORYCOUNT_POSTDIFF. The whole process that is usually managed though the MI0X transactions can be completely or in parts done by the Peakboard application.

## 10. Collect machine data from OPC UA and PLCs
Machine data is collected and submitted to SAP (mostly to SAP MES or SAP PP, but not limited to that). On the Peakboard side there are typical machine interfaces like OPC UA or a native connction to the PLC (Siemens, Beckhof, Rockwell, etc...). Typical values are machine status (usage time, fault codes), cycle time and of course good and reject count. Depending on the data and usage it might make sense to submit it directly in real time or store it first in a local database of Peakboard Hub and submit it asynchronously. As this process is highly customziable it's usually done through indvidual Z function modules on the SAP side.


