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
SAP Business Warehouse (BW) can be seen as a module within the SAP product portfolio that is used for analysis. Usually, the modules of the production environment (e.g. FI for financials or MM for material management) send their data to BW. Then, in BW, the data is processed into special reporting objects like InfoObjects, Cubes, DSOs, etc. At the end of this chain, there are visualisation tools like BusinessObjects (BO), or even just Excel, to access these BW objects.

SAP is not the only software company that provides solutions for data analysis; there are many more. That's why this industry has developed a standardized way to access objects in the data analysis system. This standard is called MDX (Multidimensional Expressions). It's a kind of language used to precisely define which data is queried and how it is queried. When we want to access the data in a SAP BW, we will use the MDX language to do so.

It takes some time to fully understand MDX, so please feel free to google for additional material on the web. There are tons of articles and videos for self-learning. This article will only be a crash course on the basics. This won't be enough to cover the knowledge you need to build queries in the real world.

## The 1 minute crash course

If you have access to a SAP BW test system, the best thing to use when learning MDX is the transaction `MDXTEST`. Here, you can type in your MDX statements, execute them right away, and get help for the metadata (dimensions) of the cube you want to query. This screenshot shows `MDXTEST` with the cube `0D_DECU` (see metadata on the left pane). It's a sales demo cube provided by SAP and it is ideal for playing around.

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

`SELECT`: Just like in SQL, this statement specifies what you want to see in the query result. This is followed by the `ON COLUMNS` statement that places this result in the column axis of the result set.

`ON COLUMNS`: This statement specifies the data you want to be put in the columns of the result set, which is typically the measurement you are interested in.

`ON ROWS`: This statement specifies the data to be put in the rows of the result set. This is often some hierarchy of members within a dimension.

`FROM [Cube]`: This statement specifies the cube from which you are querying the data.

`WHERE`: This statement is used to slice the cube by one or more dimensions. This clause is optional and can be used to filter the data.

The braces `{ }` are used to define sets in MDX. They can include one or many members. A member is an item in a dimension. For instance, in the time dimension, `2020` and `January` are members.

Here's an example for the `0D_DECU` cube. We select the three measures `0D_COST`, `0D_NETVLINV`, and `0D_INV_QTY`, to be columns, and the company code (`0D_CO_CODE`) to be put in the rows.

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

This is what it looks like in the result window of `MDXTEST`. As you can see, not only are the individual company codes shown in the rows, but so is the sum of all the company codes (first row). And there is also a row that shows the sales transactions without a company code (indicated by the `#`; the sum is 0 in that case).

![image](/assets/2023-01-10/020.png)

Let's try a small variation. As you can see in the `ON ROWS` section, we only select a data tuple—just two company codes, 1000 and 2000.

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

This limits the result set to these two rows:

![image](/assets/2023-01-10/030.png)

## Using MDX in Peakboard

Executing MDX from within an SAP data source in Peakboard is quite straightforward. We just use the `EXECUTE MDX` command to submit the original MDX for the BW system. It is directly forwarded to SAP without any interpretation:

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

For creating dynamic MDX statements, feel free to use variables. This examples shows how to inject dynamic company code.

![image](/assets/2023-01-10/050.png)

Finally, here's the result of the two sample statements (one for all company codes, and one limited to one company code through a variable). The column captions and the formatting is applied to make the tables to look nicer.

![image](/assets/2023-01-10/060.png)
