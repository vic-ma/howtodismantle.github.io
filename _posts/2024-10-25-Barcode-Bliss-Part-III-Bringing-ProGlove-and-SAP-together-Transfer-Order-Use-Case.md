---
layout: post
title: Barcode Bliss - Part III - Bringing ProGlove and SAP together - Transfer Order Use Case
date: 2023-03-01 12:00:00 +0200
tags: hardware opcuamqtt
image: /assets/2024-10-25/title.png
read_more_links:
  - name: Barcode Bliss - Part I - Integrating ProGlove Scanners with Peakboard
    url: /Barcode-Bliss-Part-I-Integrating-ProGlove-Scanners-with-Peakboard.html
  - name: Barcode Bliss - Part II - Sending Feedback to ProGlove Scanners
    url: /Barcode-Bliss-Part-II-Sending-Feedback-to-ProGlove-Scanners.html
  - name: ProGlove Documentation
    url: https://docs.proglove.com/en/connect-gateway-to-your-network-using-mqtt-integration.html
downloads:
  - name: SAPProGloveTransferOrder.pbmx
    url: /assets/2024-10-25/SAPProGloveTransferOrder.pbmx
---
In [part one](/Barcode-Bliss-Part-I-Integrating-ProGlove-Scanners-with-Peakboard.html) and [part two](/Barcode-Bliss-Part-II-Sending-Feedback-to-ProGlove-Scanners.html) of our ProGlove mini-series, we discussed the basics of integrating ProGlove scanners into Peakboard applications. We used both the USB and MQTT modes to get the scan event and related metadata. We also discussed options to send feedback to the scanner operator, such as a light with various colors or even a small display that's mounted to the scanner.

In this article, we will learn how to put together what we've already learned in order to build a complete use case. The use case is to handle a typical transfer order from SAP using only Peakboard and ProGlove.

## Process overview

A transfer order in SAP is a standard object to indicate that some goods must be collected from the warehouse and put to different location. This can useful to feed production needs or to fullfill a customer order. Usually these transfer orders are handled with SAP transaction LT01, LT02 etc... in the SAP system.

Here's the process how to handle a transfer order in our ProGlove / Peakboard scenario:

1. The worker is scanning a QR code to indicate the start of a new transfer order process, so the system is looking up the next untouched transfer order from SAP
2. The line items of the TO are displayed on the screen and the warehouse bin of the first line is shown on the scanner display
3. The worker is finding the warehouse bin and scans the code of the warehouse bin to indicate that he has arrived at the correct warehouse bin. The scan makes sure that the worker is not grabbing goods from the wrong bin. It's a double check.
4. After having scanned the correct bin that quantity of goods is dayiplayed on ProGlove, so the worker can pick the goods.
5. The worker confirms the picking with a double click on the ProGlove button
6. The double clicks confirms the picking and the next line items is activated, so the next warehouse bin is displayed for the worker on the ProGlove display. Then go to step 3
7. Repeat the steps 3-6 until all line items are picked by the worker. 

The overall process progress is displayed on the Peakboard screen. So in case something goes wrong or the worker loses orientation he can always walk back to the display and check the current situation. It's not necessary for the worker to have the screen in sight all the time because all necessary information are seeable on the ProGlove screen.

The screenshot shows an order where the first two line items are already picked and confirmed. The third one is currently in the process of being picked. The rest of the lines are still in waiting state.

![image](/assets/2024-10-25/010.png)

## The data sources

Let's have a look at the necessary data connections. This screenshot shows a simple SAP connection. To get the line items for a tranfer order we just use a query for the table LTAP. In a more real life scenario it might be usful to put the logic into an RFC function module to determine the next active transfer order.

![image](/assets/2024-10-25/020.png)

For the ProGlove connectivity we use a typical MQTT connection and some data paths to extract the useful information from the JSON string that is sent from ProGlove. The information we need is the scanned code, the serial number (to send back information to the display of the same scanner) and the information if the user has double clicked.

![image](/assets/2024-10-25/030.png)

The last thing we need is a table-like variable to store the order items. We need a separate list instead of the original SAP data source data because we need to add the 'Status' column to indicate if the item is already open (O), active (A) or done (D).

![image](/assets/2024-10-25/040.png)

## The UI 

The UI is pretty simple. We only use a QR code with a fixed value ("$Order_Start$") and a styled list. The details of the color and icon change according to the items state is not explained here. We can look that up in the pbmx that can be downloaded [here](/assets/2024-10-25/SAPProGloveTransferOrder.pbmx).

![image](/assets/2024-10-25/050.png)

## The application logic

The actual application logic (aka the magic behind) is happening in the refreshed event of the MQTT data source. This process is triggered every time the worker is doing something with the ProGove scanner. We distinguish netween three cases.

1. The 'Order Started' barcode is scanned
2. The warehouse bin is scanned 
3. The button on the scanner is double clicked

It doesn't make sense to go through any single command in detail, but here are the three cases on high level basis:

### Auxiliary functions

We use three different functions for the actual sending of the MQTT message to the display and the lights of the scanner. The only purpose here is encapsulate the complete JSON and make sure the main function is easier to understand. How the JSON works is well explained in the second parter of the article series.

![image](/assets/2024-10-25/055.png)

### 1. The order is started

In case we receive the scan of the string "$order_started$" the user wants to start a new order. So we send feedback to the ProGlove display that the order has started. Then we query the data from SAP and fill our variable list with the items (just loop over the original source). Then we send positiv feedback (green light), set the first column to A for active and send the bin information to the display.

![image](/assets/2024-10-25/060.png)

### 2. The bin is scanned

If the bin is scanned we check, if the bin is correct by comparing it to the bin of the active line item. This action is necessary to check if the worker is about to pick from the correct bin. If this is the case, we send him the quantity to pick. If not, we send an error message.

![image](/assets/2024-10-25/070.png)

### 3. Confirming the pick

When the user has finished the pick he is supposed to confirm this by double clicking on the ProGlove button. The current line item is set to D for Done and we send the worker the next bin in the list.

![image](/assets/2024-10-25/080.png)

## result and conclusion

We learned in this article how easy it is to build a Peakboard application to feed a ProGlove scanner with all information of an SAP transfer order and build the complete process to handle the whole picking process. All other details of the Peakboard app are not important to understand the logic and the architecture of the app. 

The most impressive point to show here is how the power of Peakboard and the power of ProGlove can be combined to build a perfect and secure process.

The building of the app and the actual application can be also checked in this video:

(The video is not ready yet. This link will be replaced as soon as it's available.)

{% include youtube.html id="XXX" %}



