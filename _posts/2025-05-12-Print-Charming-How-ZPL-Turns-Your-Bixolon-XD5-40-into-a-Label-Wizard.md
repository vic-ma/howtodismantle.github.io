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
A couple of weeks ago, we gave an introduction on [how to print with Peakboard](/The-Art-of-Printing-Getting-started-with-label-printing-on-Seiko-SLP720RT.html), where we explained how to use the printer extension with high level commands. In [another article](/The-Art-of-Printing-Mastering-Bixolon-SRP-Q300-Series-label-printer-with-with-enhanced-ESCPOS-commands-and-tables.html), we explained how to mix printer commands and low-level ESC/POS commands.

In today's article, we will explain how to use the Zebra programming language, or ZPL. It's a printer control language developed by Zebra Technologies---a company that specializes in barcode printing and labeling solutions.

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

The perfect tool in the context of ZPL is the [Labelary ZPL designer](https://labelary.com/viewer.html). It's very helpful to develop the base layout of the label. After having settled the layout we can just replace all variable values with placeholders and fill it in Peakboard.

![image](/assets/2025-05-12/020.png)

## The Peakboard Application

In the Peakboard application we need a data source to connect to the printer with Pos printer extension

![image](/assets/2025-05-12/030.png)

For the example we use a very simple UI and let the user type in a Delivery Number, the size of the package and the Weight.

![image](/assets/2025-05-12/040.png)

Out shipping label contains a couple of elements, e.g. the sender address, receiver address, a base-64-encoded company logo and three example placeholders #[DeliveryNumber]#, #[Size]# and #[Weight]#.
Here's the ZPL coded label for our shipping process:

{% highlight text %}
^XA
^FX Top section with logo, name and address.
^CI28
^FO50,50^GFA,4975,4975,25,,:::::::::::::::::::::::::::::::::::gJ0C,gI01E,gI03F,gI07F8,gI0FFC,gH01FFE,gH01EFF,gH03EFF,gH07CFF8,gH0F8FFC,gG01F0FFE,gG03E0IF,gG07C0IF8,gG07C0IFC,gG0F80IFE,g01F00JF,g03E007IF8,X0387C003IF802,X07CF8003IFC0F,X0IFI07IFE1F8,W01IFI0KF7FC,W03FFE060MFE,W07FFC1E1NF,W0F9FC3F3LF3F8,W0FBFC7F7KFE3FC,V01F3PFC1FE,V03E7PFE1FE,V07C7QF1FF,V0F87QF8FF8,U01F0RFEFFC,U03E0RFE7FE,U07C1VF,U0F83VF8,T01F0FFBTFC,T03E3FF9TFE,T07C7FF9TFE,T07DIF9UF,T0FBIF1UF8,S01KF1UFC,S03JFE1JF803NFE,S07JFC3JFN01IF,S0KFC3JFN01IF8,R01KF87JFN01IFC,R03KF8KFN01IFE,R07KF8KFN01IFE,R0LFCKFN01E01F,Q01LFCKFN01C00F,Q03LFEKFN01C007,Q03RF8M01C7C78,Q03SFM01C7E38,T0FN03FCL01C7F1C,T06N03FEL01C7F1E,T06O01FL01C7F8E,T07P0F8K01C7FC7,T0718N078K01C7FC78,T0718L03C38K01C7FE38,T0718L03E38K01C7FE1C,T0718L07E3CK01CI01E,T0718L07E3CK01CJ0E,T071CL07E38K01CJ0E,T070CL07E38K01CJ0E,T038CL07C38K01CJ0E,T038EL0FC7L01CJ0E,T0386L0F07MFCJ0E,T03C6N0NFCJ0E,T01C3M03NFC0FC0E,T01E38L07JFI01C1FF0E,U0E18J03FFCF8FI01C3FF8E,U0FL07FF8E078001C3878E,U078K0IF0E038001C703CE,U03CJ01JFE03LF01FC,U03EJ03JFE03LF01FC,U01F8I0KFE07LF03C,S0TF0MF878,R01gJF,:,::::::Y03CP03E07C,M03FF8N03CP07E1FC,M03FFEN03CP07E1FC,M03IFN03CP0F83E,M03E3FN03CP0F03C,M03E0F8M03CP0F03C,M03E0F83E007E03C3E07C01F03FE7F81F003E,M03E0F87F81FF83C3E1FF0FFC3FEFFC7FC0FF8,M03E0F9FFC1FFC3C7C3FF1FFE3FEFFCFFE1FFC,M03E0F9E3E1C7C3CF87FF1IF3FEFFDF1E1E3E,M03E3F3C1E103C3DF0F813E1F0F03C1E0F3C1E,M03IF3C1E001E3DE0F003C0F8F03C3C0F3C1E,M03FFC3FFE07FE3FC0F007C0F8F03C3IF3FFE,M03FF83FFE1FFE3FC0F007C078F03C3IF3FFE,M03E003FFE3FFE3FE0F007C078F03C3IF3FFE,M03E003C003C1E3DF0F003C0F8F03C3C003C,M03E003C00783E3CF0F803C0F0F03C1E003C,M03E003E047C3E3CF8FC73F3F0F03C1F063E0C,M03E001FFC3FFE3C7C7FF1FFE0F03C1FFE1FFC,M03EI0FFC3FFE3C3E3FF0FFC0F03C0FFE0FFC,M03EI03F81F9E3C3E0FE03F00F03C03FC07F8,,:::::::T0C01F01F8107C3FC607E0F8,T0C03B839C10E62E860E70CC,T0C061C30010C006061C20C,T0C060C700107006061800E,T0C060C63C103C060618007C,T0C060C30C1006060618I0E,T0C071C30C10C606060C30C6,T0FC3F81FC18FE06060FE0FC,T0FC1F00F8103C060603C078,,:::::::::::::::::::::::::::::::::::::::::^FS
^CF0,50
^FO300,50^FDPeakcoffee Logistics Inc.^FS
^CF0,30
^FO300,115^FD1000 Bean Highway^FS
^FO300,155^FDRoastchester, OR 97200^FS
^FO300,195^FDUnited States (USA)^FS
^FO50,250^GB700,3,3^FS

^FX Second section with recipient address and permit information.
^CFA,30
^FO50,300^FDWorkaround GmbH^FS
^FO50,340^FDRupert-Mayerstr. 44^FS
^FO50,380^FDMünchen, BY 81379^FS
^FO50,420^FDGermany^FS
^CFA,15
^FO600,300^GB150,150,3^FS
^FO638,340^FDPermit^FS
^FO638,390^FD40404^FS
^FO50,500^GB700,3,3^FS

^FX Third section with bar code.
^BY5,2
^FO50,550
^BCN,180,Y,N,N
^FD#[DeliveryNumber]#^FS
^FO50,850^GB700,3,3^FS

^FX Third section with package information.
^CFA,30
^FO50,890^FDPackage Size: #[Size]#^FS
^FO50,930^FDWeight: #[Weight]#^FS

^XZ
{% endhighlight %}

In the code behind the print button, we just replace the three placeholders with the actual values from the user and send the result string to the printer. So the whole printing process is done with only one building block. In real life we might need to get the sender's address from the ERP system first. But to keep it simple we skip this step and just use a fxied sender's address as sample.

![image](/assets/2025-05-12/050.png)

## result

In the video we can see, how the actual lable is printed on Bixolon XD5-40.

{% include youtube.html id="XDnLkoAyqEw" %}

![image](/assets/2025-05-12/060.png)