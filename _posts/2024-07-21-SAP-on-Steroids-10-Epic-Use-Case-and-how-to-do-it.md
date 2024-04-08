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

Monitoring outbound deliveries is a very common and an easy use case for Peakboard. It downloads all active deliveries and their items, and aggregates them by a shipping point, warehouse number, or storage location. This is a typical way of answering the question, "Where are we and where are we supposed to be?" For an example of this use case, see the [Area coordination in the warehouse](https://templates.peakboard.com/Warehouse-Management-Areas-Coordination-With-SAP/en) template.

There are a couple different ways to build the technical backend:

1. You can download the **LIKP** and **LIPS** tables directly by using the table select XQL command in Peakboard.
2. You can create a Z function module and have the SAP side handle the data join, aggregation, and selection. This is a more sophisticated option. See [How to build a perfect RFC function module to use in Peakboard](https://how-to-dismantle-a-peakboard-box.com/SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html) to learn how to do this.
3. You can create an SAP Query in the **SQ01** transaction to select the delivery information. See [How to have fun with SAP Queries](https://how-to-dismantle-a-peakboard-box.com/Easy-access-to-complex-SAP-data-or-how-to-have-fun-with-SAP-Queries.html) to learn how to do this.

## 2. Workplace capacity and status monitor 

The most common use case for an SAP production environment is to determine and display the capacity and utilization of one or more workplaces.
Unfortunately, there isn't a standard BAPI to determine workplace capacity. However, there's a function module for internal usage that can be used. So the easiest way to determine workplace capacity is to wrap the internal module in an RFC-enabled function module. See [How to determine workplace capacity](/Dismantle-SAP-Production-How-to-determine-workplace-capacity.html) to learn how to do this.

The utilization is very tricky to determine correctly. As an SAP user, you would use **COOIS** transactions to list all the production orders for a certain workplace in a certain time frame.
See [How to get the next work orders of a workplace](/Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html) to learn how to do this.
Once you have all the necessary operations and the overall capacity, it's easy to calculate the utilization.

## 3. Production order confirmation 

Confirming production orders doesn't necessarily mean finishing them. It's also important to give a regular update on the progress of the order---in terms of employee time and machine time usage, and also the current number of goods produced (normally, the number of goods, scrap, and yield).

SAP has a nice standard function module called **BAPI_PRODORDCONF_GET_TT_PROP** that does all sorts of confirmation activities. 
The actual confirmation can be done by a real person or automatically by the machine. See [Build a Production Order Confirmation Terminal with no code](/SAP-Production-Build-a-Production-Order-Confirmation-Terminal-with-no-code.html) to learn how to do this.

It might be a good idea to store the confirmation in a database or Peakboard Hub before submitting it to SAP. This makes the system more resilient against SAP outages or other IT problems, so that production doesn't come to halt if a confirmation can't be processed right away.

## 4. Transfer order monitor 

Transfer orders are objects in SAP's Logistic Execution (LE) or Warehouse Management (WM) module. The corresponding transaction is **LT03** for creation.
The data around transfer orders is often displayed at workstations for automatic material handling systems or automatic high-rack installations. We often see that the machine or sensor data is mashed up with the transfer orders. The same is true for the usage of barcode scanners. Both are possible---not just for displaying the order, but also for handling dynamic interactions by the user.

To display the order, we can use the standard function **BAPI_WHSE_TO_GET_LIST** to query the keys and then **BAPI_WHSE_TO_GET_DETAIL** to get the details. If it's too annoying to split it into so many calls (because of many orders being in the list overview), it makes sense to build a Z function and read the tables **LTBK** (headers) and **LTBP** (items) directly, and return a single table to Peakboard. This situation is quite similar to the outbound delivery use case listed under number 1.

## 5. Quality notes

Quality notes are generated many situations. For Peakboard apps, there are three common use cases:
1. Defect reporting 1: The produced good shows defects in the production process. The defects can't be handled during the process, or it will bring the process to a standstill. This can happen during production or during the quality inspection step.
2. Defect reporting 2: The material that is used for the production shows defects that were undiscovered up until now. Usually, all the material must be locked immediately to prevent other production units from using it, and additional steps must be taken.
3. Health, safety, and environmental incidents reporting: Incidents that affect health, safety, or the environment, potentially due to quality issues in processes or materials. This includes workplace accidents related to equipment or hazardous materials.

When building the Peakboard app, it's usually not necessary to do any ABAP development, because the standard BAPIs, **BAPI_QUALNOT_CREATE**, **BAPI_QUALNOT_SAVE** and **BAPI_QUALNOT_CHANGE**, are used to submit the notes to SAP. 

## 6. Packing a delivery 

In a typical outbound delivery process, the delivery is packed after collecting the goods. In SAP standard, the transactions **VL01** and **VL02N** are used. Depending on the use case, it might be necessary to submit additional information to SAP. For example, the size of the packaging material used.

One of the most common BAPIs used here is **BAPI_OUTB_DELIVERY_CHANGE**, together with **BAPI_HU_CREATE** and **BAPI_HU_PACK**. We also see a lot of custom function modules because usually this process is done in an individualized way.

In an even more sophisticated use case, we can add a camera to the Peakboard application. The camera can double-check the goods packed or systematically take and store a photo of the goods within the package, in order to document the contents and how it's packed.

## 7. Technical drawings and other documents

Peakboard is usually focused on giving a user a dashboard to see and interact with. So the main goal of Peakboard apps is to bring all the necessary information to the user. In most cases, this is structured data. However, there are a couple of use cases where documents are used---typically, technical drawings or documents containing additional instructions on certain processes.
 There are two situation seen very often: Assembly instructions for the user who are doing any kind of assembly or quality check instructions for user who carrying out quality checks.
The documents are handled in SAP through the **CV0XX** transaction (document info record). The document info record usually points to the original source of the document, but this depends on the underlying document mangement system (DMS). Most common is just an http endpoint that can be used directly in Peakboard similiar to downloading a document from Sharepoint. We already discussed this pattern in [this article](/Dismantle-Sharepoint-How-to-use-a-document-library-to-store-techical-drawings-and-download-them-to-Peakboard-dynamically.html). 

## 8. Loading gate monitor 
In the process of dispatching outbound deliveries' packages or palets are often put onto the trucks through different loading gates depending on the transport carrier. This can be a source of very annoying problems if done wrong. There is a simple version of loading gate monitors just showing the destination region, carrier or whatever helps the handling agent to cross check if his currently handled package fits to to the truck behind the loading gate (see screenshot of [this template](https://templates.peakboard.com/Overview-Truck-Loading/en)). The even more sophisticated version is to use additional hardware connected to the Peakboard app to actively cross check the process, e.g. a barcode scanner or RFID reader. The Peakboard app can then directly check and confirm with SAP the the outbound delivery is loaded onto the truck. 

## 9. Inventory
The Peakboard app is usually used on a mobile device and is replacing the printed inventory list. The most common way to do this is to use standardized BAPIs like **BAPI_INVENTORYCOUNT_SUBMIT** and **BAPI_INVENTORYCOUNT_POSTDIFF**. The whole process that is usually managed though the **MI0X** transactions can be completely or in parts done by the Peakboard application.

## 10. Collect machine data from OPC UA and PLCs
Machine data is collected and submitted to SAP (mostly to SAP MES or SAP PP, but not limited to that). On the Peakboard side there are typical machine interfaces like OPC UA or a native connction to the PLC (Siemens, Beckhof, Rockwell, etc...). Typical values are machine status (usage time, fault codes), cycle time and of course good and reject count. Depending on the data and usage it might make sense to submit it directly in real time or store it first in a local database of Peakboard Hub and submit it asynchronously. As this process is highly customziable it's usually done through indvidual Z function modules on the SAP side.


