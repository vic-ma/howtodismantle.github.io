---
layout: post
title: Print Charming - How ZPL Turns Your Bixolon XD5-40 printer into a Label Wizard
date: 2023-03-01 02:00:00 +0200
tags: hardware printing
image: /assets/2025-05-12/title.png
image_header: /assets/2025-05-12/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Printing with Peakboard
    url: /category/printing
  - name: Peakboard POS Printer extension
    url: https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter
  - name: Bixolon XD5-40 Series
    url: https://bixoloneu.com/product/xd5-40-series/
  - name: ZPL label designer
    url: https://labelary.com/viewer.html
  - name: ZPL Intro
    url: https://labelary.com/zpl.html
downloads:
  - name: ZPLPrinterDemo.pbmx
    url: /assets/2025-05-12/ZPLPrinterDemo.pbmx
---
A couple of weeks ago, [we already introduced how to print with Peakboard](/The-Art-of-Printing-Getting-started-with-label-printing-on-Seiko-SLP720RT.html). We learned how to use the printer extension by using some high level commands. In [another article](/The-Art-of-Printing-Mastering-Bixolon-SRP-Q300-Series-label-printer-with-with-enhanced-ESCPOS-commands-and-tables.html) we learned how to mix printer commands and lowlevel ESC/POS commands.
In today's article we will discuss the usage of the Zebra programming language ZPL, it's a printer control language developed by Zebra Technologies — a company that specializes in barcode printing and labeling solutions.
We build and print a typical shipping label

## Bixolon XD5-40

In our example we will use the Bixolon XD5-40 printer. It's a compact, direct thermal and thermal transfer label printer designed for high-performance barcode and label printing in various professional environments. Typically used in logistics, retail, healthcare, and manufacturing, this printer excels at producing shipping labels, product tags, barcode stickers, and other adhesive labels with precision and speed. To use it, we simply connect it to the same network as the Peakboard application. That's it. For more details about the printer [here's some additional information](https://bixoloneu.com/product/xd5-40-series/).

The only thing we need to get it working is the IP adress within the local network. With the Peakboard printer extension, no additional drivers must be installed.

![image](/assets/2025-05-12/010.png)

## ZPL

ZPL - Zebra programming language - is used to design and print labels, barcodes, and receipts on Zebra thermal printers. It's a text-based language — essentially a series of ASCII commands that describe what the printed output should look like

A perfect and easy-to-understand introduction on ZPL can be found [here](https://labelary.com/zpl.html).

Here's a minimum intro:

{% highlight text %}
^XA
^FO50,50^ADN,36,20^FDHello, ZPL!^FS
^XZ
{% endhighlight %}

.. and here's the explanation:

- "^XA" starts the label format.
- "^FO50,50" positions the field.
- "^FDHello, World!^FS" prints some text.
- "^XZ" ends the label.

The perfect tool in the context of ZPL is the [Labelary ZPL designer](https://labelary.com/viewer.html). It's very helpful to developthe base layout of the label. After having settled the layout we can just replace all variable values with placeholders and fill it in Peakboard.

![image](/assets/2025-05-12/020.png)

## The Peakboard Application



## result

In the video we can see, how the actual lable is printed on Bixolon XD5-40.

{% include youtube.html id="JxS4E6D1dJw" %}
