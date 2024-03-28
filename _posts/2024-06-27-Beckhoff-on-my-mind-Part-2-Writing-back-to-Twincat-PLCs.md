---
layout: post
title: Beckhoff on my mind - Part 2 - Writing back to Twincat PLCs
date: 2023-03-01 12:00:00 +0200
tags: hardware twincat
image: /assets/2024-06-27/title.png
read_more_links:
  - name: Part 1 - Connecting Twincat PLCs
    url: /Beckhoff-on-my-mind-Part-1-Connecting-Twincat-PLCs.html
  - name: Beckhoff Information System (the help site)
    url: https://infosys.beckhoff.com/
downloads:
  - name: BeckhoffTwincatCalculcator.pbmx
    url: /assets/2024-06-27/BeckhoffTwincatCalculcator.pbmx
---
Welcome to the second part of our Beckhoff Twincat series. In [Part 1](/Beckhoff-on-my-mind-Part-1-Connecting-Twincat-PLCs.html) we discussed the basics of connecting Peakboard and Twincat. In today's article we move one step ahead and talk about writing back to Twincat. Compared with the relatively tricky router / connection thing from the first part, writing back is fairly easy.

## The Twincat program

The idea of this showcase is to build a calculcator. We let the user of the Peakboard app provide two numbers, send it to a Twincat PLC and let it sum up these numbers. The result is written into a third variable from where we can read it to get back the result. The screenshot shows the Twincat program. Three integer variables along with exactly one line of actual code to sum them up.

![image](/assets/2024-06-27/010.png)

## The Peakboard side

Let's switch to the Peakboard designer. We set up the source and can easily access the three variables. Although we only need the iResult we select all of them for clarity and logging purpose.

![image](/assets/2024-06-27/020.png)

The next screenshot shows the canvas. We have three text controls, two for input and one for the result. The result text box is directly bound to the corresponding column in the data souce.
The table element below is only for checking how the raw data behaves. It's actually not necessary for this show case.
All the magic is happening behind the "Calculate" button

![image](/assets/2024-06-27/030.png)

Now let's look at the process that does the actual writing. As we see in the screenshot, there's a Building Block available which can be used. It needs to know the connection, the destination variable name and the actual value, which is fed directly from the text box.

![image](/assets/2024-06-27/040.png)

In case we prefer to type the code directly in LUA, here's the same function written as raw LUA statements:

{% highlight lua %}
connections.getfromid('fgl8+PJgAZEtwwxF/W6yR7VW7aI=').writevariable('MAIN', 'iInput1', screens['Screen1'].txtInput1.text)
connections.getfromid('fgl8+PJgAZEtwwxF/W6yR7VW7aI=').writevariable('MAIN', 'iInput2', screens['Screen1'].txtInput2.text)
{% endhighlight %}

## conclusion and result

The gif below shows the app during runtime. The user input is send to Twincat, calculated and then presented the next time the data source is refreshed.

![image](/assets/2024-06-27/result.gif)

