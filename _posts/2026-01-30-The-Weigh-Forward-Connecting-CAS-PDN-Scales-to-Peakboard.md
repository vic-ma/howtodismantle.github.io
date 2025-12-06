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
CAS is a South Korea–based manufacturer of industrial and commercial weighing scales. In this week's article, we will discuss how to interact with those scales from our Peakboard application. Spoiler: It's easy! 

## Set up the CAS extension

The CAS extension is available for download directly in the designer. More information is available on the [Peakboard Extension Page](https://templates.peakboard.com/extensions/CAS/index) or in the technical documentation on [Github](https://github.com/Peakboard/PeakboardExtensions/tree/master/CAS) along with the source code.

Once installed, there are four different list types available. As of the beginning of 2026, there are two series of scales supported: PDN and PB2. Each of them supports two different types of ECR protocols. For the PDN series, we will use the ECR 14 protocol.
  
![CAS Extension Setup - PDN and PB2 Scale Types with ECR Protocols](/assets/2026-01-30/cas-extension-scale-setup.png)

When the scale is plugged into the USB port, it automatically emulates a virtual COM port. In that case, we need to look up the COM port in the Windows Device Manager settings. There might be other COM emulations or regular COM ports, so the COM port number differs from system to system. Of course, the regular non-virtual COM port can also be used as long as the Peakboard runtime runs on a machine with a physical port.

![Windows Device Manager - CAS USB Scale COM Port Detection](/assets/2026-01-30/windows-device-manager-cas-com-port.png)

The COM port is the only thing that needs to be carefully configured in the extension parameters, followed by a reload to ensure the metadata of the data source is set correctly. For this type of scale, the scale actively pushes the current weight to the COM port. That's why the data source doesn't have a reload interval. It's a push data source that triggers a reload event every time data is pushed.

![CAS Extension Parameters - COM Port Configuration in Peakboard](/assets/2026-01-30/cas-extension-com-port-settings.png)

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

