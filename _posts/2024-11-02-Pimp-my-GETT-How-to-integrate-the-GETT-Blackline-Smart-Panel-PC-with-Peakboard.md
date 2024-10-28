---
layout: post
title: Pimp my GETT - How to integrate the GETT Blackline Smart Panel PC with Peakboard 
date: 2023-03-01 12:00:00 +0200
tags: hardware
image: /assets/2024-11-02/title.png
read_more_links:
  - name: Peakboard Hardware Guide
    url: /hardwareguide
  - name: GETT website
    url: https://gett-group.com/panel-pc
downloads:
  - name: GettBlackLineShowcase.pbmx
    url: /assets/2024-11-02/GettBlackLineShowcase.pbmx
---
Since the official release of Peakboard BYOD in 2023 more and more Peakboard users install the BYOD runtime on their own devices. This is especially popular in environments with special needs for certain types of screen other than than regular consumer screens - e.g. in food production or under other conditions like extreme heat or extreme dirt.
One of the screens often seen in these environments come fromna German manufactures [GETT](https://gett-group.com/panel-pc).

One of their bestsellers are the Black Line Panel PCs. They come with an RFID reader and 6 special purpose buttons. In this article we want to to discuss the techical details about how to let Peakboard applications interact with these buttons. If you are a proud owner of a Black Line PC already, feel free to download the show case and try it out on your own. 

![image](/assets/2024-11-02/010.jpeg)

## Getting the button press event

GETT designed the buttons in a way, that the the press of a button just simulates the the user presses a button on a regular keyboard. That makes it super easy to catch the event within the Peakboard application just by using the global "Key pressed" event.

![image](/assets/2024-11-02/010.png)



{% include youtube.html id="y2L8xzesbls" %}



