---
layout: post
title: Lights On - A Step-by-Step Guide to Designing a Pick-by-Light System with Captron and Peakboard
date: 2023-03-01 03:00:00 +0200
tags: hardware
image: /assets/2025-03-01/title.png
image_header: /assets/2025-03-01/title_landscape.png
read_more_links:
  - name: Captron Pick-by_light
    url: https://captron-solutions.com/en/pick-by-light/
downloads:
  - name: CaptronPBL.pbmx
    url: /assets/2025-03-01/CaptronPBL.pbmx
---

{% include youtube.html id="s8Uh0ExfEk8" %}

{ "Content": "/Set/Data/LedStrip",
  "LED_STRIP_1": {
    "Active": true,
    "Segments": [

      {
        "StartLED": 0,
        "StopLED": 10,
        "Effect": 1,
        "Colors": [ { "R": 0, "G": 150, "B": 0 } ]
      },

] } }

{
"StartLED": #[LEDStart]#,
"StopLED": #[LEDEnd]#,
"Effect": 1,
"Colors": [ { "R": 0, "G": 150, "B": 0 } ]
},



![image](/assets/2025-03-09/010.png)
