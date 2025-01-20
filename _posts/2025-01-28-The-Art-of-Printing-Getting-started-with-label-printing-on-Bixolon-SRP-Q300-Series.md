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
In industrial applications, Peakboard Boxes are often connected to other devices. This is true for large manufacturing machines and conveyor systems, but also for smaller devices like printers. We've discussed how to use Peakboard with [e-ink displays](/ByeBye-Paper-Going-paperless-with-Peakboard-and-Woutex-e-Ink-Displays.html), but we often still find printed tags, papers, and labels in modern logistic processes. So in today's article, we will discuss how to integrate a label printer into Peakboard.

For our example, we'll use an SRP-Q300 series printer from Bixolon. Peakboard is based on Microsoft Windows, so it would be obvious to use a Windows driver to communicate with the printer. However, we don't do this, because of performance reasons. Responsiveness is critical in any human-machine interaction. So, our Peakboard application will instead communicate directly with the printer through a TCP/IP connection. This is by far the fastest way to communicate.

In the modern world of label printing, there are two main protocols that are used. They are ZPL (Zebra Programming Language) and ESC/POS (Epson Standard Code for Point of Sale). For our example, we will use higher level commands provided by the POS data source extension that encapsulate ESC/POS commands.

## The extension

To make things as easy as possible, we'll use a Peakboard extension called "POS Printer." For more information about this extension, see the [POS Printer GitHub](https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter) or the [POS Printer extension page](https://templates.peakboard.com/extensions/POSPrinter/index).

![image](/assets/2025-01-28/010.png)

In the data source, we enter the IP address and port on which the printer is listening to our commands. Our Bixolon SRP-Q300 printer doesn't provide any web interface for configuration. We can check the manual to find the network configuration.

![image](/assets/2025-01-28/020.png)

## Prepare the Peakboard application

The actual Peakboard application is pretty simple. Besides the data source for the printer connectivity, we also add three combo boxes. These let the user select the product, the size, and the addition to be printed on a label.

The application works like a coffee shop, where we take the order first and then print a label with all the details that are used in the preparation process. The options for the three combo boxes are taken from three variable lists containing the products, sizes, and additions. The actual magic happens behind the button. 

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

