---
layout: post
title: Use Conditional Formatting with icons for machine states - no script, no code
date: 2023-05-08 12:00:00 +0200
tags: essentials nocode
image: /assets/2023-05-08/040.png
---


We already saw in another [article](2023-05-01-Best-Practice-Store-machine-states-in-SQL-Server-and-build-data-historian.md) how to typically collect, store and monitor the state of an artefcat (machine, person, etc...) by using an SQL Server database. But of course the main goal is to give an end user a good and clear information about that state. So this article describes a very simple, yet best practice pattern on how to visualize a state without using any kind of script code, just by using the so called Conditional Formatting (CF).
Please keep in mind how the basic data looks like. We have one row per machine, and we have a column called State with either a RUN or a STOP to represent the current state.

![image](/assets/2023-05-08/010.png)

What we want to do now is to visualize these two states by using the icon control.
The initial stage of this icon control should be a gray circle (no data available) as shown in the screenshot. Please use the search bar, and the color picker to define the gray circle. 

![image](/assets/2023-05-08/020.png)

To switch this circle automatically to the actual state of the machine we will apply Conditional Formatting. The CF pane can be accessed by clicking on the CF button in the property grid.

![image](/assets/2023-05-08/030.png)

The basic idea behind CF is to apply rules. There can be added unlimited rules and every time the initial value is changing, Peakboard walks through all rules (starting from the top) and checks, if the rules is to be applied. There's a condition, e.g. "If the column 'State' at row 0 of the MyMachines data source equals RUN" you can set properties of the control where the CF is attached to. In our case we're setting the Icon property. In case of RUN, we set it to a green check mark. In case of STOP we set it to the red cross.

![image](/assets/2023-05-08/040.png)
![image](/assets/2023-05-08/041.png)

And that's it.... no code, no complicated formula.

Here's the icon in the preview board:

![image](/assets/2023-05-08/050.png)

