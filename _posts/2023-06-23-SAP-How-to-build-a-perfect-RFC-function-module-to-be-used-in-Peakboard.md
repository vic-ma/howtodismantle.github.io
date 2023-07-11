---
layout: post
title: How to build a perfect RFC function module to use in Peakboard
date: 2023-06-23 12:00:00 +0200
tags: sap
image: /assets/2023-06-23/title.jpg
---
Within the available Peakboard connectors, there are multiple options for accessing different SAP objects: reports, MDX, BAPI/function modules, tables, queries, etc. The right one to use often depends on what is already there and what the customer is able to build on their own. For example: When you already have a report in place that delivers the data you're looking for, you should take the report. If you can easily get the data from one or two tables which are not too large and long, take them. 

However, the question is: If you are totally free to choose your weapon, how would you build the perfect SAP interface? And the answer is: The ideal interface is a remote-enabled function module (RFC). And this RFC should follow certain rules to make life as easy as possible. These rules are described in this article.

## The SAP development part

In our example, we will build a delivery monitor. So for a certain shipping point (for the Germans: *Versandstelle*), we want to list some details about the deliveries that are waiting to be processed at this shipping point. The RFC we will build is called `Z_PB_DELIVERY_MONITOR`. Make sure it's remote-enabled.

![image](/assets/2023-06-23/010.png)

In the `Import` section, we list all the parameters that might vary. Let's assume we want to reuse the function module for different shipping points. So, we add a parameter for the shipping point. (If you want to be a real, professional SAP developer, please use naming conventions like the prefix `I_` to indicate it's an import parameter.)

![image](/assets/2023-06-23/020.png)

Let's go to the tables section. Here, we have exactly one table that is returned by the RFC. 

![image](/assets/2023-06-23/030.png)

The table structure should look exactly like the result set of the data to be returned. Therefore, first create a structure in transaction `SE11` that exactly reflects the signature of the data that should be returned later.

![image](/assets/2023-06-23/040.png)

## The source code

Let's have a look at the source code.

![image](/assets/2023-06-23/050.png)

Here's the complete source code. As you can see, the code actually only consists of a single call, which is a database query that puts the data right into the return table. In the real world, the code might be a little more complicated, but this example demonstrates some useful concepts, as the data is already joined from different tables, filtered, and aggregated. All in one call. 

{% highlight abap %}
FUNCTION z_pb_delivery_monitor.
*"----------------------------------------------------------------------
*"*"Local Interface:
*"  IMPORTING
*"     VALUE(I_VSTEL) LIKE  LIKP-VSTEL DEFAULT 1000
*"  TABLES
*"      T_DELIVERIES STRUCTURE  ZPBDELIVERYMONITOR
*"----------------------------------------------------------------------

  SELECT
      likp~vbeln
      likp~erdat
      likp~vstel
      tvstt~vtext AS vtext
      likp~anzpk
      likp~kunnr
      likp~btgew
      likp~gewei
      kna1~name1
      kna1~pstlz
      kna1~ort01
      COUNT(*) AS items
    INTO CORRESPONDING FIELDS OF TABLE
      t_deliveries
    FROM likp INNER JOIN lips
      ON likp~vbeln = lips~vbeln
      INNER JOIN kna1
      ON likp~kunnr = kna1~kunnr
      INNER JOIN tvstt
      ON likp~vstel = tvstt~vstel
    WHERE likp~vstel = i_vstel
     AND tvstt~spras = sy-langu
    GROUP BY
      likp~vbeln
      likp~erdat
      likp~vstel
      likp~anzpk
      likp~kunnr
      likp~btgew
      likp~gewei
      tvstt~vtext
      kna1~name1
      kna1~pstlz
      kna1~ort01
  .
ENDFUNCTION.
{% endhighlight %}

## The Peakboard part

When the RFC is so perfectly prepared, the integration into Peakboard is a breeze. The XQL code just calls the RFC by submitting the shipping point and processing the return table. That's it! Now, you can use this perfect table right away in your application logic.

![image](/assets/2023-06-23/060.png)

## Conclusion

When you need to decide which of the SAP interfaces to choose, please keep this example in mind. It's considered to be perfect because all the calculations are done in SAP, with a single call. There's absolutely no data waste downloaded into Peakboard, nor is the data processing in Peakboard unreasonably complicated.
