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

The following screenshot shows a simple, standard RFC function module called `SD_RFC_CUSTOMER_GET`. It returns a list of customers, according to the search pattern for the customer name and customer number (`NAME1` and `KUNNR`). As you can see in the screenshot, the function module throws two exceptions (yes, we use the term *throws*). 

* `NOTHING_SPECIFIED` is thrown when the caller fails to submit any useful import parameters.
* `NO_RECORD_FOUND` is thrown when the search pattern doesn't lead to any data rows in the result set.

![image](/assets/2024-04-16/010.png)

## Handling exceptions in LUA

Calling the `SD_RFC_CUSTOMER_GET` function is straightforward. Our Peakboard app has a simple canvas that asks the user for their input. The actual SAP call happens once the button is pressed. We use LUA to make the call.

![image](/assets/2024-04-16/020.png)

The following is the basic LUA for the call. As you can see, the XQL has a dynamic component that is filled with a placeholder. The XQL returns one table, which can be easily processed.

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

The trick is to understand that the `execute` function must be placed inside a try-catch block: 

{% highlight lua %}
local res, ex = trycatch(function()
	returntable = con.execute(xql)  
end)
{% endhighlight %}

The try-catch block returns two variables:

- `res`: a boolean describing whether an error occurred (true) or not (false).
- `ex`: an exception object that lets you look deeper into the details of the problem that occurred.

If an error occurred (`res` is false), you can use `ex.type` to examine the error. If `ex.type` contains `SAP`, that means an ABAP exception has occurred. In that case, you can use `ex.code` to get the actual exception string, which we saw in `SE37` earlier. This makes it possible to react based on which exception is thrown.

In case there's an SAP ABAP related exception (for example, the connection to SAP can't be established, or the XQL has a syntax error), we dump out a general purpose error message to the log by using `ex.message`.

{% highlight lua %}
if res then
  peakboard.log(returntable.count .. ' rows found')
else
  if ex.type == 'SAP' and ex.code == 'NO_RECORD_FOUND' then
 	  peakboard.log('No data rows found')
  else
    peakboard.log('Unexpected error of type: ' .. ex.type
	  .. ', message: ' .. ex.message)
  end   
end
{% endhighlight %}

## Conclusion

Every time we use XQL in a real-world project, it's a good habit to process and check for the error objects, as shown in this article. Even though an RFC function module is not supposed to throw any ABAP exceptions, they can occur because of unforeseeable circumstances.


