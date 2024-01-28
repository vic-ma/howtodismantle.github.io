---
layout: post
title: SAP on fire - how to perfectly integrate LUA scripting with SAP
date: 2023-03-01 12:00:00 +0200
tags: sap lua
image: /assets/2024-03-31/title.png
read_more_links:
  - name: More fancy SAP articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/sap
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
    url: /Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html
downloads:
  - name: SAPBAPICallsInScripting.pbmx
    url: /assets/2024-03-31/SAPBAPICallsInScripting.pbmx
---
Because SAP is one of the top 3 systems used with Peakboard, there are already [a lot of SAP articles](https://how-to-dismantle-a-peakboard-box.com/category/sap) available on this blog. But those articles cover how to access SAP for reading and writing from a data source.

In this article, we will learn how to make sophisticated SAP calls directly from a LUA script, without creating a data source.

## SAP connection

For all the examples in this article, we need an active SAP connection. So we make sure that a connection is available in the connection manager, and that we know its ID. 

![image](/assets/2024-03-31/010.png)

To access the SAP connection in LUA, we use the `connections.getfromid` command.

{% highlight lua %}
local con = connections.getfromid('As4kF5peAjw+3MIuEQf3Fc1kEeY=')
{% endhighlight %}

This connection object exposes the `execute` method, which executes regular XQL against the SAP system.

This is the same XQL we used with the SAP data sources. It represents the SAP interface and specifies the object to be addressed in SAP (for example, RFC function module, report, MDX statements, SAP query, table `SELECT`).

In this article, we will only use some sophisticated RFCs and table calls. We will focus on the LUA part.

## Simple data handling

Let's start with some very simple XQL: a `SELECT` statement that gets data from the `MAKT` table. Exactly one table is returned. So the `execute` function returns a LUA array.

The following code shows that you can handle the return data like a regular array-like data object. You can use aggregate functions like `count`, or address a single row with an ordinal. 

{% highlight lua %}
local myMAKT = con.execute('SELECT top 10 * FROM MAKT;')

peakboard.log(myMAKT.count)
peakboard.log(myMAKT[1].MAKTX)
{% endhighlight %}

## The `vals` object

The simple case of returning one table is often not enough. In the real world, we often have to exchange multiple data artifacts with SAP objects. To do this, we use the `vals` object.

The following is the same code from before, but this time, we create a generic array object called `vals` and use the execute function to fill it.

We must tell the XQL engine which container we want to store the return value of our table query in. We do this with the command `into @MyMakt`.

After the call, we can use the data by addressing the same container: `vals.MyMakt`. This instance is an array with the same elements as the original table in SAP. 

{% highlight lua %}
local vals = {}

con.execute('SELECT top 10 * FROM MAKT into @MyMakt;', vals)

peakboard.log(vals.MyMakt.count)
peakboard.log(vals.MyMakt[1].MAKTX)
{% endhighlight %}

## Multiple returns

Let's go one step further and try an RFC function that returns multiple tables.

The following code shows the call of the function module `BAPISDORDER_GETDETAILEDLIST`. You can use it to query information about multiple sales documents with a single call.

The function module accepts a fixed table called `SALES_DOCUMENTS` that contains two rows with two sales document numbers. It returns two tables:
* `ORDER_HEADERS_OUT` as a list of requested sales document headers
* `ORDER_ITEMS_OUT` as a list of all items of all requested sales documents

The pattern for how to use the `vals` object stays the same.

Note that in this example, we use the double brackets in LUA to indicate a string. This way, our XQL can span multiple lines without the need for artificial line breaks. This makes the code much more readable.

{% highlight lua %}
local vals = {}

con.execute([[
EXECUTE FUNCTION 'BAPISDORDER_GETDETAILEDLIST'
  EXPORTS
    I_BAPI_VIEW-HEADER = 'X',
    I_BAPI_VIEW-ITEM   = 'X'
   TABLES
      SALES_DOCUMENTS = ((VBELN),
         ('0000016233'),
         ('0000016232')),
      ORDER_HEADERS_OUT INTO @MyHeaderTable,
	  ORDER_ITEMS_OUT INTO @MyItemsTable
  ]], vals)

peakboard.log(vals.MyHeaderTable.count)
peakboard.log(vals.MyHeaderTable[0].CREATED_BY)

peakboard.log(vals.MyItemsTable.count)
peakboard.log(vals.MyItemsTable[0].MATERIAL)
{% endhighlight %}

## Multiple inputs and outputs

This time, we want to submit data to the XQL engine, not just receive data from it. The following example shows how to make the `SALES_DOCUMENTS` table dynamic.

We set `SALES_DOCUMENTS` before the call, by adding a container named `MyVbelnList` to the `vals` collection and referencing it in the XQL with `@MyVbelnList`.

The same thing works with scalar parameters. In this example, the `X` string is actually injected into the XQL by using `@MyBapiViewX`.

{% highlight lua %}
local vals = {}

vals.MyVbelnList = {
  { VBELN = '0000016232' },
  { VBELN = '0000016233' }
}

vals.MyBapiViewX = 'X'

con.execute([[
EXECUTE FUNCTION 'BAPISDORDER_GETDETAILEDLIST'
  EXPORTS
    I_BAPI_VIEW-HEADER = @MyBapiViewX,
    I_BAPI_VIEW-ITEM   = @MyBapiViewX
   TABLES
      SALES_DOCUMENTS = @MyVbelnList,
      ORDER_HEADERS_OUT INTO @MyHeaderTable,
	  ORDER_ITEMS_OUT INTO @MyItemsTable
  ]], vals)

[processing the return]
{% endhighlight %}

## In and out of the same table

To complete our examples, we need to add two niche cases.

The following is an example where a dynamic table is submitted to and returned by the XQL / SAP system at the same time (the code is shortened for clarity):

{% highlight lua %}
local vals = {}

vals.MyVbelnList = {
  { VBELN = '0000016232' },
  { VBELN = '0000016233' }
}

con.execute([[
...
   TABLES
      SALES_DOCUMENTS = @MyVbelnList INTO @MyVbelnList_out,
...
  ]], vals)

peakboard.log(vals.MyVbelnList_out.count)
peakboard.log(vals.MyVbelnList_out[0].VBELN)
{% endhighlight %}

And here's the same thing, but for processing scalar, non-table-like parameters, to be returned from the RFC function. In this example, we use the RFC `BAPI_MATERIAL_GET_DETAIL` to get some information about a given material. 

The export parameter `MATERIAL_GENERAL_DATA` is a data structure. One of the elements is `MATL_DESC`, the material description. It is stored into the container `MyMatDesc` and then processed.

{% highlight lua %}
local vals = {}

con.execute([[
EXECUTE FUNCTION 'BAPI_MATERIAL_GET_DETAIL'
  EXPORTS
    MATERIAL = '100-100',
    PLANT = '1000'
   IMPORTS
      MATERIAL_GENERAL_DATA-MATL_DESC into @MyMatDesc
  ]], vals)

peakboard.log('Material Description: ' .. vals.MyMatDesc)
{% endhighlight %}

## Conclusion

Calling SAP is sometimes complicated. When calling sophisticated RFCs, the developer needs to perfectly understand how complex data exchange between LUA, the XQL engine, and SAP works. This article covers how to do the exchange in all cases. Feel free to download the sample PBMX and play around with it. All function modules are available in the SAP standard. Enjoy!

![image](/assets/2024-03-31/020.png)

