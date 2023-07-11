---
layout: post
title: Accessing a new dimension of SAP BW with MDX
date: 2022-01-10 12:00:00 +0200
tags: sap
image: /assets/2023-01-10/title.png
read_more_links:
  - name: Wikipedia about MDX
    url: https://en.wikipedia.org/wiki/MultiDimensional_eXpressions
  - name: MDX at SAP
    url: https://help.sap.com/doc/saphelp_nw75/7.5.5/de-DE/14/a3639e028a144d8c8b7dd403b22a1a/frameset.htm
downloads:
  - name: SAPMDX.pbmx
    url: /assets/2023-01-10/SAPMDX.pbmx
---
SAP Bussines Warehouse (BW) can be seen as some kind of module within the SAP product portfolio that is used for analysis purpose. Usually the modules of the production environment (e.g. FI for financials or MM for material management) are sending their data to BW. In BW the data is processed to and in special reporting objects like Info Objects, Cubes, DSOs etc.... A the end of this chain, there are visualisation tools like Business Objects (BO) or just Excel to access these BW objects.

SAP is not the only software company providing solutions for data analysis, there are many more. That's why this industry has developed some kind of standardized way to access objects in the data analysis system. This standard is called MDX (Multidimensional Expressions). It's some kind of language to precisely define which and how the data is queried. When we want to access the data in a SAP BW we will use exactly this MDX langauge to do so.

MDX needs some time to be fully understood. So please feel free to google additional material on the web to learn more. There are tons of articles and videos for self-learning. In this article we will only cover a minimal crash course to understand some basics. This wont be enough to cover the knowledge you need to build queries in real life.

## The 1 minute crash course

If you have access to a SAP BW text system the best thing to go ahead with learning MDX is the transaction MDXTEST. Here you type in your MDX statements, execute them right away and get help for metadata (dimensions) of the sube you want to query. The picture shows MDXTEST with the cube 0D_DECU (on the pane). It's a sales demo cube provided by SAP and it is ideal for playing around.

![image](/assets/2023-01-10/010.png)

The basic structure of an MDX statement is similar to a SQL statement. Here is the general format:

{% highlight mdx %}
SELECT 
    { [Measure].[MeasureName] } ON COLUMNS,
    { ([Dimension].[Hierarchy].[Member] ) } ON ROWS
FROM [Cube]
WHERE ( [Dimension].[Hierarchy].[Member] )
{% endhighlight %}

Let's break this down:

SELECT: Just like in SQL, this keyword specifies what you want to see in the query result. This is followed by the ON COLUMNS keyword that places these results on the column axis of the result set.

ON COLUMNS: This clause specifies the data to be put on the columns of the result set, which is typically the measures you are interested in.

ON ROWS: This clause specifies the data to be put on the rows of the result set. This is often some hierarchy of members within a dimension.

FROM [Cube]: This specifies the cube from which you are querying the data.

WHERE: This is used to slice the cube by one or more dimensions. This clause is optional and can be used to filter the data.

The braces { } are used to define sets in MDX. They can include one or many members. A member is an item in a dimension. For instance, in the time dimension, 2020 or January are members.

Here's a sample for 0D_DECU cube. We select the three mesaures 0D_COST, 0D_NETVLINV and 0D_INV_QTY to be columns, and the Company Code (0D_CO_CODE) to be put on the rows.

{% highlight mdx %}
SELECT
NON EMPTY
  { [Measures].[0D_COST],
    [Measures].[0D_NETVLINV],
    [Measures].[0D_INV_QTY] }
ON COLUMNS,
  { [0D_CO_CODE] }
ON ROWS
FROM
  [$0D_DECU]
{% endhighlight %}

This is how it looks like in the result window of MDXTEST. As you see, not only the dedicated Company Codes are shown in the rows, but also the sum of all Comapny Codes (first row). And also a line indicating the sales transactions without company code (indicated by the #, sum is 0 in that case).

![image](/assets/2023-01-10/020.png)

Let's try a small variation. As you see in the ON ROWS section we only select a so called data tuple; just two company codes 1000 and 2000.

{% highlight mdx %}
SELECT
NON EMPTY
  { [Measures].[0D_COST],
    [Measures].[0D_NETVLINV],
    [Measures].[0D_INV_QTY] }
ON COLUMNS,
  { [0D_CO_CODE].[1000],
    [0D_CO_CODE].[2000] }
ON ROWS
FROM
  [$0D_DECU]
{% endhighlight %}

This makes the sresultset limit on these two rows:

![image](/assets/2023-01-10/030.png)

## Doing MDX in Peakboard

Executing MDX from within an SAP data source in Peakboard is quite straight forward. We just use the Execute MDX command to submit the original MDX for the BW system. It is directly forwarded to SAP without and interpretation:

{% highlight SQL %}
EXECUTE MDX | <MyMDXStatement> |;
{% endhighlight %}

Just insert the MDX:

{% highlight SQL %}
EXECUTE MDX |
SELECT NON EMPTY
  { [Measures].[0D_COST], [Measures].[0D_NETVLINV],
    [Measures].[0D_INV_QTY] }
ON COLUMNS,
  { [0D_CO_CODE].[1000], [0D_CO_CODE].[2000] }
ON ROWS
FROM
  [$0D_DECU] |;
{% endhighlight %}

![image](/assets/2023-01-10/040.png)

For creating dynamics MDX statements, feel free to use variables. This sample shows how to inject a dynamic Comapny Code.

![image](/assets/2023-01-10/050.png)

Finally here's the result of the two initial statements (one for all Company Codes, one for limited Company Codes). The column captions and the formatting is applied make the tables to look nicer.

![image](/assets/2023-01-10/060.png)

