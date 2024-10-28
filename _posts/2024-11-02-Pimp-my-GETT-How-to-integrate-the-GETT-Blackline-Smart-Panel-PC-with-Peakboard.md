---
layout: post
title: Pimp my GETT - How to integrate the GETT Blackline Smart Panel PC with Peakboard 
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
Since the official release of Peakboard BYOD in 2023 more and more Peakboard users installed the BYOD runtime on their own devices. This is especially popular in environments with special requirements for certain types of screens other than than regular consumer screens - e.g. in food production or under other conditions like extreme heat or extreme dirt.
One of the screens often seen in these environments come from the German manufacturer [GETT](https://gett-group.com/panel-pc).

One of their bestsellers are the Black Line Panel PCs. They come with an RFID reader and 6 special purpose buttons. In this article we want to to discuss the technical details about how to let Peakboard applications interact with these buttons. If you are a proud owner of a Black Line PC already, feel free to download the show case and try it out on your own. 

![image](/assets/2024-11-02/010.jpeg)

## Getting the button press event

GETT designed the buttons in a way, that the the press of a button just simulates the the user presses a button on a regular keyboard. That makes it super easy to catch the event within the Peakboard application just by using the global "Key pressed" event.

![image](/assets/2024-11-02/020.png)

Within the "Key pressed" event, we just check, if the pressed key is one of the function keys (F1 to F6) and then call the corresponding screen according to the function key. That's all!

![image](/assets/2024-11-02/030.png)

## Let the keys light up

The pure button press is not the only nice feature combined with the GETT Black Line. The button experience even gets better when addressing the button's underlying LED lighting. For a direct interaction between Peakboard and the button lights we need the GETT extensions. It can be installed as any other extension too, in the extension section of the data sources.

![image](/assets/2024-11-02/040.png)

To use this we need exactly one instance of the datasource available in the project, so we just add it, refresh the data and that's it. No need to provide any additional properties. In fact it can be seen as a fake datasource without proper data. The only reason we need this, is that the built-in-functions are now available to be used in the Building Blocks. 

![image](/assets/2024-11-02/050.png)

Let's assume we want to have the buttons to be lighted with a white border. And we want the border to turn into orange when the corresponding screen is activated. So let's check how to do the color change. The screenshot shows how to call the function SetMultipleKeyColor. We need to to set the color to white (hey code FFFFFF) for the five inactive buttons and one to orange for the active one (color code FFA500). There's a [web page](https://www.rapidtables.com/convert/color/hex-to-rgb.html) for converting colors into Hex codes, in case we don't know which code to use. 

![image](/assets/2024-11-02/060.png)

In our sample pbmx we can see, that the color change doesn't take place immediately in the moment the activation of the screen happens. The reason for this is the it always takes some factions of second until the screen builds up. So we use the Activation event of a screen to actually activate a timer of 300ms. And then do the actualy color change when the timer is event is fired. So in reality the screen shows up in the same moment the actual color change of the button takes place.

![image](/assets/2024-11-02/070.png)

## One more thing

Setting the colors of the buttons as explained above is by far the most common use case of the GETT extension, however it also provides some more features what you can do with the buttons.

* Activate / Deactivate the blink mode (just let the button blink)
* Activate / Deactivate the switch mode (use the button to let the user swap between states)
* Set a delay feature to force the user to use a long press to activate something

If the feature makes sense usually is determined by the use cases. Most customer use customized buttons, so not only pure F1 to F6 like in the sample pictures.

![image](/assets/2024-11-02/080.png)

## result

The short video shows the script from above in action when running on a GETT Black Line PC.

{% include youtube.html id="y2L8xzesbls" %}



