---
layout: post
title: The Art of Printing - Getting started with label printing on Bixolon SRP-Q300 Series
date: 2023-03-01 02:00:00 +0200
tags: hardware printing
image: /assets/2025-01-28/title.png
image_header: /assets/2025-01-28/title_landscape.png
read_more_links:
  - name: Printing with Peakboard
    url: /category/printing
  - name: Peakboard POS Printer extension
    url: https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter
  - name: ZPL Online Designer
    url: https://labelary.com/viewer.html
  - name: SRP-Q300 Series
    url: https://bixoloneu.com/product/srp-q300-series/
downloads:
  - name: ESCPOSPrinter.pbmx
    url: /assets/2025-01-28/ESCPOSPrinter.pbmx
---
Peakboard as a platform for any kind of industrial applications is very often used together with other devices. Often that's true for huge manufactoring machines or conveyers, but also for simple things like printers. Although we already discussed [how to get rid of paper and use e-ink displays](/ByeBye-Paper-Going-paperless-with-Peakboard-and-Woutex-e-Ink-Displays.html), we still often find printed tags, papers, or labels in modern logistic processes.

In today's article we will go our first steps and learn how to integrate a label printer. In our sample we will use a printer of the SRP-Q300 Series provided by Bixolon. Peakboard is based on Microsoft Windows, so it would be obvious to use the Windows driver model interact with a Windows hosted piece of software and a printer. We don't do this. The main reason is performance. Responsiveness is a crucial point in any human to machine interaction. So we communicate directly between our Peakboard application and the printer on the basis of direct TCP/IP connection. That's by far the fastest and most responiviest way to do it.
In a modern world of label printing, there are mainly two important protocols to by used in that cases. One is called ZPL (Zebra Programming Language), the other one is ESC/POS (Epson Standard Code for Point of Sale). For our example, the Bixon printer, we will use ESC/POS commands that are encapsulated in some high level commands provided by the POS data source extension.

## The extension

To make it as easy as possible, we will use a Peakboard extension called "POS Printer". More information about this extension can be found directly at [github](https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter), or on the [extension website](https://templates.peakboard.com/extensions/POSPrinter/index).

![image](/assets/2025-01-28/010.png)

In the data source, we fill the ip address and a port on which the printer is listening to our commands. The Bixolon SRP-Q300 printer we're using in this article doesn't provide any web interface for configuration. We can check the manual to find out the network configuration.

![image](/assets/2025-01-28/020.png)

## Preparing the Peakboard application

The actual Peakboard application is pretty simple. Beside the data source for the printer connectivity we have three combo boxes for letting the user select the product, the size and addition to be printed on a label. We do this like it would be done in a coffee shop to take the order first and then print a label with all the details that used in the actual preparation process. the three combo boxes are fed from three variable lists containing the products, sizes and additions. The actual magic is happening behind the button. 

![image](/assets/2025-01-28/030.png)

## Scripting the printed label

The actual printing process is triggered by one single function provided by the Printer data source. It receives just a text string representing the label. ESC/POS is a protocol that is usually defined by literals and HEX commands for the formatting. The details can be found [here](https://download4.epson.biz/sec_pubs/pos/reference_en/escpos/index.html). To make it easier for Peakboard users, the POS Printer data source offers its own simple commands, that are translated into ESC/POS. A reference for the available commands is [here](https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter). Although it is possible to mix both the simple commands and real ESC/POS we only use the simple commands in this example and save the ESC/POS part for a different article.

In our example we use the following command string. The first line prints out the coffee shop logo. It's a just a simple png image with less than 300px of width that is added to the Peakboad project as local resource. The preceding "~(CentralAlign)~" makes it centered.
Then we see three actual text parts, the product, the size, the addition. And we end up with a command for cutting the paper. That's it.

{% highlight text %}
~(CentralAlign)~~(Image:Starpeak_small.png)~
~(Style:DoubleHeight,DoubleWidth)~~(LeftAlign)~Caramel Macchiato

Size: Large
~(Style:Bold)~
Additions: Cream
~(FullCutAfterFeed:2)~
{% endhighlight %}

The whole label is created using the POS printer extension high level language, no pure ESC/POS.

## The code

The screenshot shows the Building Block for sending the command. We're using the placeholder replacement block to replace the three literals in the string mentioned above with dynamic values from the user input.

![image](/assets/2025-01-28/040.png)

For LUA lovers, here's the LUA code that does the same:

{% highlight lua %}
local _ = data.MyPrinter.print(string.gsubph([[~(CentralAlign)~~(Image:Starpeak_small.png)~
~(Style:DoubleHeight,DoubleWidth)~~(LeftAlign)~#[Product]#

Size: #[Size]#
~(Style:Bold)~
Additions: #[Addition]#
~(FullCutAfterFeed:2)~]], 
  { Product = screens['Screen1'].cmbProduct.selectedvalue, 
  Size = screens['Screen1'].cmdSize.selectedvalue, 
  Addition = screens['Screen1'].cmdAddition.selectedvalue })
, nil)
{% endhighlight %}

## Result

The videos shows the final result in action. It's very impressive to see that the printing starts almost without any delays. 

{% include youtube.html id="pdjLAC5k6fA" %}

