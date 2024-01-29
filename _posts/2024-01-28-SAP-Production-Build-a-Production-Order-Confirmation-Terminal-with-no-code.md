---
layout: post
title: Dismantle SAP Production - Build a Production Order Confirmation Terminal with no code
date: 2024-01-28 12:00:00 +0200
tags: sap
image: /assets/2024-01-28/title.png
read_more_links:
  - name: Dismantle SAP Production - How to determine workplace capacity
    url: /Dismantle-SAP-Production-How-to-determine-workplace-capacity.html
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
    url: /Dismantle-BAPI_PRODORD_GET_DETAIL-How-to-get-production-order-details-from-SAP.html
downloads:
  - name: SAPProdOrderConfirmation.pbmx
    url: /assets/2024-01-28/SAPProdOrderConfirmation.pbmx
---

Peakboard is often used with SAP in production environments. One of the most common use cases is building interactive terminals for confirming operations of production orders.

An operation of a production order might require multiple confirmations. For example, a confirmation might be needed for starting the operation, submitting an update, and confirming the operation.

Usually, the operator (end users) uses a confirmation number to submit the confirmation. This number is often printed as a barcode on one of the papers that come with the instructions for fulfilling the operation. 

The following GIF shows our finished interactive terminal. The user enters the confirmation number to get some details from SAP (in this case, the production order number and operation). Then, the user can submit the yield, scrap quantity, and machine time.

![image](/assets/2024-01-28/result.gif)

Of course, this is a sample use case. In the real world, the user might submit more sophisticated values, and the machine time would be detected automatically by the Peakboard application. 

## Get order details from SAP

Before we work on the SAP integration, let's first take a look at the UI. It's a text box that gets the confirmation number from the user, and a text box that prints the order number. The magic happens behind the button.

![image](/assets/2024-01-28/005.png)

By submitting the confirmation number, we can use the function module `BAPI_PRODORDCONF_GET_TT_PROP` to get more information. So we create a SAP data source.

The following XQL fills the table `TIMETICKETS` and gets the return value in the same table from SAP. The actual number is injected into the XQL by using the variable placeholder. Make sure to pre-fill the variable during design time with a valid confirmation number, in order to hit the data load button and get some sample data.
 
{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORDCONF_GET_TT_PROP'
   TABLES
      TIMETICKETS = ((CONF_NO),
         ('#[ConfirmationNo]#'))
      INTO @RETVAL;
{% endhighlight %}

![image](/assets/2024-01-28/010.png)

When the user clicks on **Load Information**, the user's entry is written into the variable, and the data source is reloaded:

![image](/assets/2024-01-28/020.png)

To write the data that is returned from the data source to the textbox output, we use some simple blocks in the *refreshed* script. (Pro tip: feel free to use data binding to get the data into the text boxes. That also works well.)

![image](/assets/2024-01-28/030.png)


## Submit the confirmation to SAP

To submit the user entry to SAP, we can use the same pattern as the previous part. 

The XQL is slightly more complicated. We use the function module `BAPI_PRODORDCONF_CREATE_TT`. The actual data is submitted in the table `TIMETICKET`. In the XQL, we have to fill various columns. The fields `CONF_NO`, `YIELD`, and `SCRAP` are easy to understand.

For submitting a time value (machine time, in our case), this table offers dynamic values, depending on the operation. When we look at the operation in the SAP UI, we can see that the machine time is the second time attribute. That's why we have to fill the `ONF_ACTIVITY2` column. `CONF_ACTI_UNIT2` is set to `H` for hour. `CONF_TEXT` is a random text with additional information.

![image](/assets/2024-01-28/040.png)

Here's the final XQL. Note that we need to add a call of a second function module called `BAPI_TRANSACTION_COMMIT`. If we don't do this, SAP rolls back the command and doesn't do anything.

The table `DETAIL_RETURN` contains the feedback message from SAP. We will use it later, so we define it as the output of the data source.

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

On the canvas, we add some text boxes for user input, as well as a button for submitting the confirmation.

![image](/assets/2024-01-28/050.png)

Behind the submit button, we cast the user input into numbers, put them into the global variables, and reload the data source that does the actual work.

![image](/assets/2024-01-28/060.png)

Finally, here's the *refreshed* script of the call. The output of the data source is used to forward the SAP message to the user. We use a regular pop up notification.

![image](/assets/2024-01-28/070.png)

## Conclusion

This example shows how easy it is to use the standard BAPIs of SAP to read and write production order confirmations. They are well suited for use with Peakboard.

