---
layout: post
title: From UI to PNG - The Technical Guide to Peakboard Screenshot Automation
date: 2023-03-01 00:00:00 +0000
tags: peakboardhub image
image: /assets/2026-03-11/title.png
image_header: /assets/2026-03-11/title.png
bg_alternative: true
read_more_links:
  - name: More image processing
    url: /category/image
downloads:
  - name: xScreenshotDemoxx.pbmx
    url: /assets/2026-03-11/ScreenshotDemo.pbmx
---
Peakboard is designed for applications to happen completely on one physical screen, prefrebly a touch screen. That's how a typical Peakboard backed industrial application is supposed to work. However it might be necessary sometimes to conserve or send the screen or parts of that outside of the current screen. Typical use cases would be to inform someone else by email or just store the current status of a chart and get back to it the next day (a typical shopfloor management process). In this article we will discuss how to generate a screenshot from our current visualisation and do something with - store it, send it, push it to an API endpoint - whatever the 