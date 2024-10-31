---
layout: post
title: Pimp my GETT - How to integrate the GETT BlackLine Smart Panel PC with Peakboard
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2024-11-02/title.jpg
read_more_links:
  - name: Peakboard Hardware Guide
    url: /hardwareguide
  - name: GETT website
    url: https://gett-group.com/panel-pc
downloads:
  - name: GettBlackLineShowcase.pbmx
    url: /assets/2024-11-02/GettBlackLineShowcase.pbmx
---

Since the official release of Peakboard BYOD in 2023, more and more Peakboard users have installed the BYOD runtime on their own devices. This is especially popular in environments with special requirements for screens---for example, in food production or in extreme heat or dirt.

Some of the screens commonly seen in these environments are the BlackLine Panel PCs by German manufacturer [GETT](https://gett-group.com/panel-pc). These come with an RFID reader and 6 special-purpose buttons.

In this article, you'll learn how to let Peakboard applications interact with these buttons. If you're a proud owner of a BlackLine PC, feel free to download [the showcase](/assets/2024-11-02/GettBlackLineShowcase.pbmx) and try it out on your own.

![image](/assets/2024-11-02/010.jpeg)

## Get the button press event

GETT designed their buttons to simulate a normal keyboard press. This makes it super easy to catch the event within the Peakboard application by using the global "Key pressed" event.

![image](/assets/2024-11-02/020.png)

Within the "Key pressed" event, we check if the pressed key is one of the function keys (`F1` to `F6`), and then show the corresponding screen. That's all!

![image](/assets/2024-11-02/030.png)

## Make the keys light up

The GETT BlackLine offers more than a simple button press. The buttons also have LED lighting.

For Peakboard to be able to use the button lights, we need the "Gett HMI Keys" extension. It can be installed like any other extension, in the **Extensions** section of the data sources.

![image](/assets/2024-11-02/040.png)

To control the lights, we need exactly one instance of the data source in the project. So we add the data source and refresh the data.

You can see this as a fake data source without any real data. The only reason we need it is to give us access to the built-in functions, in Building Blocks.

![image](/assets/2024-11-02/050.png)

Let's say we want to give the buttons a white border. And we want the border to turn orange when the corresponding screen is activated.

The following screenshot shows how to call the `SetMultipleKeyColor` function. We set the five inactive buttons to white (color code `#FFFFFF`). And we set the active one to orange (color code `#FFA500`).

Also, here's a website for [converting hex codes and RGB values](https://www.rapidtables.com/convert/color/hex-to-rgb.html).

![image](/assets/2024-11-02/060.png)

In our script, we don't change the color at the exact same time that we change the screen. We use the activation event of a screen to start a timer of 300ms. Once that timer expires, we change the colors.

The reason for this is that it always takes some factions of a second before the screen loads. The added delay makes the screen show up at the exact same time that the buttons change color.

![image](/assets/2024-11-02/070.png)

## One more thing

Setting the colors of the buttons as explained above is by far the most common use case of the GETT extension. However, it also allows you to do some others things:

- Activate/deactivate the blink mode (makes the button blink)
- Activate/deactivate the switch mode (use the button to let the user swap between states)
- Set a delay feature to force the user to use a long press to activate something

If the feature makes sense usually is determined by the use cases. Most customer use customized buttons, so not only pure `F1` to `F6` like in the sample pictures.

![image](/assets/2024-11-02/080.png)

## Result

The short video shows the script from above in action when running on a GETT BlackLine PC.

{% include youtube.html id="y2L8xzesbls" %}
