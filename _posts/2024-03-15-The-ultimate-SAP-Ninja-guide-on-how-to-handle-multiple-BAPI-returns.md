---
layout: post
title: The ultimate SAP Ninja guide on how to handle multiple BAPI returns 
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-03-15/title.png
read_more_links:
  - name: More fancy SAP articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/sap
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
    url: /Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html
downloads:
  - name: SAPSuperSophisticatedBAPICalls.pbmx
    url: /assets/2024-03-15/SAPSuperSophisticatedBAPICalls.pbmx
---

In an earlier article, we discussed [how to build an ideal SAP RFC function modules to be connected to Peakboard](/SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html). The basic idea is that the RFC has multiple scalar input parameters, and it returns a single table with the payload. This architecture serves the Peakboard side perfectly, because every data source's output is exactly one table. However, that's often not how it works in the real world.

In the real world, we often have to deal with function modules that don't fit into that structure. This is especially true for SAP standard BAPIs. The actual payload is often returned in multiple tables or in multiple scalar, non-table-like data structures. This article explains how to deal with that situation in Peakboard.

For the following three examples, we use a standard BAPI called `BAPI_PRODORD_GET_DETAIL` to illustrate the example. We have an article on [how to use `BAPI_PRODORD_GET_DETAIL`](/Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html).

## How to handle multiple tables

With the help of our sample BAPI, `BAPI_PRODORD_GET_DETAIL`, we can retrieve multiple tables that are related to a certain production order. Let's assume we want to retrieve the two tables `COMPONENT` (for the materials used in the production order) and `HEADER` (with additional information of the production order). 

For the `COMPONENT` table, we use the regular data source output. For the `HEADER` table, we need a variable list. The following screenshot shows how to create the variable list. The trick is that the column names must exactly match the names of the SAP table column you want to extract. In this example, we only use 4 columns. We also must match the correct data type. 

![image](/assets/2024-03-15/010.png)

Let's go to the SAP data source. Usually, we use the reserved word `@RETVAL` to make the output of the data source equal the output of the table. We do the same with the `COMPONENT` table.

For the `HEADER` table, we just use the `@` character, followed by the name of the variable list. This lets the data source put the table into that variable list. Because we used the same column names, that the data source knows which column to put the data in.

Here's the correct XQL and a screenshot of the data source:

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORD_GET_DETAIL'
   EXPORTS
      NUMBER = '000060003912',
      ORDER_OBJECTS-COMPONENTS = 'X',
      ORDER_OBJECTS-HEADER = 'X'
   TABLES
      COMPONENT INTO @RETVAL,
      HEADER INTO @Header
{% endhighlight %}

![image](/assets/2024-03-15/020.png)

And here is the final result of the two table controls. One is bound to the data source output and one is bound to the variable list.

![image](/assets/2024-03-15/030.png)

## How to handle multiple scalar, non-table-like parameters

Our sample BAPI has a return structure called `RETURN`. This structure contains information about potential errors (like the production order number being invalid). One structure attribute is `TYPE` (`E` for Error) and one is `MESSAGE` (for the actual error message). Let's assume we're interested in handling this information.

First, we need two variable with the correct data type. Let's call them `ReturnType` and `ReturnMessage`.

![image](/assets/2024-03-15/040.png)

Now, we can address these variables like we did with the tables in the previous example. We address it with @MyVariable. Here is the XQL:

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORD_GET_DETAIL'
   EXPORTS
      NUMBER = 'XXX',
      ORDER_OBJECTS-COMPONENTS = 'X'
   IMPORTS
      RETURN-TYPE INTO @ReturnType,
      RETURN-MESSAGE INTO @ReturnMessage
   TABLES
      COMPONENT INTO @RETVAL
{% endhighlight %}

And here's the result in the sample pbmx. The two variable are just bound to two test fields and an error is provoked by requesting a non-existant production order.

![image](/assets/2024-03-15/050.png)

## How to handle function with no return table

In the last two examples we always had at least one return table to feed the usable output of the SAP data source. Let's assume that an RFC function module does not return any tables at all but only has scalar return parameters. We can use our sample BAPI and just ignore the table. So how to handle the returns and turn it into a data source table output?

We again make use of the @RETVAL parameter to address the output table. If we apply that to scalar values the SAP data source will turn it into a table with exactly one row and store the data there. To indicate the column name we just add the column name separated by $. So @RETVAL$MyColumn will store the value in the MyColumn column.

Here's the XQL:

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORD_GET_DETAIL'
   EXPORTS
      NUMBER = 'XXX',
      ORDER_OBJECTS-COMPONENTS = 'X'
   IMPORTS
      RETURN-TYPE INTO @RETVAL$TYPE,
      RETURN-MESSAGE INTO @RETVAL$MESSAGE
{% endhighlight %}

And here's how the data source looks like:

![image](/assets/2024-03-15/060.png)

And here is the preview. The output table of the data source is just bound to a table control.

![image](/assets/2024-03-15/070.png)

## Conclusion

This article shows that's it's actually not too difficult to break out from the traditional one-table-output that applies for any Peakboard data source. Especially in the world of SAP function modules we must be more flexible because SAP is not :-)



