---
layout: post
title: How to use RFC_READ_TEXT to handle SAP production order operation texts with Peakboard
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-12-03/title.png
image_header: /assets/2024-12-03/title_landscape.png
read_more_links:
  - name: Dismantle SAP Production - How to determine workplace capacity
    url: /Dismantle-SAP-Production-How-to-determine-workplace-capacity.html
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction
    url: /Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html
downloads:
  - name: SAPPOOperationText.pbmx
    url: /assets/2024-12-03/SAPPOOperationText.pbmx
---
SAP connectivity is a [big topic](https://how-to-dismantle-a-peakboard-box.com/category/sap) on this blog---especially when it comes to production and production planning. In this article, we'll take a look at long texts.

The term "long text" refers to a pattern that is used in many SAP contexts and business objects to store additional information to the object that is not bound to any length restriction. Long texts in SAP are handled in the same way, regardless of the SAP module.

In our example, we will handle long texts of an operation that is bound to a SAP production order. However, the function module we're using to read the text can be used for any long text.

## Long text in SAP

The following screenshots show what a long text in SAP looks like. We start from the operation line. Then, we double-click on the text column to view and edit the long text. This text is what our Peakboard application will download.

![image](/assets/2024-12-03/010.png)

![image](/assets/2024-12-03/020.png)

## Build the Peakboard application

In a previous article, we explained how to [download a production order](/Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html). We used `BAPI_PRODORD_GET_DETAIL` to get a list of components that are used in the order.

Here, we do the same thing, except we download the operations instead of components. In the Peakboard application, we place two text fields for the production order number and operation number and bind it to two variables that we will use later.

![image](/assets/2024-12-03/030.png)

For the data source, we use an XQL statement that uses `BAPI_PRODORD_GET_DETAIL` to download a list of operations for the given production order number. The `#[OrderNo]#` is a placeholder for the `OrderNo` variable.

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORD_GET_DETAIL'
   EXPORTS
      NUMBER = '#[OrderNo]#',
      ORDER_OBJECTS-OPERATIONS = 'X'
   TABLES
      OPERATION
      INTO @RETVAL;
{% endhighlight %}

![image](/assets/2024-12-03/040.png)

We are only interested in the operation that the user provided in the text field. So, we use a data flow to filter the operation list for the operation the user wants.

![image](/assets/2024-12-03/050.png)

We now have access to all the structured operation's fields, including the short description text and the work center. But how do we download the long text for the operation? The magic happens in the refreshed script of the data flow. The refreshed script will execute each time the data flow refreshes.

## `RFC_READ_TEXT`

You can use the function module `RFC_READ_TEXT` to download long text remotely. However, the way to use it isn't as you might expect. There's a table named `TEXT_LINES` that must be filled with two attributes, as well as a long compound key that identifies the text. Sending these values to the function module makes it fill the table with text lines of the text we request.
Initially, we must fill the table with these values:


TDOBJECT | The name of the business object to be queried, in our case AUFK which represents the production order.
TDID | The name of the sub object to identify which text is to be queried, in our case it's AVOT which stands for the production order operation text.
TDNAME | A compound key. In our case the pattern is MMMXXXXXXXXXXYYYY, where MMM is the client, XXXXXXXXXX is the routing number of the operation, and YYYY is the counter of the operation.



* TDOBJECT is the name of the business object to be queried, in our case AUFK which represents the production order.
* TDID is name of the sub object to identify which text is to be queried, in our case it's AVOT which stands for the production order operation text.
* TDNAME is a compound key. In our case the pattern is MMMXXXXXXXXXXYYYY, where MMM is the client, XXXXXXXXXX is the routing number of the operation, and YYYY is the counter of the operation.

The compound key might feel very strange for people who are unfamiliar with SAP compound keys, but actaully it's easy to handle, because the "client" is fixed, and routing number and counter are both easy accessible from the operation table that is returned by the BAPI_PRODORD_GET_DETAIL function.

![image](/assets/2024-12-03/055.png)

Here's is the script that is processed to get the texts. First the table element is built and filled with the three attributes. The key is concatenated from the operations table.
The next step is to execute the XQL. The term "TEXT_LINES = @MyLines INTO @Outputlines" is ued to indicate that the table is sent from the caller to SAP and also is returned from SAP.
In the last part, we just iterate of over the text lines of the return table and concatenate the lines to one single string with line breaks.

{% highlight lua %}
local con = connections.getfromid('As4kF5peAjw+3MIuEQf3Fc1kEeY=')

local vals = {}

local TextKey = '800' .. data.MyOperation.first.ROUTING_NO .. data.MyOperation.first.COUNTER

vals.MyLines = {
  { TDOBJECT = 'AUFK',
    TDNAME =  TextKey,
    TDID =  'AVOT'
  }
}

con.execute([[
 EXECUTE FUNCTION 'RFC_READ_TEXT'
   TABLES
      TEXT_LINES = @MyLines INTO @Outputlines
  ]], vals)


local LongText = ''
  
for i, line in ipairs(vals.Outputlines) do
  LongText = LongText .. line.TDLINE .. '\r'
end

screens['Screen1'].TxtDescriptionLong.text = LongText
{% endhighlight %}

## Result

The video show the final result. The reload of the source is triggered through the button, then the dataflows is triggered automatically. And finally the long text is queried from SAP in the refreshed script.

![image](/assets/2024-12-03/result.gif)
