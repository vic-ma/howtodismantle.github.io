---
layout: post
title: Matryoshka Dolls - Surviving SAP's Multi-Nested Parameters Maze
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-05-02/title.png
read_more_links:
  - name: More fancy SAP articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/sap
  - name: SAP on fire - how to perfectly integrate LUA scripting with SAP
    url: /SAP-on-fire-how-to-perfectly-integrate-LUA-scripting-with-SAP.html
  - name: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
    url: /Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html
downloads:
  - name: SAPMultinestedTables.pbmx
    url: /assets/2024-05-02/SAPMultinestedTables.pbmx
---
Back in the 90s, calling SAP RFC function modules was pretty straight forward. There were import, export, and table parameters. The imports and exports could either be scalar or structured---and that was the end of the story.

Over the years, more complex parameters where pressed into this structure, and "Changings" became supported by modern SAP systems.

In today's article, we'll cover multi-nested tables and how to handle them in XQL. This is one of the XQL features that is supported from Peakboard version 3.8 or later.
 
## The RFC function module

For this article, we'll have a look at a sample RFC with an import parameter called `I_SELOPT`. This parameter is a table-like import parameter. But not just a simple table, but it's a mutli-nested table. Which means, that every row can have a cell that itself is another table. The most common case for this are tables for dynamic data selection. Every row refers to a certain field and the table per row refers to a set of Select Options. this makes it possible, to provide a complete generic selection without the need for changing the import parameters when an additional filter field is necessary.

![image](/assets/2024-05-02/010.png)

Let's drill deeper into the associated type ZTABRANGE. It points to the structure ZSINGLERANGE.

![image](/assets/2024-05-02/020.png)

Within the line type ZSINGLERANGE we find the scalar field name called FIELDNAME and also the table for the generic filter called SELOP.

![image](/assets/2024-05-02/030.png)

The table type ZTRSDSSELOPT in turn refers to the line type RSDSSELOPT.

![image](/assets/2024-05-02/040.png)

The standard line type RSDSSELOPT contains 4 single values to represent a filter line:
- SIGN - I for include, E for exclude
- OPTION - An operator (e.g. EQ for equals, or BT for between)
- LOW - the lower value
- HIGH - the optional upper value

![image](/assets/2024-05-02/050.png)

## How to build the XQL

We start with a very simple version and build a non-multinested XQL table to call the function module. The table we build has just one column called fieldname and two rows with values MATNR and WERKS.

{% highlight sql %}
EXECUTE FUNCTION 'Z_PB_MAT_SELECTION'
   EXPORTS
      I_SELOPTS = ((FIELDNAME),
         ('MATNR'),
         ('WERKS'))
   CHANGING
      C_MATTAB INTO @RETVAL
{% endhighlight %}

Now we need to add one more table to each selection row. The column name for the set of tables is called SELOP. And each SELOP cell is in turn a table with the four columns mentioned earlier. In our sample the first SELOP table has exactly one row, it contains a Select Option row for the Material number '100-100'. The second SELOP row has two filter rows, in the first filter we have the criteria 'EQUALS 1000' and the second filter row is 'BETWEEN 2000 and 3000'.
We note, that we have to be very careful with the brackets and it's highly recommended to use the notation as shown in the sample with the correct line breaks and indents. 

{% highlight sql %}
EXECUTE FUNCTION 'Z_PB_MAT_SELECTION'
   EXPORTS
      I_SELOPTS = ((FIELDNAME, SELOP),
         ('MATNR', ((SIGN,OPTION,LOW,HIGH), 
                    ('I','EQ','100-100', '')) ),
         ('WERKS', ((SIGN,OPTION,LOW,HIGH), 
                    ('I','EQ','1000', ''), 
                    ('I','BT','2000', '3000')) 
                  ))
   CHANGING
      C_MATTAB INTO @RETVAL
{% endhighlight %}


