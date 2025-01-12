---
layout: post
title: The Art of Printing - Getting started with label printing on Bixolon SRP-Q300 Series
date: 2023-03-01 02:00:00 +0200
tags: hardware printing
image: /assets/2025-01-28/title.jpg
image_header: /assets/2025-01-28/title_landscape.jpg
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

In today's article we will go our first steps and learn how to integrate a label printer. In our sample we will use a printer of the SRP-Q300 Series provided by Bixolon. Peakboard is based on Microsoft Windows, so it would be obvious to use the Windows driver model interact with a Windows hosted piece of software and a printer. We don't do this. The main reason is performance. In any environment in which application responsiveness to the what the user wants is a very crucial point. So we communicate directly between our Peakboard application and the printer on the basis of direct TCP/IP connection. That's by far the fastest and most responiviest way to do it.
In a modern world of label printing, there are mainly two important protocols to by used in that cases. One is called ZPL (Zebra Programming Language), the other one is ESC/POS (Epson Standard Code for Point of Sale). FOr our sample, the Bixoon printer, we will use ESC/POS.

## The extension

To make it as easy as possible, we will use a Peakboard extension called "POS Printer". More information about this extension can be found directly at [github](https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter), or on the [extension website](https://templates.peakboard.com/extensions/POSPrinter/index).

![image](/assets/2025-01-28/010.png)

In the data source, we fill the ip address and a port on which the printer is listening to out commands. 

![image](/assets/2025-01-28/020.png)
