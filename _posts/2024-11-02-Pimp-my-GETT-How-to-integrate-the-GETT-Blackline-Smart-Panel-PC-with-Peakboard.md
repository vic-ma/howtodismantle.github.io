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

Let's assume we want to have the buttons to be lighted with a white border. And we want the border to turn into orange when the corresponding screen is activated. So let's check how to do the color change. The screenshot shows how to call the function SetMultipleKeyColor. We need to to set the color to white (hey code FFFFFF) for the five inactive buttons and one to orange for the active one (color code FFA500). There's a [web page](https://www.rapidtables.com/convert/color/hex-to-rgb.html) for converting colors into Hex codes, in case we don't know which code to use.

![image](/assets/2024-11-02/060.png)

In our sample pbmx we can see, that the color change doesn't take place immediately in the moment the activation of the screen happens. The reason for this is the it always takes some factions of second until the screen builds up. So we use the Activation event of a screen to actually activate a timer of 300ms. And then do the actualy color change when the timer is event is fired. So in reality the screen shows up in the same moment the actual color change of the button takes place.

![image](/assets/2024-11-02/070.png)

## One more thing

Setting the colors of the buttons as explained above is by far the most common use case of the GETT extension, however it also provides some more features what you can do with the buttons.

- Activate / Deactivate the blink mode (just let the button blink)
- Activate / Deactivate the switch mode (use the button to let the user swap between states)
- Set a delay feature to force the user to use a long press to activate something

If the feature makes sense usually is determined by the use cases. Most customer use customized buttons, so not only pure `F1` to `F6` like in the sample pictures.

![image](/assets/2024-11-02/080.png)

## result

The short video shows the script from above in action when running on a GETT BlackLine PC.

{% include youtube.html id="y2L8xzesbls" %}
