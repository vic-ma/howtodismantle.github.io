---
layout: post
title: Best Practice - Use Power BI for integrating maps
date: 2023-04-24 12:00:00 +0200
tags: tutorial bestpractice
image: /assets/2023-04-24/title.png
read_more_links:
  - name: Microsoft Power BI Playground for Newbies
    url: https://playground.powerbi.com/en-us/
- name: Microsoft Documentation  - Embed a report
    url: https://learn.microsoft.com/en-us/javascript/api/overview/powerbi/embed-report
  - name: Microsoft Documentation  - Embed a dashboard tile
    url: https://learn.microsoft.com/en-us/javascript/api/overview/powerbi/embed-dashboard-tile
downloads:
  - name: PowerBIMapIntegration.pbmx
    url: /assets/2023-04-24/PowerBIMapIntegration.pbmx
---
A lot of people use Peakboard as medium to intgrate Power BI dashboards along with other data and create a dashboard that mashes up both non-Power-BI with Power-BI visuals and data. In today's article we will have a look on how to do that by building a Power BI report with certain elemens, especially a map and later bring exactly this map to be part of Peakbaord canvas.

### Building the Power BI report

Our sample report contains a table of Asian's 10 largest economies along with their GPD. Beside the table the data is shown on a map. Each bubble represents one country. Feel free to download the pbix [here](/assets/2023-04-24/GDPPerCountry.pbix).

![image](/assets/2023-04-24/010.png)

Go ahead and upload this report to a Power BI workspace. Along with this create a new, empty dashboard there.

![image](/assets/2023-04-24/020.png)

Open the report from the workspace overview and click on the pin, that can be found in the upper right corner of the map visual. Then pin the visual to the dashboard. That's it! We only need the visual to be pinned somewhere on a dashboard in order to pick out this particular visual later in Peakboard. In that case it's called a _Tile_ in the langauge of Power BI.

![image](/assets/2023-04-24/025.png)

### Requirements for embedding Power BI artifacts

To be able to embedd Power BI report and tiles in external apps, you first need a so called registered App in your Azure AD. How to do this, please check the [Peakboard help](https://help.peakboard.com/controls/Extended/en-power-bi.html). Every step is explained there, especially how to get the client ID and the tenant ID which we will need later.
Beside this you will need to provide user credentials for a user, that has access to the workspace we created earlier.
It's very hard to understand, that you need a real user beside the app registration. Actually this doesn't make sense, but that's how MS designed it. So please complain with them. The next annoying thing is, that this process doesn't support 2FA. So in case you have a 2FA policy in place, please deactivate it for this dedicated user and restrict his rights as much as possible to maintain an acceptable level of security.

### Building the board

Let start with the Peakboard side now by dragging a new Power BI control on the canvas. The control is asking you for the IDs you have collected during the setup process of the app registration plus a user with enough rights to access the workspace. Then log in...

![image](/assets/2023-04-24/030.png)

On the next pane, you can select the name of the shared workspace. If the user who created the workspace is the same as the user here, it might be a _private_ instead of a _shared_ workspace. Peakboard is supporting to embedd two kind of objects: Reports or dashboard tiles. In real life most people prefer to just embed a single dashboard's tile rather than a whole report, because it's clean and simple. But the decision depends on the use case. We're sticking to our sample dashboard as shown in the screenshot.

![image](/assets/2023-04-24/040.png)

And now we're all set to enjoy our dashboard tile as part of the Peakboard canvas.

![image](/assets/2023-04-24/050.png)

### Trouble shooting and tips

- Be very careful with the API permission in the app registration. The permissions are listed in the help file, however might vary for some use cases. It might be necssary to add some more.
- Be very careful with the user and the password. Especially when switching off 2FA. Restrict the user's right as tight as possible.
- In most cases dashboard tiles are the better choice instead of a whole report. If in doubt, use the tiles.
