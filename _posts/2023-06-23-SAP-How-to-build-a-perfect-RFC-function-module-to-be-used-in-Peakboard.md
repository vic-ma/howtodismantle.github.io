---
layout: post
title: How to build a perfect RFC function module to be used in Peakboard
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2023-06-23/title.jpg
---
Within the available Peakboard connectors there are multiple options to access different SAP objects: Reports, MDX, BAPI / function modules, tables, queries, etc.
The right decision which of these to use often depends on what is already there and what the customer is able to build on his own. For example: When you already have a report in place that basically delivers the data you're looking for, you would take this. If you can easily get the data from one or two tables which are not too large and long, take them. However the question is: If you're totally free in choosing your weapons, how would you build the technically most perfect SAP interface? And the answer is: The technically most ideal interface is a remote enabled function module (RFC). And this RFC should follow certain rules to make life as easy as posible. These rules are described in this article.

## The SAP development part

In our sample we will build a delivery monitor. So for a certain Shipping Point (for the Germans: Versandstelle) we want to list a couple of details of the deliveries, that are waiting to be processed at this shipping point. The RFC we're building is called Z_PB_DELIVERY_MONITOR. Make sure it's remote enabled.

![image](/assets/2023-06-23/010.png)

In the _import_ section we list all parameters, that might be varying. Let's assume we want to reuse the function module for different shipping points, so we add a parameter for the shipping point (If you want to be a real professional SAP developer please use naming conventions like the prefix "I_" to indicate it's an import parameter).

![image](/assets/2023-06-23/020.png)

Let's go to the tables section. Here we have exactly one table that is returned by the RFC. 

![image](/assets/2023-06-23/030.png)

The table structure is supposed to look exactly like you want the resultset of the data to be returned. Therefor create a structure in transaction SE11 first, that exactly reflects the signature of the data that is needed to be returned later.

![image](/assets/2023-06-23/040.png)

## The source code

Let's have a look at the source code.

![image](/assets/2023-06-23/050.png)

Here's the complete source code. As you see, the code actually only consists of one single call which is a database query that puts the data right into the return table. In real life, the code might be a little more complicated but this sample already reflects some nice concepts, as the data is already joined from diffeent tables, filtered and aggregated. All in one call. 

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

When the RFC is so perfectly prepared the integration in Peakboard is quite straight forward. The XQL code just calls the RFC by submitting the shipping point and processes the return table. That's it! Now you can use this perfect table right away in your application logic.

![image](/assets/2023-06-23/060.png)

## Conclusion

When you need to decide which of the SAP interfaces to choose, please keep this sample mind. It's considered to be perfect because all the calculation is done within SAP, within one single call. There's absolutely no data waste downloaded to Peakboard, nor is the data processesing in Peakboard unreasonable complicated.



