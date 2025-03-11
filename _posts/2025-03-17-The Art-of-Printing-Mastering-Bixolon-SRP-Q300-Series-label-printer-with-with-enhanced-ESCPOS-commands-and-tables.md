---
layout: post
title: The Art of Printing - Mastering Bixolon SRP-Q300 Series receipt printer with with enhanced ESC/POS commands and tables
date: 2023-03-01 02:00:00 +0200
tags: hardware printing
image: /assets/2025-03-17/title.png
image_header: /assets/2025-03-17/title_landscape.png
read_more_links:
  - name: Printing with Peakboard
    url: /category/printing
  - name: Peakboard POS Printer extension
    url: https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter
  - name: Bixolon SRP-Q300 Series
    url: https://www.bixolon.com/product_view.php?idx=191
  - name: Bixolon User Manual
    url: https://www.bixolon.com/_upload/manual/Manual_User_SRP-Q300302_ENG_V2.00.pdf
downloads:
  - name: ESCPOSPrinter.pbmx
    url: /assets/2025-03-17/ESCPOSPrinter.pbmx
---
A couple of weeks ago we already introduced the topic about [how to print with Peakboard](/The-Art-of-Printing-Getting-started-with-label-printing-on-Seiko-SLP720RT.html). We learned how to use the POS printer extension and how to use the simplified, internal commands to build ESC/POS command set to be sent to the printer. In today's article we will deepen some new aspects of printing:

1. How to use the Bixolon SRP-Q300 printer
2. How to print tables
3. How to use pure ESC/POS command in case a command is not covered by the simplified command set

## Bixolon SRP-Q300

The Bixolon SRP-Q300 series we're using for today's example is a super light weight 80mm POS printer hat can be typically found in hospitality area - for any kind of receipts. That's why we will print a receipt of our Starpeak cofee shop in this example. 

The Bixolon printer doesn't come with a web interface but it can be easily configured through a configuration menu with the help of the function keys. How to do that can be checked out at the [user's manual from the Bixolon webiste](https://www.bixolon.com/_upload/manual/Manual_User_SRP-Q300302_ENG_V2.00.pdf). In the initial state it gets an ip address from the network's DHCP server and that's it. So in most situations no explicit configuration is necessary.

![image](/assets/2025-03-17/010.png)

## Printing a table

In the [first article about printing](https://how-to-dismantle-a-peakboard-box.com/The-Art-of-Printing-Getting-started-with-label-printing-on-Seiko-SLP720RT.html) we learned that there are several options of command sets to be used to send commands to POS printers. The easiest way is to use Peakboard's own markup language that is translated to ESC/POS commands before sent to the printer (see [here](https://github.com/Peakboard/PeakboardExtensions/tree/master/POSPrinter) for a full list of commands). The Peakboard's POS markup language is much easier to read than regular ESC/POS commands.

What we haven't see so far is how to print tables. When it comes to tables some kind of html like definition is used to embedd tables in the POS commands:

{% highlight text %}
~(PosTable:<CommaSeparatedColumnsWith>:<ActualHTMLStyleTable>)~
{% endhighlight %}

Let's take a look at concrete example with some LUA code to initiate the printing process. First we build the html-like table, then we call the "print" function of the data source based on the POS printer extension. The printer function takes two markup commands: One for the table with 3 columns and one for the feed and cut of the paper. That's it.

{% highlight lua %}
local table = [[
<tr>
    <th>Item</th>
    <th>Price</th>
    <th>Quantity</th>
</tr>
<tr>
    <td>Apple</td>
    <td>1.20 EUR</td>
    <td>10</td>
</tr>
<tr>
    <td>Banana</td>
    <td>0.99 EUR</td>
    <td>5</td>
</tr>
]]

local _ = data.MyPrinter.print("~(PosTable:20,14,14:" .. table .. ")~~(FullCutAfterFeed:2)~")
{% endhighlight %}

Here's how the result looks like:

![image](/assets/2025-03-17/020.png)

## Printing pure ESC/POS commands

Let's discuss a completely different approach. The pre-defined POS markup commands might not be sufficient for our printing project, or maybe we already have a ESC/POS command set for a complex layout that we want to use. In that case we can just submit pure ESC/POS commands within the POS markup commands like this:

{% highlight text %}
~(PureESCPOS: <MyPureCOmmands>)~
{% endhighlight %}

Here's the corresponding LUA example. First we build the ESC/POS commands by using plain text and hex commands. The hex commands for line feed and other formatting purpose is done throught constants. That improves readabaility. Then we wrap this string into the markup command discussed earlier and at last we shoot this string against the printer. That's it.

{% highlight lua %}
local ESC = "\x1B"
local LF = "\x0A" 
local INIT = ESC .. "@"
local BOLD_ON = ESC .. "E" .. "\x01"
local BOLD_OFF = ESC .. "E" .. "\x00"
local DOUBLE_SIZE = ESC .. "!" .. "\x38"
local NORMAL_SIZE = ESC .. "!" .. "\x00"
local ALIGN_CENTER = ESC .. "a" .. "\x01"
local ALIGN_LEFT = ESC .. "a" .. "\x00"
local CUT = ESC .. "i"
local FEED_5 = ESC .. "d" .. "\x05"

local receipt = INIT ..
    DOUBLE_SIZE .. "STARPEAK COFFEE" .. LF .. NORMAL_SIZE ..
    "123 Main Street, Los Angeles, CA" .. LF ..
    "Tel: (123) 456-7890" .. LF ..
    "--------------------------------" .. LF ..
    "Date: 02/17/2025  14:30 PM" .. LF ..
    "Order: 00123" .. LF ..
    "--------------------------------" .. LF ..
    BOLD_ON .. "Item           Qty   Price" .. LF .. BOLD_OFF ..
    "Latte         x1   $4.50" .. LF ..
    "Cappuccino    x1   $4.75" .. LF ..
    "Blueberry Muffin x1  $3.25" .. LF ..
    "--------------------------------" .. LF ..
    BOLD_ON .. "Subtotal:           $12.50" .. LF ..
    "Tax (8%):          $1.00" .. LF ..
    "Total:             $13.50" .. LF .. BOLD_OFF ..
    "--------------------------------" .. LF ..
    "Payment: Credit Card" .. LF ..
    "Card #: **** **** **** 1234" .. LF ..
    "Approval: 456789" .. LF ..
    "--------------------------------" .. LF ..
    ALIGN_CENTER ..
    "Thank you for visiting Starpeak!" .. LF ..
    "www.peakboard.com" .. LF ..
    ALIGN_LEFT ..
    FEED_5 ..
    CUT

local _ = data.MyPrinter.print("~(PureESCPOS: " .. receipt .. ")~")
{% endhighlight %}

Here's the result on paper which already comes pretty close to a professional lable.

![image](/assets/2025-03-17/030.png)

## result and conclusion

Beside getting to know the beautiful Bixolon printer we learned how to print tables and how to use pure ESC/POS commands. We must carefully consider pure ESC/POS commands (hard to build and might get complicated fast) versus built-in markup language (easy to use but potentially limited).

{% include youtube.html id="JxS4E6D1dJw" %}
