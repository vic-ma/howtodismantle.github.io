---
layout: post
title: Dismantle SAP Production - Build a Production Order Confirmation Terminal with no code
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-01-28/title.png
read_more_links:
  - name: Dismantle SAP Production - How to determine workplace capacity
    url: /Dismantle-SAP-Production-How-to-determine-workplace-capacity.html
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction
    url: /Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html
downloads:
  - name: SAPProdOrderConfirmation.pbmx
    url: /assets/2024-01-28/SAPProdOrderConfirmation.pbmx
---

Peakboard is often used with SAP in production environments. One of the most common use cases is building interactive terminals to confirm operations of production orders.

An operation of a production order might require multiple confirmations. For example, a confirmation might be needed for starting the operation, submitting an update, and confirming the operation.

Usually, the operator (end users) uses a confirmation number to submit the confirmation. This number is often printed as a barcode on one of the papers that come with the instructions for fulfilling the operation. 

The following GIF shows our finished interactive terminal. The user enters the confirmation number to get some details from SAP (in this case, the production order number and operation). Then, the user can submit the yield, scrap quantity, and machine time.

![image](/assets/2024-01-28/result.gif)

Of course, this is a sample use case. In the real world, the user might submit more sophisticated values, and the machine time would be detected automatically by the Peakboard application. 

## How to get order details from SAP

Before we start with SAP related stuff, here's the UI. Just some simple text boxes to get the confirmation number from the user and some text box for printin out the data. The magic happens behind the button.

![image](/assets/2024-01-28/005.png)

By submitting the confirmation number we can use the function module BAPI_PRODORDCONF_GET_TT_PROP to get more information. So we create a SAP data source. The XQL is filling the table TIMETICKETS and getting the return in the same table from SAP. The actual number value is injected into the XQL by using the variable placeholder. Make sure to pre-fill the variable during design time with a valid confirmation number to be able to hit the data load button and get some preview sample data.

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORDCONF_GET_TT_PROP'
   TABLES
      TIMETICKETS = ((CONF_NO),
         ('#[ConfirmationNo]#'))
      INTO @RETVAL;
{% endhighlight %}

![image](/assets/2024-01-28/010.png)

When the user clicks on 'Load Information' the user's entry is just written into the variable and the data source is reloaded:

![image](/assets/2024-01-28/020.png)

For writing the data that is returned from he data source to the textbox output we use some simple blocks in the 'refreshed' script. (Pro tip: Feel free to use data binding to get the data into the text boxes. That also works well)

![image](/assets/2024-01-28/030.png)


## Submitting the confirmation to SAP

To submit the user entry to SAP we can the same pattern as for the first part. The XQL is slightly more complicated. We use the function module BAPI_PRODORDCONF_CREATE_TT. The actual data is submitted in the table TIMETICKET. In the XQL you can see that we have to fill various columns. The fields CONF_NO, YIELD and SCRAP are easy to understand. For submitting the time value (Machine time in our case) this table offers dynamic values depending on the operation. When we look at the operation in SAP UI we can see, that the 'Machine time' is the second time attribute. That's why we have to fill the ONF_ACTIVITY2 column. CONF_ACTI_UNIT2 is set to 'H' for hour. The text CONF_TEXT is just a random text with addtional information.

![image](/assets/2024-01-28/040.png)

Here's the final XQL. Please also note, that we need to add a call of second function module called BAPI_TRANSACTION_COMMIT. If we don't do this SAP rolls back the command and doesn't do anything.

The table DETAIL_RETURN contains the feedback message from SAP. We will use it later on and that's why we define it as the output of the data source.

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORDCONF_CREATE_TT'
   TABLES
      TIMETICKETS = ((CONF_NO, YIELD, SCRAP, CONF_ACTIVITY2, CONF_ACTI_UNIT2, CONF_TEXT),
         ('#[ConfirmationNo]#', '#[YieldQuantity]#', '#[ScrapQuantity]#', 
            '#[MachineTime]#', 'H', 'Submitted by Peakboard')),
      DETAIL_RETURN INTO @RETVAL;

EXECUTE FUNCTION 'BAPI_TRANSACTION_COMMIT'
{% endhighlight %}

The final data source looks like this:

![image](/assets/2024-01-28/045.png)

Here's what we need on the canvas. Just some text boxes for he user input and a button.

![image](/assets/2024-01-28/050.png)

Behind the submit button we just cast the user input to numbers, put into the global variables and reload the data source that does the actual work.

![image](/assets/2024-01-28/060.png)

And one last step. Here's the Refreshed Script of the call. The output of the data source is used to forward the SAP message to the user. We just use a regular pop up notification.

![image](/assets/2024-01-28/070.png)

## Conclusion

This example shows how easy it is to use the standard BAPIs of SAP to read and write production order confirmations. They are very suitable to be used with Peakboard.

