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

In this article, we'll discuss how to build an enterprise-wide Peakboard template that enforces corporate design requirements, and ensures that even non-design and non-marketing people can build dashboards and applications that comply with corporate requirements. These steps have turned out to be best practices for hundreds of customer projects.

## Colors

**Project Colors** offers a central place to store a set of colors used in the current application. Every new control relies on this color set. The background colors determine the overall look---whether it's a light or dark theme. The primary, highlight, and signal colors are typically taken from the corporate design requirements.

![image](/assets/2024-10-01/010.png)

## Fonts

Corporate design guidelines usually specify one or two fonts that should be used. All font-related definitions for different font styles (like header, button, and default) can be defined in the corresponding dialog.

But what if your company's font is not available by default in Peakboard? In that case, you have to add a custom font to the project, as a resource (TTF file). That way, the TTF file is deployed as part of the project to the Peakboard Box.

![image](/assets/2024-10-01/020.png)

## Building the Master Screen

To set one or more master screens, we first add the company's logo as a resource, and then build the actual template. Usually, a header with the logo, and a text box with the current time and a headline, is a good start.

After placing these items, we group them and then lock them by clicking on the lock icon. That way, they can't be changed accidentally.

When we want to create a new screen, we right click on the template screen and then choose **Duplicate (only locked controls)**. The new screen will only have the locked, template controls on it.

![image](/assets/2024-10-01/030.png)

## UI Components

The UI components are mainly defined through the default colors and default fonts, which we've already covered. For further customization, we can build UI control templates on a separate screen, as shown in the following screenshot.

Then, the user can pick a UI control from the template, and then copy and paste it into the screen they're working on. The copied control will have all the necessary styles, and can be used right away.

The template also provides a guide on proper spacing. It shows a spacing of 20px and 40px, and explains when to use each. Of course, you can add more explanatory text and hints if necessary.

![image](/assets/2024-10-01/040.png)

## Grid

An often-used pattern is to group controls into dedicated areas. The following screenshot shows how to prepare these grid areas. We can copy and paste the grid onto a new screen, adjust its height, and then use it right away.

This kind of grid makes the end result look more professional, and also gives the end user of the board a sense of which controls belong together, making the whole board much more understandable and easier to use.

![image](/assets/2024-10-01/050.png)

## Final Result

The following screenshot shows a screen that uses all the techniques we've discussed. We use predefined colors and fonts, along with the master template and its header. The UI controls are copied from the template screen and placed on a predefined grid.

![image](/assets/2024-10-01/060.png)
