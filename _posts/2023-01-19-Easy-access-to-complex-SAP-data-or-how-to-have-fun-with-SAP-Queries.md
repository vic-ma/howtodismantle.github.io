---
layout: post
title: Easy access to complex SAP data: How to have fun with SAP Queries
date: 2023-01-19 12:00:00 +0200
tags: sap
image: /assets/2023-01-19/title.png
read_more_links:
  - name: SAP Query-Reporting at Amazon
    url: https://www.amazon.de/-/en/Stephan-Kaleske/dp/3836218402/ref=sr_1_1?crid=3EUFYNIDSOIH3&keywords=sap+query+reporting&qid=1690095229&sprefix=sap+query+reportin%2Caps%2C88&sr=8-1
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
---
SAP Queries can be seen as a very early version of what we call self-service business intelligence. In this article, we cover some basic information about SAP Queries and how to use them in Peakboard.

One important thing before we start: There's no installation necessary in SAP. You only need an RFC user with enough permissions to access the system and execute a query. That's it!

## The 1 minute crash course

The query itself is created and maintained in transaction `SQ01`. The basis behind the Query can be a simple table, a table join, or a logical database.

A logical database is a kind of hierarchical, predefined table join for a certain topic (e.g. material movements). Most queries depend on table joins. In transaction `SQ01`, you can select a query and execute it right away in the SAP GUI. It's important to understand exactly what the query is doing and how the selection process works before using it in Peakboard.

![image](/assets/2023-01-19/010.png)

When a query is executed, you can first see a selection screen similar to a traditional SAP report. Fill in some useful selection values, then execute it. Feel free to store your selection as a variant (we might need it later to make life easier on the Peakboard side).

![image](/assets/2023-01-19/020.png)

The result set looks like a traditional ALV Grid output with columns and rows.

![image](/assets/2023-01-19/030.png)

## Understanding the terms "InfoSet," "area," and "user group"

We need to understand some terms before we move on. As you saw in the first screenshot, there are some containers for organizing and structuring queries.

![image](/assets/2023-01-19/040.png)

### User groups

User groups are a kind of container used to organize queries and InfoSets. The term is a bit misleading. We often find user groups for things like material management, logistics, planning, etc., but it depends on how the company organizes itself. There is no general rule. In our example, the user group is called `/SAPQUERY/MB`. The `SAPQUERY` is just a namespace, and the `MB` stands for material management.

### InfoSet

The goal of an InfoSet is to separate data acquisition from data visualization. In our example, the InfoSet is called `/SAPQUERY/MEVL31` and it's just a table join on certain purchase document tables. Our sample query, `MEVL31`, uses this InfoSet to acquire the data. So when you see the selection screen or the output, you actually see the query. But what happens in the background is done by the InfoSet . Of course, it makes sense that multiple queries share one InfoSet, as they just present different views of the same InfoSet .

### Area

There are two areas into which InfoSets, queries, and user groups are placed. The global area is available for every user and can only be maintained in the SAP development system. The local area is more user specific and can be created directly in the production system.
Sometimes, the terms "cross client" and "client specific" are used rather then "global" and "local." They mean the same thing (nobody understands why SAP's naming scheme is so inconsistent).

## Using SAP Queries in Peakboard

In Peakboard, SAP Queries can be used as part of the XQL syntax in a SAP datasource. They follow this structure:

{% highlight SQL %}
SELECT * FROM QUERY 'A|B|C' USING 'V'
{% endhighlight %}

`A` is the area (`L` for local, `G` for global), `B` is the user group, `C` is the actual query name, and `V` is the name of the variant that you previously saved in SAP GUI.

Let's apply this pattern for our sample query:

{% highlight SQL %}
SELECT * FROM QUERY 'G|/SAPQUERY/MB/|MEVL31' 
USING 'PLANT1000'
{% endhighlight %}

Here's the screenshot with our sample data:

![image](/assets/2023-01-19/050.png)

It's a very good idea to use variants for complex selections, because it makes the XQL statement super easy to build and understand. However, in some situations, it may be useful to NOT use a variant, but rather fill the select option in the statement like this:

{% highlight SQL %}
SELECT * FROM QUERY 'A|B|C' WHERE CRIT1, CRIT2
{% endhighlight %}

The criteria `CRIT` refers to the name of the selection option. To find this name, go to the selection screen, place you cursor in the field, hit `F1`, and then click on `Technical Information`. The name of the selection option is then shown in the dialog. Please note that the `-LOW` suffix is not part of the name. So in this example, the name would just be `SP$00023`.

![image](/assets/2023-01-19/060.png)

Now you can use the information to build the criteria. For comparing, we can use `=` or just the regular SAP terms. In our case, it would be `EQ` for "equals." So to limit the plant to 1000, the XQL would be:

{% highlight SQL %}
SELECT * FROM QUERY 'G|/SAPQUERY/MB/|MEVL31'
   WHERE
SP$00023 EQ '1000'
{% endhighlight %}

Let's do one more example. We'll have the criteria additionally limit the material number (select option `SP$00018`) to material numbers starting with 100. We use `MP` for "matches pattern," and an asterisk. Here we go:

{% highlight SQL %}
SELECT * FROM QUERY 'G|/SAPQUERY/MB/|MEVL31'
   WHERE
SP$00023 EQ '1000',
SP$00018 MP '100*'
{% endhighlight %}

![image](/assets/2023-01-19/070.png)

## Conclusion

Using SAP Queries in Peakboard is easy and convenient. Whenever we can use Queries, we should use them, rather than resorting to tables or reports. Please note that the SAP Query processor is not able to extract more than roughly 10000 rows. So it may not be suitable for every use case.
