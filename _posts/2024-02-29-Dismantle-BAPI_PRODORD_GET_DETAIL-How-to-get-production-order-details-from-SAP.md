---
layout: post
title: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-02-29/title.png
read_more_links:
  - name: Dismantle SAP Production - How to determine workplace capacity
    url: /Dismantle-SAP-Production-How-to-determine-workplace-capacity.html
  - name: How to build a perfect RFC function module to use in Peakboard
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
  - name: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction
    url: /Dismantle-SAP-Production-How-to-get-the-next-work-orders-of-a-workplace-by-using-COOIS-transaction-in-Peakboard.html
downloads:
  - name: SAPProdOrderGetDetails.pbmx
    url: /assets/2024-02-29/SAPProdOrderGetDetails.pbmx
---
Peakboard goes very well together will lots of topics around SAP production. In this article we will cover a simple task to access the components of a production order in SAP. The term "components" covers products that are needed to fullfill a production order. This can be different kinds of material like parts to build the actual target product or other materials like additional things that are packed into the box (e.g. a handbook). Materials can but not must be necessaryily be bound to a certain operation of the production order.

After accessing the production order in SAP (transaction CO03) we can have a look at the components by clicking on the components button.

![image](/assets/2024-02-29/005.png)

Usually the needed components generate additional logistic processes (like getting the material from the warehouse and preparing them for assemblation). 

![image](/assets/2024-02-29/010.png)

## Understanding the BAPI BAPI_PRODORD_GET_DETAIL

The RFC function module BAPI_PRODORD_GET_DETAIL is a standard BAPI (Business Application Programming Interface) to request information about a production order in SAP. It's available in any SAP system where the production module is active.
The screenshot shows the function module in transaction SE37 with which you can find out all necessary details about the function module.
You can see in the screenshot that this function module has two import parameters that are relevant for us. NUMBER is the number of the production order. And ORDER_OBJECTS is a structure to define which parts of the order you want to be part of the response of the call. 

![image](/assets/2024-02-29/020.png)

let's dive deeper into the ORDER_OBJECTS structure. There are several elements we can place an X if we want to have this as part of the reponse. As we're interested in the components we will place an X in the COMPONENTS attribute of the structure. The reason why the function module works like this is to simplify the complexity of the call. If the caller is not interested in for example the header data, we just don't put an X into HEADER. That makes the call perform much faster because it avoids unnecessary internal database queries. 

![image](/assets/2024-02-29/030.png)

Let's jump to the table section. There are several tables returned by the call. We're only interested in the COMPONENTS table.

![image](/assets/2024-02-29/040.png)

Now that we know how to operate the function module, which attributes to fill and what to expect in return, so we can create a XQL statement to call the BAPI. It looks like this:

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORD_GET_DETAIL'
   EXPORTS
      NUMBER = '000060003912',
      ORDER_OBJECTS-COMPONENTS = 'X'
   TABLES
      COMPONENT
      INTO @RETVAL;
{% endhighlight %}

If you use the call later in the Peakboard application we will replace the fixed order number by a variable and replace it dynamically.

## Building the Peakboard app

For the Peakboard app we create a text field, button and table to show the result of the BAPI call.

![image](/assets/2024-02-29/045.png)

The actual XQL we're using has a plcaeholder in it ("#[OrderNo]#"). So the value will be taken from the content of the OrderNo variable. Please note, that the Reload State is set ot manual reload as it doesn't make sense that the data source is running automatically. We only want to run it by the code behind the button.

![image](/assets/2024-02-29/050.png)

Let's have a look at Building Block script behind the button. It just writes the value of the text field (that was entered by the user) into the variable and then trigger a reload.

![image](/assets/2024-02-29/060.png)

## result and conclusion

Let's have a looks at the Peaboard app in action. As you saw in the step-by-step guide in this article only a view steps are necssary to get the function module executed and the result processed properly. A lot of SAP standard function modules are rather complicated, this one is easy to use and handle. 

![image](/assets/2024-02-29/result.gif)








