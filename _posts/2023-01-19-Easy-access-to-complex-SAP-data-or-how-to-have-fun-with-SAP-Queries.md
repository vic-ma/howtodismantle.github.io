---
layout: post
title: Easy access to complex SAP data or how to have fun with SAP Queries
date: 2023-01-19 12:00:00 +0200
tags: sap
image: /assets/2023-01-19/title.png
read_more_links:
  - name: SAP Query-Reporting at Amazon
    url: https://www.amazon.de/-/en/Stephan-Kaleske/dp/3836218402/ref=sr_1_1?crid=3EUFYNIDSOIH3&keywords=sap+query+reporting&qid=1690095229&sprefix=sap+query+reportin%2Caps%2C88&sr=8-1
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
---
SAP Queries can be seen as very early version of what we call Self Service Business Intelligence. In this article we cover some basic information about SAP Queries and how to use them in Peakboard.
One important information before we start: There's no installation necessary in SAP. You only need an RFC user to access the system with enough rights to execute a query. That's it!

## The 1 minute crash course

The query itself is created and maintained in transaction SQ01. The basis behind the Query can be a simple table, a table join or a so called logical database. A logical database can be seen as some kind of hierachical, predefined table join for a certain topic (e.g. Material Movements). Most queries depend on table joins. In transaction SQ01 you can select a query and just execute it right away in the SAP GUI. It's important to exactly understand what the query is doing and how the selection works before using it in Peakboard.

![image](/assets/2023-01-19/010.png)

When a query is executed, you can first see a Selection Screen similiar to a traditional SAP report. Fill in some useful selection values, then execute it and feel free to store your selection as a variant (we might need it later to make life easier on the Peakboard side).

![image](/assets/2023-01-19/020.png)

The resultset looks like a tradtional ALV Grid output with columns and rows.

![image](/assets/2023-01-19/030.png)

## Understanding the terms Infoset, Area and User Group

We need to understand some terms before we move one. As you already saw in the first screenshot, there are some containers to organiszre and structure queries.

![image](/assets/2023-01-19/040.png)

### User Groups

User Groups are some kind of container to organize queries and infosets. The term might be a bit misleading. We often find User Groups for certain topics like Material Management, Logistics, Planning.... but it depends on how the company is organising itself. There's no general rule. In our sample the User Group is called /SAPQUERY/MB. The SAPQUERY is just a namespace and MB stands for Material Management.

### Infoset

The idea of an Infoset is to separate the data aquisiation from how it's presented ot the user. In our example the Infoset is called /SAPQUERY/MEVL31 and it's just a table join on certain Puchase document tables. Our sample query MEVL31 is using this infoset to aquire the data. So when you see the selection screen or the output, you actually see the query. But what happens in the background is done by the infoset. Of course it makes sense that multiple queries share one infoset as they just present different views on the same infoset.

### Area

There are two areas in which Infosets, queries and user group are placed into. The Global Area is available for every user and can only be maintained in the SAP development system, and Local Area is more user specific and can be created directly in the production system.
Sometime the terms Cross Client, Client Specific are used rather then Global and Local. It's the same, just different words (nobody understands why SAP is naming these so inconsistently)

## Using SAP queries in Peakboard

SAP Queries can be used as part of the XQL syntax in a SAP datasource and follow this structure:

{% highlight SQL %}
SELECT * FROM QUERY 'A|B|C' USING 'V'
{% endhighlight %}

A is the Area (L for local, G for Global), B is the User Group, C the actual query name, and V is the name of the variant that you previuosly saved in SAP GUI.

Let's apply this pattern for our sample query:

{% highlight SQL %}
SELECT * FROM QUERY 'G|/SAPQUERY/MB/|MEVL31' 
USING 'PLANT1000'
{% endhighlight %}

Here's the screenshot with our sample data:

![image](/assets/2023-01-19/050.png)

It's very good idea to use variants for complex selections, because it makes the XQL statement super easy to build and understand. However in some situations it might be useful NOT to use a variant but fill the select option in the statement like this:

{% highlight SQL %}
SELECT * FROM QUERY 'A|B|C' WHERE CRIT1, CRIT2
{% endhighlight %}

The criteria CRIT refers to the name of the selection option. To find out this name, go to the selection screen, place you cursor in the field, hit F1 and the click on "technical information". The name of the select option is then shown in the dialog. Please note, that the "-LOW" suffix is not part of the name. So in that sample it would be just SP$00023.

![image](/assets/2023-01-19/060.png)

Now you can use the information to build the criteria. We can use the = sign or just the regular SAP terms for comparing. In our case it would be EQ for Equals. So to limit the plant to 1000 the XQL would be

{% highlight SQL %}
SELECT * FROM QUERY 'G|/SAPQUERY/MB/|MEVL31'
   WHERE
SP$00023 EQ '1000'
{% endhighlight %}

Let's do one more and add the criteria to addtionally limit the Material Number (select option SP$00018) to material numbers starting with 100. So we use MP for 'Matches Pattern' and an asterisk. Here we go:

{% highlight SQL %}
SELECT * FROM QUERY 'G|/SAPQUERY/MB/|MEVL31'
   WHERE
SP$00023 EQ '1000',
SP$00018 MP '100*'
{% endhighlight %}

![image](/assets/2023-01-19/070.png)

## Conclusion

Using SAP queries in Peakboard is very easy and convenient. Whenever we can use queries we should do it rather then use tables or reports. Please note, that the SAP query processor is not able to extract more than roughly 10000 rows. So it might not be suitable for every use case.




