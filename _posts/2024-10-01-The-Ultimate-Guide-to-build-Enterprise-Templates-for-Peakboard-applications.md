---
layout: post
title: The Ultimate Guide to Building an Enterprise Template for Peakboard Applications
date: 2023-01-28 12:00:00 +0200
tags: administration bestpractice
image: /assets/2024-10-01/title.png
read_more_links:
  - name: How to manage and organize team access for Peakboard boxes in large organisations
    url: /How-to-manage-and-organise-team-access-for-Peakboard-boxes-in-large-organisations.html
downloads:
  - name: CorporateDesignTemplate.pbmx
    url: /assets/2024-10-01/CorporateDesignTemplate.pbmx
---
Building a Peakboard application is easy and straightforward. But as the scope increases, the team working on the application usually becomes more spread out, with people from different departments and geographical locations working on the same application. In this case, there are some [best practices and patterns for enterprise](/How-to-manage-and-organise-team-access-for-Peakboard-boxes-in-large-organisations.html) to consider. 

In this article, we'll discuss how to build an enterprise-wide template to meet all necessary requirements regarding Corporate Design and ensure even non-design or non-marketing people can build their dashboards and applications aligned with corporate regulations. The steps described here have turned out to be best practices through hundreds of customer projects.

## Colors

A Peakboard project offers a central place to store a set of colors used in the current application. Every new control relies on this color set. The background colors determine the overall look - whether it's a rather light or dark theme. The Primary, Highlight, and Signal colors are typically taken from the corporate design guidelines. 

![image](/assets/2024-10-01/010.png)

## Fonts

Every enterprise design guideline usually lists one or two fonts that are supposed to be used for any kind of correspondence. All font-related definitions for different font styles (like header, button, default, etc.) can be defined in the corresponding dialog. In the screenshot, it is shown that the corporate font is not a font that is available by default. It can be added to the project as a resource (ttf file). In that case, the ttf file is deployed as part of the project to the box.

![image](/assets/2024-10-01/020.png)

## Building the Master Screen

To set one or more master screens, we first add the company's logos as a resource and then build the actual main template. Usually, a header with the logo and a text box for the current time and a headline is a good start. After placing all these items, we first group them and then lock them by clicking on the lock icon. This way, they can't be changed accidentally. When we want to create a new screen, we can just right-click on the template screen and then choose “Duplicate (only locked controls)”. The new screen will only have the locked, template controls on it.

![image](/assets/2024-10-01/030.png)

## UI Components

The UI components are mainly defined through their default colors and default fonts that refer to what we already created. If there are more attributes necessary to take care of, we build UI control templates on a separate screen as shown in the screenshot. This way, the template user can just pick a predefined UI control, copy & paste it to their own screen, and it's ready to be used right away while following all prepared styles. We also take care of the spacing to show how to do proper spacing. The sample shows a spacing of 20px and 40px depending on whether we space controls of the same group or if there's a group change. Of course, we can add more explanatory text and hints if necessary.

![image](/assets/2024-10-01/040.png)

## Grid

An often-used pattern is to group controls into dedicated areas. The screenshot shows how to prepare these grid areas. We can just copy & paste the grid to a new screen and use it right away by adjusting the height of the prepared grid. These kinds of grids not only make the end result look more professional but also give the final viewer or user of the board a sense of which controls belong to each other and make the whole board much more understandable.

![image](/assets/2024-10-01/050.png)

## Final Result

The last screenshot shows a sample screen that used all the techniques we discussed earlier. We use predefined colors and fonts, along with the master template and its header. The UI controls are copied from the template screen and placed on a predefined grid.

![image](/assets/2024-10-01/060.png)
