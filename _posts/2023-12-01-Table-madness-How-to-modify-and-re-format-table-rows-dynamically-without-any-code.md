---
layout: post
title: Table madness - How to modify and reformat table rows dynamically without any code
date: 2023-03-01 12:00:00 +0200
tags: bestpractice
image: /assets/2023-12-01/title.png
read_more_links:
  - name: Dismantle Number Value Formatting
    url: /Dismantle-Number-Value-Formatting.html
downloads:
  - name: TableRowFormatting.pbmx
    url: /assets/2023-12-01/TableRowFormatting.pbmx
---

Table controls are the most commonly used controls for displaying table data. However, we often see that designers do not use table controls to their full potential. Table controls can present data in a way that lets the end user find what they are looking for as quickly as possible. This article covers the best practices for modifying a table row on the fly and adjusting it to the content. In our example, we use a list of outbound deliveries. Here's how we want to modify the data in the table:

1. When the `Priority` column of an outbound delivery is set to `High`, the row should have a red background color.

2. When the weight of a delivery is more than 3 kg, we want to place a biceps icon next to value, to advise the end user that this delivery is especially heavy.

## Preparing the data

This example is based on a simple CSV data file. It's included in the project file, and it shows a couple of outbound deliveries, including the recipient's name, weight of the delivery, priority etc.

![image](/assets/2023-12-01/010.png)

To make it easier to handle the data later, we add a dataflow to the data source. As seen in the screenshot, we change the data type for the `Weight` and `NumberOfPackages` columns to `Number`. We have to do this manually, because the data comes from a CSV file, and every column of a CSV file is treated as a string by default.

![image](/assets/2023-12-01/020.png)

## Setting up the table control

To add our table control, we drag and drop the data flow onto the canvas, and then choose a table control. To make it a bit nicer, we adjust the column labels, column width, and title of the control.

The most important part is the *Datarow loaded event*. It's triggered every time a row is painted onto the canvas during runtime. Let's assume our dataset has 10 rows. Then, the event is fired 10 times, once for each data row.

All the manipulation can be done within this event. The manipulation can change attributes like the cell color or even to the data values that are displayed. Let's learn more about these manipulations.

![image](/assets/2023-12-01/030.png)

Within the event editor, there are several blocks we can use for the *Datarow loaded event*. Let's assume we want to know the content of the `Priority` column of the data row that is currently being painted. Then, we just use the block *Row value*.

This script shows how you can get the value of a column and use it in an if statement. It checks if the priority of the delivery is high, and if so, it changes the background color to red.

![image](/assets/2023-12-01/040.png)

Here is a sample to manipulate the data content. In this case we check if the weight column content is greater than 3 (kg). If so we just add a bit of text to the table cell. Actaully it's not pure text, it's unicode character repredenting the flxed bizeps muscle: 💪.
Using these unicode characters in Peakboard is always monochrome. Please note: We are manipulating the cell content not the data content. Thats why we use the "Set table cell" block to paint our adjusted content.

![image](/assets/2023-12-01/050.png)

Here's the final result in preview with the red priority columns and the muscle icon:

![image](/assets/2023-12-01/060.png)

## Conclusion

This article shows how easy it is to manipulate the output of a table control and define the adjustments with only a few mouse clicks. However sometimes more complex adjustments are necessary and the options are limit within the table control. In this case it's better to use a "Styled list" control and have an indiviual design for each row.