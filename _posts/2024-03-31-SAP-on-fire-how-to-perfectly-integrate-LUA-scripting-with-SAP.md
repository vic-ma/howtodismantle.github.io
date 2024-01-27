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
  - name: SAPBAPICallsInScripting.pbmxx
    url: /assets/2024-03-31/SAPBAPICallsInScripting.pbmx
---
As SAP is one of the top 3 systems to be used together with Peakboard there are already [a lot of articles](https://how-to-dismantle-a-peakboard-box.com/category/sap) available here on the blog. But all of these articles mostly cover how to access SAP for reading and writing from a data source. In his article we learn how to do very sophisticated SAP calls directly from within a LUA script without the need to create a data source. For all the samples in this article we need an active SAP connection. So we make sure a connection is available in the connection manager and we know its ID. 

![image](/assets/2024-03-31/010.png)

To have this connection available in LUA to do crazy stuff with it, we just use the connections.getfromid command.

{% highlight lua %}
local con = connections.getfromid('As4kF5peAjw+3MIuEQf3Fc1kEeY=')
{% endhighlight %}

This connection object offers the 'Execute' method which executes regular XQL to be shot against the SAP system. This XQL is the same we already used in many contexts in the SAP data sources. It represents the SAP interface and specifies which object to be addressed in SAP (e.g. RFC function module, Report, MDX statements, SAP query, table select....). In this article we will only use some sophisticated RFCs and table calls and fully focus on the LUA part.

## simple data handling

Let's start with some very simple XQL. A table select statement to get data from the MAKT table. Only exactly one table is returned. So in that case the Execute function returns a LUA array. The code shows that you can just handle the return data like a regular array-like data object and use aggregate functions like 'count' or address a single row with the ordinal number. 

{% highlight lua %}
local myMAKT = con.execute('SELECT top 10 * FROM MAKT;')

peakboard.log(myMAKT.count)
peakboard.log(myMAKT[1].MAKTX)
{% endhighlight %}

## the vals object

The simple version of returning exactly one table is often not enough. In real life we often have to exchange multiple data artefacts with the SAP objects. For this we use the vals object. Here's is the code we already know, but this time we create a generic array object called 'vals' and let the execute function fill it. We must tell the XQL engine in which container we want to store the return of our table query. So we do with the term 'into @MyMakt'. After the call we can use the data by addressing this container with the same name: vals.MyMakt. This instance in turn is again an array with the same elements as the original table in SAP. 

{% highlight lua %}
local vals = {}

con.execute('SELECT top 10 * FROM MAKT into @MyMakt;', vals)

peakboard.log(vals.MyMakt.count)
peakboard.log(vals.MyMakt[1].MAKTX)


## multiple returns

Let's go one step further and try an RFC function that returns multiple tables. The following code shows the call of the function module BAPISDORDER_GETDETAILEDLIST. You can use it to query information about multiple sales documents with one single call.
It receives one fixed table SALES_DOCUMENTS that contains two rows with two sales docment numbers. And it returns the tables ORDER_HEADERS_OUT as list of requested sales document headers and ORDER_ITEMS_OUT as a list of all items of all requested sales documents. The pattern how to use the vals object stays the same. By the way: In this sample we use the double brackets in LUA to indicate a string, in that case our XQL, that extends over multiple lines without the need for articificial line breaks. That makes the code much more readable.

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

## mutiple ins and outs

In our next iteration we don't want to only receive data from the XQL enine but also submit data to it. The next sample shows how make the SALES_DOCUMENTS table dynamic and fill before the call just by adding a container named MyVbelnList to the vals  collection and just refer to it in XQL with @MyVbelnList. The same works with scalar parameters. In the sample the 'X' string actually is injected into the XQL by using @MyBapiViewX.

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

## in and out of the same table

To complete the samples we need to add two niche cases. Here is a sample when a dynamic table is used be submitted to and returned by the XQL / SAP system at the same time. (the code is shortened to make it clearer).

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
{% endhighlight %}

peakboard.log(vals.MyVbelnList_out.count)
peakboard.log(vals.MyVbelnList_out[0].VBELN)
{% endhighlight %}

And here's the same to process scalar, non-table-like parameter to be returned from the RFC function. In that sample we use the RFC BAPI_MATERIAL_GET_DETAIL to get some detail information about a given material. The export parameter MATERIAL_GENERAL_DATA is a data strcuture. One of the elements is MATL_DESC, the material decription. it is stored into the container MyMatDesc and then processed.

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

## conclusion

Calling SAP is sometimes complicated and especially calling sophisticated RFCs the developer needs to perfectly understand how complex data exchange between LUA, the XQL engine and SAP works. This article covers all case on how to do the exchange. Feel free to download the sample pbmx and play around. All function modules are avaialble in the SAP standard. Enjoy!

![image](/assets/2024-03-31/020.png)

