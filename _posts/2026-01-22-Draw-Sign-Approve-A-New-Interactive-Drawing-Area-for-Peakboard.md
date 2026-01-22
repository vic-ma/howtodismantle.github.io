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

## Add the drawing area control

To add a drawing area control, look under the interactive controls, and drag or double-click the pencil icon.

The drawing area control is made up of an empty canvas with some buttons on the corners. The buttons let the end users modify the drawing area:
* Color selection
* Thickness selection
* Clear button (erases the canvas)
* Activation button (enables and disables the drawing area)

You can activate or deactivate individual buttons in the control configuration, under *Appearance > Drawing > Toolbar.*

For our demo, we'll hide all these buttons. That way, the user is presented with a simple blank space where they can sign their name in black, with the default thickness.

![image](/assets/2026-01-22/010.png)

## Add an order number variable

To make our demo app a little more realistic, let's pretend like the purpose of the signature is to sign off on an order. So, we create a new `OrderNo` variable and store a random number in it. This represents the order number of the order that the user must sign off on.

![image](/assets/2026-01-22/020.png)

## Add a submit button

Next, we add a *Confirm and Submit to Hub* button. The user presses this button when they are finished with their signature and is ready to upload it to Peakboard Hub.

### Create the script

Now, let's create the tapped script for the button. Here's what the finished script looks like:

![image](/assets/2026-01-22/030.png)

The main function that we use is *CONTROLS > Screen1 > Groups > Save screenshot in Peakboard Hub*. We use the function to capture a screenshot of the `SignGroup` group.`SignGroup` only contains our drawing area control, so the screenshot will only contain the user's signature.

We also set the folder and file name to save the screenshot under, inside Peakboard Hub. Of course, you can also choose to save the screenshot elsewhere, like Sharepoint of email. All you need to do is choose one of the other Building Blocks, under *CONTROLS > Screen1 > Groups.* You can even send the signature to an arbitrary system, by converting the screenshot to Base64 with *Gen

However, we could also choose different destinations like SharePoint or email (see the right area of the screenshot). If necessary, we can pass the signature to an external, generic API using Base64 encoding to submit the image for processing.

In our example, the signature is stored with a timestamp and the order number in the filename in a dedicated Hub file system folder. Then, the area is cleared for the next signature.


## Result

For our test, we sign the order...

![image](/assets/2026-01-22/040.png)

... and check the Peakboard Hub filesystem to confirm the signature is stored successfully.

![image](/assets/2026-01-22/050.png)



