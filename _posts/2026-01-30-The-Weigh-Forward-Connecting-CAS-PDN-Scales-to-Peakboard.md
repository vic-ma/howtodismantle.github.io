---
layout: post
title: The 'Weigh' Forward - Connecting CAS PDN Scales to Peakboard
date: 2023-03-01 00:00:00 +0000
tags: hardware
image: /assets/2026-01-30/title.jpg
image_header: /assets/2026-01-30/title.jpg
bg_alternative: true
read_more_links:
  - name: CAS PDN Scales
    url: https://cas-retail.com/pdn/
  - name: CAS PDN Manual
    url: https://www.cas-usa.com/amfile/file/download/file/390/
  - name: CAS Extension on Extension portal
    url: https://templates.peakboard.com/extensions/CAS/index
  - name: CAS Extension on Github
    url: https://github.com/Peakboard/PeakboardExtensions/tree/master/CAS
  - name: Driver for USB COM port
    url: https://ftdichip.com/drivers/vcp-drivers/
downloads:
  - name: CASPDNScales.pbmx
    url: /assets/2026-01-30/CASPDNScales.pbmx
  - name: scale_baking.pbmx
    url: /assets/2026-01-30/scale_baking.pbmx
---
[CAS](http://www.globalcas.com/) is a Korean company that makes industrial and commercial scales. In this article, we'll explain how to connect to those scales with Peakboard. (Spoiler alert: It's easy!)

## Install the CAS extension

Our first step is to install the [CAS extension](https://templates.peakboard.com/extensions/CAS/index). We do this directly from Peakboard Designer:

![CAS Extension Setup - PDN and PB2 Scale Types with ECR Protocols](/assets/2026-01-30/cas-extension-scale-setup.png)

To check out the documentation and source code, go to the [CAS extension GitHub](https://github.com/Peakboard/PeakboardExtensions/tree/master/CAS).

## Choose a CAS data source

Next, we create a CAS data source. As of early 2026, the CAS extension has four data sources to choose from---providing support for two series of scales (PDN and PB2) and two protocols for each series:
* PDN ECR Typ 12
* PDN ECR Typ 14
* PB2-Serial
* PB2-BLE (Bluetooth)

For our demo, we choose the PDN ECR Typ 14 data source.
  
![CAS Extension Setup - PDN and PB2 Scale Types with ECR Protocols](/assets/2026-01-30/cas-extension-scale-setup.png)

## Identify the scale's port number

When we plug our scale into a USB port, the scale automatically emulates a virtual COM port. In order for our data source to connect to the scale, we need to identify the scale's COM port number.

To do this, we open the Windows Device Manager and look for the virtual COM port that the scale created. There could be other virtual or physical COM ports, so the COM port number can differ from system to system. 

![Windows Device Manager - CAS USB Scale COM Port Detection](/assets/2026-01-30/windows-device-manager-cas-com-port.png)

The COM port is the only thing that needs to be carefully configured in the extension parameters, followed by a reload to ensure the metadata of the data source is set correctly. For this type of scale, the scale actively pushes the current weight to the COM port. That's why the data source doesn't have a reload interval. It's a push data source that triggers a reload event every time data is pushed.

![CAS Extension Parameters - COM Port Configuration in Peakboard](/assets/2026-01-30/cas-extension-com-port-settings.png)

Note: You can also connect the scale to a physical COM port, if the Peakboard app is running on a machine with a physical COM port.

## Different protocols ECR 12 and ECR 14

The data that is exchanged through COM communication is not standardized. That's why the PDN scale supports many different protocols depending mostly on which protocol is expected by the connected systems—typically the different POS manufacturers. The preferred protocol can be easily configured in the scale as described in the [manual](https://www.cas-usa.com/amfile/file/download/file/390/). In our example, we're using ECR 14, which is a very simple streaming of the weight value and nothing else. The more sophisticated ECR 12, which is also supported by the Extension, doesn't actively stream. When using ECR 12, the weight must be requested by calling the function `GetWeight`, which is NOT necessary with ECR 14. It also supports resetting the scale to zero by calling `SetZero`.

![CAS PDN Scale Protocol Selection - ECR 12 vs ECR 14 Configuration](/assets/2026-01-30/cas-scale-ecr-protocol-selection.png)

## Build the example

For our example we only need to bind the output of the data source to a text field and format the number correctly. That's all.

![Peakboard Weight Display - Data Binding Text Field from CAS Scale](/assets/2026-01-30/peakboard-weight-data-binding.png)

The video shows the Peakboard scale in action. The data transfer between the scale and the Peakboard application is happening literally without any delay.

{% include youtube.html id="DXPHLvzxVkM" %}

## Demo Use Case with Austrian dessert

In the last part of this article, we want to take a look at a ready-to-use template that can be downloaded [here](/assets/2026-01-30/scale_baking.pbmx). The idea is that the user can choose between different recipes. In this case, it's all about Austrian special dessert dishes.

![Peakboard Recipe Selection Interface - Austrian Dessert Baking Application](/assets/2026-01-30/peakboard-recipe-selection-interface.png)

When a recipe has been selected, the application shows the ingredients to add one after the other. The scale displays the current weight in real-time, and the user is supposed to add the indicated ingredient until the weight is correct. Then the user can confirm the step, and the scale is reset for the next ingredient.

![CAS Scale Real-Time Ingredient Weighing - Peakboard Recipe Instruction Display](/assets/2026-01-30/cas-scale-ingredient-weighing-recipe.png)

