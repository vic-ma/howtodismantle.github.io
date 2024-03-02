---
layout: post
title: Dismantle BAPI_PRODORD_GET_DETAIL - How to get production order details from SAP
date: 2024-02-29 12:00:00 +0200
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
Peakboard works well with a lot of things relating to SAP production. In this, article, we will learn how to access the components of a production order in SAP.

The term *components* refers to products that are needed to fulfill a production order, such as:

* The parts that are needed to build the target product.
* Additional items that are packed into the box (like a manual).

Materials can optionally be bound to a certain operation in the production order.

After accessing the production order in SAP (transaction `CO03`), we take a look at the components by clicking the components button.

![image](/assets/2024-02-29/005.png)

Usually, the necessary components generate additional logistic processes (like getting the material from the warehouse and preparing them for assembly). 

![image](/assets/2024-02-29/010.png)

## Understand `BAPI_PRODORD_GET_DETAIL`

The RFC function module `BAPI_PRODORD_GET_DETAIL` requests information about a production order in SAP. It's a standard BAPI (Business Application Programming Interface) and is available in any SAP system where the production module is active.

The following screenshot shows the function module in transaction `SE37`. You can use this to learn all the necessary details about the function module.
You can see in the screenshot that this function module has two import parameters that are relevant to us.
* `NUMBER` is the production order number.
* `ORDER_OBJECTS` is a structure that defines which parts of the order you want to include in the response to the call. 

![image](/assets/2024-02-29/020.png)

Let's dive deeper into the `ORDER_OBJECTS` structure. There's a list of elements, and we can select the ones we want to include in the response. Because we're interested in the components, we select the `COMPONENTS` attribute of the structure.

The function module works like this in order to reduce the complexity of the call. If the caller is not interested in the header data, for example, we can just not select `HEADER`. That makes the call perform much faster, because it avoids unnecessary internal database queries. 

![image](/assets/2024-02-29/030.png)

Let's jump to the table section. There are several tables returned by the call. We're only interested in the `COMPONENTS` table.

![image](/assets/2024-02-29/040.png)

We create an XQL statement to call the BAPI, now that we know how to operate the function module, which attributes to fill, and what to expect in return. Here is the XQL statement:

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORD_GET_DETAIL'
   EXPORTS
      NUMBER = '000060003912',
      ORDER_OBJECTS-COMPONENTS = 'X'
   TABLES
      COMPONENT
      INTO @RETVAL;
{% endhighlight %}

When we use the call later in the Peakboard application, we will replace the fixed order number with a variable, in order to get the order number dynamically.

## Build the Peakboard app

For the Peakboard app, we create a text field, a button, and a table, to show the result of the BAPI call.

![image](/assets/2024-02-29/045.png)

The actual XQL has this placeholder in it: `#[OrderNo]#`. That way, the value will be taken from the contents of the `OrderNo` variable. Note that the reload state is set to manual reload, because it doesn't make sense for the data source to run automatically. We only want it to run as a result of the code behind the button.

![image](/assets/2024-02-29/050.png)

Let's have a look at the Building Block script behind the button. It writes the value of the text field (that is entered by the user) into the variable and then triggers a reload.

![image](/assets/2024-02-29/060.png)

## Result and conclusion

The following recording shows the Peaboard app in action. As you saw in this article, we only need a few steps to be able to execute the function module and process the result properly. A lot of SAP standard function modules are rather complicated, but this one is easy to use and handle. 

![image](/assets/2024-02-29/result.gif)