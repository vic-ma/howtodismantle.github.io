---
layout: post
title: Mission RFC Possible – Executing Custom Function Modules with Integration Flows
date: 2023-03-01 03:00:00 +0200
tags: sap sapbtp
image: /assets/2025-06-05/title.png
image_landscape: /assets/2025-06-05/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Breaking the Ice - How SAP Integration Suite and Peakboard Hub Became Best Friends
    url: /Breaking-the-Ice-How-SAP-Integration-Suite-and-Peakboard-Hub-Became-Best-Friends.html
  - name: Download CLoud Connector
    url: https://tools.hana.ondemand.com/#cloud
downloads:
  - name: Iflow Z_PB_DELIVERY_MONITOR.zip
    url: /assets/2025-06-05/Z_PB_DELIVERY_MONITOR.zip
  - name: SAPDeliveries.pbmx
    url: /assets/2025-06-05/SAPDeliveries.pbmx
---
A couple of weeks ago, we took introduced SAP BTP and the SAP Integration Suite. We discussed how to [connect an integration flow to Peakboard Hub](/Breaking-the-Ice-How-SAP-Integration-Suite-and-Peakboard-Hub-Became-Best-Friends.html).
In this article we will discuss how to build an Integration Flow that exposes a JSON endpoint to be called by an Peakboard application. This new Integration Flow internally calls an RFC function module in SAP. The SAP system is connected to BTP via a cloud connector.
This is a typical scenario for customer who still have on prem SAP systems, but want to use BTP and the Integration Suite for their communication to the non-SAP world like Peakboard.

## The RFC function module

In our example we use a very simple, custom function module called Z_PB_DELIVERY_MONITOR. It has one Import parameter I_VSTEL which repesents a shipping point. And it returns a table of delivery rows that are currently in the handled and in the shipping process on this shipping point.
The screenshot shows a call in the test cockpit of transaction SE37. When we submit the shipping point I_VTEL = 1000 it returns 11  deliveries that are currently about to be shipped.

The integration flow we want to build will receive the shippint point I_VSTEL as query parameter and returns the deliveries as JSON table.

![image](/assets/2025-06-05/010.png)

![image](/assets/2025-06-05/020.png)

## Set up the Cloud Connector

The SAP Cloud Connector must be installed on the on prem side of the SAP system that should be brought to the cloud. The binaries can be download [here](https://tools.hana.ondemand.com/#cloud). Here are the steps that must be taken to fully connect it to the BTP portal:

1. The installed instance of the Cloud Connector can be be configured through https://localhost:8443/.
2. In the Cloud Connector configuration, we must provide the subaccount of the BTP instance we want to connect to (the ID and the region is necessary. It can be looked up directly in the BTP).
3. In the sub menu of the sub account we choose the item "Cloud to on-premises" and create a new "Mapping virtual to Internal System". In our case the system is an ABAP system. We must then provide all typical attribute like application host, instance number and also credentials for logging on to the system. 
4. Below the system entry we must provide the Resources we want to connect to. That's the name of our function module. The screenshot shows the configured subaccount, mapped system and resource.

![image](/assets/2025-06-05/030.png)

5. When we switch to the BTP portal and select the corresponding subaccount we can see the active connection under the Cloud Connector tab

![image](/assets/2025-06-05/040.png)

6. The last configuration we have to set up is the "Destination". In the destination entry we must provide the typical attributes like application server, instance number, etc... again. Later in the Integration flow we will refer to the Destination name. It will find it's connection through the corresponding attributes and will "find" the route through Cloud connector. The check connection command will test this route.

![image](/assets/2025-06-05/050.png)

![image](/assets/2025-06-05/060.png)

## Building the Integration Flow

For the Integration Flow we want to translate an inncoming HTTPS call into a the RFC call. So the steps we need to to do is:

1. Configure the HTTPS call that is coming from a sender
2. Prepare the XML for the RFC call
3. Do the actual RFC call
4. Translate the XML answer from the RFC into JSON (and return it to the caller)

The screenshot shows the whole Integration Flow

![image](/assets/2025-06-05/070.png)

1. Configure the HTTPS call that is coming from a sender

No much to do here other than just defining the route for the external call. In our case we just use the name of the function module.

![image](/assets/2025-06-05/080.png)

It's also imprtant to allow the parameter i_vstel to be routed from the external caller to be a header within the integration flow.

![image](/assets/2025-06-05/075.png)

2. Prepare the XML for the RFC call

The actual payload that represents the RFC call is coded in XML. How to form this XML with complex function modules can be checked in [this article](https://community.sap.com/t5/technology-blog-posts-by-sap/cloud-integration-creating-xml-structure-for-remote-function-call-rfc-that/ba-p/13559556). So for our relatively simple function module this is the XML. The actual value of shipping point for the parameter I_VSTEL is filled by the script expression "${header.i_vstel}", so it's taken dynamically from the header.

{% highlight xml %}
<ns1:Z_PB_DELIVERY_MONITOR xmlns:ns1="urn:sap-com:document:sap:rfc:functions">
     <I_VSTEL>${header.i_vstel}</I_VSTEL>
</ns1:Z_PB_DELIVERY_MONITOR>
{% endhighlight %}

![image](/assets/2025-06-05/090.png)

3. Do the actual RFC call

The actual RFC call is done through a Request/Response element. The only thing we need to configure is the name of the destination (see above). That's all. The payload and the name of the function module to be called is defined in the XML. 

![image](/assets/2025-06-05/100.png)

4. Translate the XML answer from the RFC into JSON (and return it to the caller)

The answer from the RFC call is coded in XML as part of the message body. As it's easier to handle JSON on the Peakboard side, we just translate the XML code into JSON by using the corresponding element.

![image](/assets/2025-06-05/110.png)

## Authentification

The caller of the service mut be authenticated. The easiest way to do this is to go to the Integration Runtime in BTP portal and generate a pair of Service Keys. The Client ID and CLient Secret can be used for the call with Basic Authentification as user name and password.

![image](/assets/2025-06-05/120.png)

## The Peakboard application

The implementation on the Peakboard side is pretty easy. The screenshot shows a regular REST call to the our URL with one query parameter named I_VSTEL. We can use the value help (three dots) to choose the right path to the table data within the JSON. When the function module gets more complicated, the path for the prcoessing of the body might be a bit more complicated. But in our case the main payload is just the table of deliveries.

![image](/assets/2025-06-05/130.png)

## Result

We just use a table control with some fancy formatting to present the data. The screenshot shows the final result of the Peakboard application.

![image](/assets/2025-06-05/140.png)

