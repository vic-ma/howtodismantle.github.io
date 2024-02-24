---
layout: post
title: Expect the unexpected - How to handle SAP exceptions in LUA scripting
date: 2023-03-01 12:00:00 +0200
tags: sap lua
image: /assets/2024-04-16/title.png
read_more_links:
  - name: More fancy SAP articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/sap
  - name: SAP on fire - how to perfectly integrate LUA scripting with SAP
    url: /SAP-on-fire-how-to-perfectly-integrate-LUA-scripting-with-SAP.html
  - name: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
    url: /Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html
downloads:
  - name: SAPExeptionHandling.pbmx
    url: /assets/2024-04-16/SAPExeptionHandling.pbmx
---
As readers of this blog know, we have [a lot of articles about SAP](https://how-to-dismantle-a-peakboard-box.com/category/sap). The last one was [SAP on fire - how to perfectly integrate LUA scripting with SAP](/SAP-on-fire-how-to-perfectly-integrate-LUA-scripting-with-SAP.html). But there was something missing in that article, which we will discuss now.

In this article, we will learn how to handle ABAP exceptions when calling RFC function modules or BAPIs, in SAP, from Peakboard.
 
## RFC exceptions

SAP ABAP has a very unique (some might say old-fashioned) way of handling exceptions in function modules. If we look at transaction `SE37` to find out more, we see the **Exceptions** tab.

The following screenshot shows a simple, standard RFC function module called `SD_RFC_CUSTOMER_GET`. It returns a list of customers, according to the search pattern for customer name and customer number (`NAME1` and `KUNNR`). As you can see in the screenshot, the function module throws two exceptions (yes, we use the term *throws*). 

* `NOTHING_SPECIFIED` is thrown when the caller fails to submit any useful import parameters.
* `NO_RECORD_FOUND` is thrown when the search pattern doesn't lead to any data rows in the result set.

![image](/assets/2024-04-16/010.png)

## Handling exceptions in LUA

Calling the `SD_RFC_CUSTOMER_GET` function is straight forward. For our Peakboard app, we use a simple canvas that asks the user for their input. The actual SAP call happens once the button is pressed. We use LUA to make the call.

![image](/assets/2024-04-16/020.png)

The basic LUA to do this, we can see here. We see, that the actual XQL has a dynamic component that is filled through a placeholder. The execution of the XQL just returns one table that can be easily processed.

{% highlight lua %}
local con = connections.getfromid('As4kF5peAjw+3MIuEQf3Fc1kEeY=')

local xql = [[
EXECUTE FUNCTION 'SD_RFC_CUSTOMER_GET'
  EXPORTS
    NAME1   = '@MySearchPattern'
   TABLES
      CUSTOMER_T INTO @RetVal
  ]]
  
xql = string.gsub(xql, '@MySearchPattern', screens['Screen1'].txtCustomerFilter.text)

local returntable = con.execute(xql)  

peakboard.log(returntable.count .. ' rows found')
{% endhighlight %}

The secret trick to understand is that the 'execute' function not only returns the output of the XQL statement, but also a second object that represents all potential errors. To process both objects we need to adjust the call to handle both objects. 

{% highlight lua %}
local returntable, error = con.execute(xql)  
{% endhighlight %}

If there is an error, the resulttable is 'nil' and error object is filled. In case of an error, the error object is filled, but resulttable is 'nil'.
This makes it possible to distiniguish between these cases. When it comes to an error the attribute 'type' is the first thing to check. There are two options: Type 'ABAP' is an exception that is thrown by the SAP system, in case of 'XQL' there might be syntax error in the XQL. So we have to check for ABAP-errors first, and then can use the 'code' attribute to get the actual exception string that we already saw in SE37 earlier. This makes it possible to react accordingly to which exception is thrown.

{% highlight lua %}
if error then
 if error.type == 'ABAP' and error.code == 'NO_RECORD_FOUND' then
 	peakboard.log('No data rows found')
 else
    peakboard.log('Unexpected error: ' .. error.message)
 end
elseif returntable then
   peakboard.log(returntable.count .. ' rows found')
end
{% endhighlight %}

The error object offers two more attribute worth to mention: 'message' is just a generic message that comes from the XQL engine. It might helpful to dump this message into the log for further investigation. The attribute 'stacktrace' goes deep into the technical details that lead to the error. It might used for support cases or other low lever debugging.

## conlusion

Every time we use XQL in a real life project it's a good habit to process and check for the error objects as shown in this article. Even though an RFC function module is not supposed to throw any ABAP exceptions they can occur because of unforeseeable circumstances.


