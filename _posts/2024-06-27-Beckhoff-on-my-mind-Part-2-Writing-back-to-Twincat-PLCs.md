---
layout: post
title: Beckhoff on my mind - Part 2 - Writing back to TwinCAT PLCs
date: 2024-06-27 00:00:00 +0200
tags: hardware twincat
image: /assets/2024-06-27/title.png
read_more_links:
  - name: Part 1 - Connecting TwinCAT PLCs
    url: /Beckhoff-on-my-mind-Part-1-Connecting-Twincat-PLCs.html
  - name: Beckhoff Information System (the help site)
    url: https://infosys.beckhoff.com/
downloads:
  - name: BeckhoffTwincatCalculcator.pbmx
    url: /assets/2024-06-27/BeckhoffTwincatCalculcator.pbmx
---
Welcome to the second part of our Beckhoff TwinCAT series. In [Part 1 - Connecting TwinCAT PLCs](/Beckhoff-on-my-mind-Part-1-Connecting-Twincat-PLCs.html), we discussed the basics of connecting TwinCAT and Peakboard. In today's article, we will talk about writing back to TwinCAT. Compared to the relatively tricky router / connection task from the first part, writing back is fairly easy.

## The TwinCAT program

In this article, we will build a calculator. Here's how the calculator works:
1. The user enters two numbers into the Peakboard app.
2. Peakboard sends the numbers to a TwinCAT PLC.
3. The TwinCAT PLC sums up the two numbers and writes the result into a third variable.
4. The Peakboard app reads from the third variable to get back the result.

The following screenshot shows the TwinCAT program: three integer variables along with exactly one line of actual code to sum them up.

![image](/assets/2024-06-27/010.png)

## The Peakboard side

Let's switch to Peakboard Designer. We set up the data source and can easily access the three variables. Although we only need `iResult`, we select all of them for clarity and logging purposes.

![image](/assets/2024-06-27/020.png)

The following screenshot shows the canvas. We have three text controls: two for input and one for the result. The result text box is directly bound to the corresponding column in the data source. The table element below is only necessary for checking how the raw data behaves. It's not actually necessary for our example. All the magic happens behind the **Calculate** button.

![image](/assets/2024-06-27/030.png)

Now, let's look at the process that does the actual writing. We use a **Beckhoff Write variable** Building Block. It takes in three arguments:
* The connection.
* The destination variable name.
* The actual value, which is fed directly from the text box.

![image](/assets/2024-06-27/040.png)

If you prefer to use LUA, here's the same function written as raw LUA statements:

{% highlight lua %}
connections.getfromid('fgl8+PJgAZEtwwxF/W6yR7VW7aI=').writevariable('MAIN', 'iInput1', screens['Screen1'].txtInput1.text)
connections.getfromid('fgl8+PJgAZEtwwxF/W6yR7VW7aI=').writevariable('MAIN', 'iInput2', screens['Screen1'].txtInput2.text)
{% endhighlight %}

## Conclusion and result

The following GIF shows the app during runtime. The user input is sent to TwinCAT, calculated, and then presented the next time the data source is refreshed.

![image](/assets/2024-06-27/result.gif)

