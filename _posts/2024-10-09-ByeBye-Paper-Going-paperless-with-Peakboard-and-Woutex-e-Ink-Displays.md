---
layout: post
title: ByeBye Paper - Going paperless with Peakboard, SAP, and Woutex e-Ink Displays
date: 2023-03-02 01:00:00 +0200
tags: hardware sap
image: /assets/2024-10-09/title.png
read_more_links:
  - name: Woutex Paperless
    url: https://woutex-paperless.de/
downloads:
  - name: WoutexDemoboard.pbmx
    url: /assets/2024-10-09/WoutexDemoboard.pbmx
---
Peakboard applications are often used in paperless environments. But what if a piece of paper is necessary to identify a pallet, a package, or a warehouse bin? One answer is to use a paperless e-ink display.

In this article, you will learn how to transfer data from a Peakboard app to a [Woutex e-ink display](https://woutex-paperless.de/). And to make things even more fancy, we won't just use random information. Instead, we'll use an outbound delivery from SAP as the logical basis for the display.

![image](/assets/2024-10-09/010.png)

Most Woutex e-ink displays have a low-energy architecture. This means the batteries in the display will last for years. To achieve this, the display doesn't consume any energy unless the image changes. Also, the display doesn't use Wi-Fi. It uses a low energy radio connection to a hosting station, or central hub. This hub is connected to a regular server within the company network that exposes JSON/REST endpoints. These endpoints adjust the content of the displays. 

## Get data from SAP

We've discussed [SAP connectivity](https://how-to-dismantle-a-peakboard-box.com/category/sap) many times in this blog. For our dashboard, we let the user provide a delivery number (for example, by scanning a barcode). The delivery data is then queried from SAP with the push of a button.

![image](/assets/2024-10-09/020.png)

The following is the LUA code that gets the data from SAP. It reads two tables:
1. It reads LIKP, the delivery header data, with the given delivery number.
2. It reads KNA1, which contains customer master data, with the customer number from the first table query.

The resulting columns, like customer number, shipping point, weight, and customer name, are written to the text boxes on the screen. For more details on Lua and SAP, see our article on [integrating LUA scripting with SAP](https://how-to-dismantle-a-peakboard-box.com/SAP-on-fire-how-to-perfectly-integrate-LUA-scripting-with-SAP.html).

{% highlight lua %}
local vals = {}
local con = connections.getfromid('pryG92a+fHDgAKI6hdZ6DiNGqtw=')

vals.MyVBELN = screens['Screen1'].DeliveryNo.text

con.execute("SELECT KUNNR, BTGEW, VSTEL FROM LIKP INTO @MyLIKP WHERE VBELN = @MyVBELN;", vals)

screens['Screen1'].txtShippingPoint.text = vals.MyLIKP[0].VSTEL
screens['Screen1'].txtWeight.text = vals.MyLIKP[0].BTGEW
screens['Screen1'].txtCustomerNo.text = vals.MyLIKP[0].KUNNR
vals.CustomerNo = vals.MyLIKP[0].KUNNR

con.execute("SELECT NAME1 FROM KNA1 INTO @MyKNA1 WHERE KUNNR = @CustomerNo;", vals)

screens['Screen1'].txtCustomerName.text = vals.MyKNA1[0].NAME1
{% endhighlight %}

## Connect to the Woutex server

The easiest way to connect to the Woutex server is by using the Woutex extension, which is available through the extension dialog. For information on how to install an extension, see the [Manage extensions](https://help.peakboard.com/data_sources/Extension/en-ManageExtension.html) documentation.

![image](/assets/2024-10-09/030.png)

The data source is easy to use. Just fill in the Woutex server URL and credentials. As a rule, a data source must produce some output, even if we don't need it. So we must click on the data load button to load a dummy column. Otherwise, the data source can't be saved.

![image](/assets/2024-10-09/040.png)

## Build Woutex connectvity

After having installed and configured the Woutex extension, we can use functions that send data to the display or reset the display. The first parameter is always the display ID---a number that corresponds to the barcode on the display.

To send data to the display, we use the `ChangeContent` function. Besides the display ID, we pass in a JSON string with the following fields:
1. The name of the template (as defined in the backend of the Woutex server).
2. Values for all the variables in the template. In our case, these are things like order number and customer number.

Here's an example of a JSON string we'd send:

{% highlight json %}
  { "template" : "peakboard-7.5",
         "data":{
             "order-nr": "9273618",
             "customer-nr": "123456789",
             "customer-name": "Customer GmbH",
             "text-1": "Lorem ipsum dolor sit amet",
             "text-2": "consectetur adipiscing elit."
         }
  }
{% endhighlight %}

To build the JSON string in a way that is easy to maintain, we put placeholders in the JSON string, and we use the Placeholder Text Building Block to replace the placeholders with actual values. The actual values come from the text boxes where we stored the data from SAP. And that's it. This call is all we need to apply a template to the display.

![image](/assets/2024-10-09/050.png)

To reset a display, we use the `ResetContent` function:

![image](/assets/2024-10-09/060.png)

## Result

Getting data from SAP and sending it to a Woutex e-ink display is straightforward and can be done with only few steps. Note that when the data is transferred to the display, it usually takes between 5 and 20 seconds. The reason for this is that the radio connection to the base station is designed to save as much energy as possible.

![image](/assets/2024-10-09/result2.gif)

![image](/assets/2024-10-09/result.jpg)
