---
layout: post
title: Table madness - How to modify and re-format table rows dynamically without any code
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

Table controls are the most commonly used controls to display table data. However we often see, that designers are not using their full potential to present the data in a way to perfectly support the end user to find what they are looking for as fast and as clear as possible. This article covers the best practice to modify a table row on the fly and adjust it to the content. In our sample we use a list of outbound delveries. Here's how we want to modify the data in the table:

1. When the column 'Priority' of an outbound delivery is set to 'High' the row is supposed to have a red back color.
2. When the weight of a delivery is more than 3 kgs we want to place a muscle bizeps icon next to value to advice the end user that this delivery is especially heavy.

## Preparing the data

This sample is based on a simple CSV data file. It's included in the project file and it shows a couple of outbound deliveries, including the recipient's name, weight of the delivery, priority etc.

![image](/assets/2023-12-01/010.png)

To make it easier to handle the data later, we add a dataflow to the source. As shown in the screenshot we adjust the data types of the columns 'Weight' and 'NumberOfPackages' to 'Number'. As it comes from a CSV file every column is treated as a 'string'. That's why we need to correct it.

![image](/assets/2023-12-01/020.png)

## Setting up the table control

The main part is the table control. We can add it by just drag and drop the data flow to the canvas and then choose a table control to show the output of the dataflow. To make it a bit nicer we adjust the column captions, column width and title of the control.
The most important thing is the event 'Datarow loaded'. It's triggered every time a row is painted to the convas during runtime. Let's assume our dataset has 10 rows, then the event is fired 10 times, once for each data row. All the manipulation can be done within this event. The manipulation can be applied to attributes like the cell color or even to the data values that are displayed. We learn more about both options.

![image](/assets/2023-12-01/030.png)

Within the edtior of the event we can find several blocks to be used in the context os the 'row loaded'. Let's assume we want to know the content of the 'Priority' columnof the data row that is currently painted, we just the block 'row value'. This sample script shows how to get the value and use it in a if statement to determine, if the priority of the delivery is high, and if so, set the background color of the current row to red.

![image](/assets/2023-12-01/040.png)

Here is a sample to manipulate the data content. In this case we check if the weight column content is greater than 3 (kg). If so we just add a bit of text to the table cell. Actaully it's not pure text, it's unicode character repredenting the flxed bizeps muscle: 💪.
Using these unicode characters in Peakboard is always monochrome. Please note: We are manipulating the cell content not the data content. Thats why we use the "Set table cell" block to paint our adjusted content.

![image](/assets/2023-12-01/050.png)

Here's the final result in preview with the red priority columns and the muscle icon:

![image](/assets/2023-12-01/060.png)

## Conclusion

This article shows how easy it is to manipulate the output of a table control and define the adjustments with only a few mouse clicks. However sometimes more complex adjustments are necessary and the options are limit within the table control. In this case it's better to use a "Styled list" control and have an indiviual design for each row.

