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

One of the screens often seen in these environments come from the German manufacturer [GETT](https://gett-group.com/panel-pc). Some of their bestsellers are the BlackLine Panel PCs. These come with an RFID reader and 6 special-purpose buttons.

In this article, you'll learn how to let Peakboard applications interact with these buttons. If you're a proud owner of a BlackLine PC, feel free to download [the showcase](/assets/2024-11-02/GettBlackLineShowcase.pbmx) and try it out on your own.

![image](/assets/2024-11-02/010.jpeg)

## Get the button press event

GETT designed their buttons to simulate a normal keyboard press. That makes it super easy to catch the event within the Peakboard application by using the global "Key pressed" event.

![image](/assets/2024-11-02/020.png)

Within the "Key pressed" event, we check if the pressed key is one of the function keys (`F1` to `F6`), and then show the corresponding screen. That's all!

![image](/assets/2024-11-02/030.png)

## Make the keys light up

The simple button press is not the only nice feature that the GETT BlackLine offers. The button experience gets even better when we use the button's underlying LED lighting.

For Peakboard to be able to use the button lights, we need the "Gett HMI Keys" extension. It can be installed like any other extension, in the extension section of the data sources.

![image](/assets/2024-11-02/040.png)

To control the lights, we need exactly one instance of the data source available in the project. So we just add the data source, refresh the data, and that's it. No need to provide any additional properties.

In fact, you can see this as a fake data source without any proper data. The only reason we need it is to give us access to the built-in functions, in Building Blocks.

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
