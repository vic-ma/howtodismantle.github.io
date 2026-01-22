---
layout: post
title: Draw, Sign, Approve – A New Interactive Drawing Area for Peakboard
date: 2023-03-01 00:00:00 +0000
tags: peakboardhub image
image: /assets/2026-01-22/title.jpg
image_header: /assets/2026-01-22/title.jpg
bg_alternative: true
read_more_links:
  - name: More image processing
    url: /category/image
  - name: How to build an auditing app
    url: /Digital-Walkaround-Inspections-Part-I-How-to-Build-an-Audit-App.html
downloads:
  - name: SignBoard.pbmx
    url: /assets/2026-01-22/SignBoard.pbmx
---
With the release of [Peakboard version 4.1](/Peakboard-4.1-Is-Here-And-Its-a-Game-Changer.html), we now have access to a new visual control: the **drawing area.** This control lets users draw on a touchscreen device, by using a finger or a stylus.

You can use this control whenever you want the user to draw or sign something.
You may remember the [audit app we made](/Digital-Walkaround-Inspections-Part-I-How-to-Build-an-Audit-App.html). That's an example of an app where we could use a drawing area to get the user to sign off on any audits they make.

Now, let's go over how the drawing area control works and how to set it up! We'll create a demo app where the user can sign their name and upload the signature to Peakboard Hub.

## The drawing area control

To add a drawing area control, look under the interactive controls, and drag or double-click the pencil icon.

The drawing area control is made up of an empty canvas with some buttons on the corners. The buttons let the end users modify the drawing area:
* Color selection
* Thickness selection
* Clear button (erases the canvas)
* Activation button (enables and disables the drawing area)

You can activate or deactivate individual buttons in the control configuration, under *Appearance > Drawing > Toolbar.*

For our demo, we'll hide all these buttons. That way, the user is presented with a simple blank space where they can sign their name in black, with the default thickness.

![image](/assets/2026-01-22/010.png)

## Prepare the Screen

The order number is stored in a variable. To process the signature, we must place the drawing area control into a group and name the group `SignGroup`. 

![image](/assets/2026-01-22/020.png)

## Processing the Drawing

To process a drawing, we use a universal function. Peakboard offers several Building Blocks to process the screenshot of a group. Our group `SignGroup` consists of only one control: the drawing area control. For uploading the signature, we use the Peakboard Hub file system to store it. However, we could also choose different destinations like SharePoint or email (see the right area of the screenshot). If necessary, we can pass the signature to an external, generic API using Base64 encoding to submit the image for processing.

In our example, the signature is stored with a timestamp and the order number in the filename in a dedicated Hub file system folder. Then, the area is cleared for the next signature.

![image](/assets/2026-01-22/030.png)

## Result

For our test, we sign the order...

![image](/assets/2026-01-22/040.png)

... and check the Peakboard Hub filesystem to confirm the signature is stored successfully.

![image](/assets/2026-01-22/050.png)



