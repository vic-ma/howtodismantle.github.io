---
layout: post
title: ByeBye Paper - Going paperless with Peakboard, SAP, and Woutex e-Ink Displays
date: 2023-03-01 00:00:00 +0200
tags: hardware sap
image: /assets/2024-10-01/title.png
read_more_links:
  - name: Woutex Paperless
    url: https://woutex-paperless.de/
downloads:
  - name: CorporateDesignTemplate.pbmx
    url: /assets/2024-10-01/CorporateDesignTemplate.pbmx
---
Peakboard applications are often used in paperless environments. But how to do that when an additional piece of paper is actually necessary to identify a palet, a package or a warehouse a bin? One possible answer is to transfer the necessary information to a paperless e-ink display to replace paper. In this article we will learn how to that transfer from the Peakboard app to [Woutex e-ink display](https://woutex-paperless.de/). And to make things even more fancy we don't use just random information, but we use a outbound delivery from SAP as the logical basis for the display.

![image](/assets/2024-10-09/010.png)

Usually the Woutex e-ink displays come with a low energy architecture. This means the batteries of the display will last for years. To achieve this the display itself does not consume any energy as long as it's not changed. The display doesn't use WiFi but a low energy radio connection to a hosting station, or central hub. This hub is connected to a regular server within the network that exposes JSON/REST endpoints that we will used in this article to adjust the content of the displays. 

## Getting data from SAP

We discussed [SAP connectivity uncountable times}(https://how-to-dismantle-a-peakboard-box.com/category/sap) in this blog. We let the user provide a delivery number (e.g. through scanning a barcode). The delivery data is then queried from SAP through the press of a button. The following screnshot shows the design...

![image](/assets/2024-10-09/020.png)

And here is the LUA code to get the data from SAP. Actually we are just reading two tables. The first table is LIKP, the delivery header data, be using the given delivery number. The second table is KNA1, containing customer master data, with the customer number that was within the result of the first table select. The result columns like customer number, shipping point, weight and customer name is written to the text boxes on the screen. For more details on Lua and SAP, please refer to [this aticle](https://how-to-dismantle-a-peakboard-box.com/SAP-on-fire-how-to-perfectly-integrate-LUA-scripting-with-SAP.html).

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

## Connecting to the Woutex server

The easiest way to connect to he Woutex server is by using the Woutex extension which is availabe to the public through the extension dialog. For information about how to install an extension, see the [Manage extensions documentation](https://help.peakboard.com/data_sources/Extension/en-ManageExtension.html).

![image](/assets/2024-10-09/030.png)

The datasource is easy to use. Just fill in the Woutex server and credentials. As a rule, any datasource must produce some output even if we don't need it. So we just click on the data load button once to load a dummy column, otherwise the data source can't be saved.

![image](/assets/2024-10-09/040.png)

## Building the Woutex connectvity

After having installed and configured the Woutex extension, we can just use some functions to send data to the display or reset it. The first parameter is always the display ID. It's a number that corresponds to the barcode on the display.

To send data to the display we use the function 'ChangeContent', beside the display number it receives a JSON string. Within the JSON string we provide the name of the template (as defined in the backend of the Woutex server) along with all placeholders within the template. In our case these are parameters like order number, customer number, etc....

Here's a sample JSON:

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

To build the JSON string in a way that is easy to maintain, we use placholders in the JSON string and use the Placholder Text Building Block to exchange the placeholders with real values. The real values are read from the text boxes where we stored the values that come from SAP orginally. And that's it. This single call is sufficient to apply any template on the display with the given number.

![image](/assets/2024-10-09/050.png)

To reset a display that function 'ResetContent'.

![image](/assets/2024-10-09/060.png)

## result

Getting data from SAP and send it to the Woutex e-ink display is straight forward and can be done with only few steps. We must understand that when the data is transferred to the display it usually takes between 5 and 20 seconds. The reasons for this is that the radio connection to the base station is designed to save as much energy as possible.

![image](/assets/2024-10-09/result2.gif)

![image](/assets/2024-10-09/result.jpg)
