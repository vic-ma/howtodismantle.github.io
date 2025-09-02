---
layout: post
title: 2025-09-16 Because Nobody Loves MI05 - How to handle SAP Inventory
date: 2023-03-01 03:00:00 +0000
tags: sap usecase
image: /assets/2025-09-16/title.png
image_header: /assets/2025-09-16/title_landscape.png
bg_alternative: true
read_more_links:
  - name: SAP related article
    url: /category/sap
downloads:
  - name: SAPInventory.pbmx
    url: /assets/2025-09-16/SAPInventory.pbmx
---
We already talked about many [SAP related use cases](/category/sap) on this blog. Today we want to talk about what SAP calls a physical inventory. This is a process when a person walks around in the warehouse and counts goods. After doing so the quantity of goods that is supposed to be in the warehouse is corrected for the real quantity. In the good old days this was done with the help of paper lists. Today we will learn how to build a paperless version that runs on a tablet.

## The SAP side

In SAP the main transaction will be MI01 for creating a new inventory list. Our application will replace the transaction MI05 which is used to submit the quantity of the counted goods. The screenshots show this document and how it looks like in SAP.

![image](/assets/2025-09-16/010.png)

SAP offers a set of BAPIs to process inventory documents. We will use BAPI_MATPHYSINV_GETDETAIL to get all items of inventory document after the user has entered the inventory number (and the fiscal year to make it unique). After the user provided the counts we use BAPI_MATPHYSINV_COUNT to submit the result back to SAP.

Let's have a look at the XQL statement we will use to call these BAPIs. For BAPI_MATPHYSINV_GETDETAIL we use only the table ITEMS after submitting the inventory number and fiscal year. Here's an example of that call:

{% highlight test %}
EXECUTE FUNCTION 'BAPI_MATPHYSINV_GETDETAIL'
   EXPORTS
      PHYSINVENTORY = '0100000004',
      FISCALYEAR = '2025'
   TABLES
      ITEMS INTO @RETVAL
{% endhighlight %}

For the BAPI_MATPHYSINV_COUNT we need to submit inventory number, fiscal year and the actual count date to identify the document. The actual count items are submitted in the table ITEMS. The minimum requirement is the item number, Material number, and the counted quantity along with the unit. Here's an example of a simple call with one item in the table. We will later generate parts of this XQL dynamically through code.

{% highlight test %}
EXECUTE FUNCTION 'BAPI_MATPHYSINV_COUNT'
   EXPORTS
      PHYSINVENTORY = '0100000004',
      FISCALYEAR = '2025',
      COUNT_DATE = '20250824'
   TABLES
      ITEMS = ((ITEM, MATERIAL, ENTRY_QNT, ENTRY_UOM),
         ('002', '100-120', '51', 'ST')),
   RETURN INTO @RETVAL;

EXECUTE FUNCTION 'BAPI_TRANSACTION_COMMIT'
{% endhighlight %}

## The Peakboard application

For the UI part we place some simple controls on our Peakboard canvas. The user is supposed to provide inventory number and year at the top, load the document, type in his counts and then submit it back to SAP.

In the center there's a styled list to show the items.

![image](/assets/2025-09-16/020.png)

This styled list for the items is bound to a variable list to store all the items that are currently processed by the application. The three scalar variables are used later to make the XQL calls dynamic. It's the same pattern as with other SAP applications that use XQL.

![image](/assets/2025-09-16/030.png)

## Getting the inventory document from SAP

For querying the inventory document from SAP we use a regular SAP data source with the above-mentioned XQL. However, we use placeholders to make the XQL dynamic. The text boxes are bound to variables, and the variables are used in the XQL.

![image](/assets/2025-09-16/040.png)

In the refreshed script we loop through the raw data that is coming from SAP and replicate the fields we need later into the variable list and that in turn is used as the backend for the UI presentation.

![image](/assets/2025-09-16/050.png)

The code behind the `Load Document` button is nothing more than to trigger a refresh of the dynamic data source. That's all.

![image](/assets/2025-09-16/060.png)

## Submitting the count to SAP

As mentioned above we will use BAPI_MATPHYSINV_COUNT to submit the counted numbers followed by BAPI_TRANSACTION_COMMIT to confirm the call. We put all our XQL into a regular SAP data source. However, we leave some placeholders in the command. Besides the inventory number and fiscal year, we also use a placeholder called `CountTablePayload`. It contains a dynamic number of table rows, depending on how many items the inventory document brings along.

{% highlight test %}
EXECUTE FUNCTION 'BAPI_MATPHYSINV_COUNT'
   EXPORTS
      PHYSINVENTORY = '#[MyInventoryNo]#',
      FISCALYEAR = '#[MyFiscalYear]#',
      COUNT_DATE = '20250824'
   TABLES
      ITEMS = ((ITEM, MATERIAL, ENTRY_QNT, ENTRY_UOM),
         #[CountTablePayload]#),
      RETURN INTO @RETVAL;

EXECUTE FUNCTION 'BAPI_TRANSACTION_COMMIT'
{% endhighlight %}

![image](/assets/2025-09-16/070.png)

Let's discuss the code behind the `Submit` button. First we loop through the items table and build a string for the table content according to the XQL that is necessary to put the counted quantity into the correct form. This string is stored in the `CountTablePayload` variable and then, when the data source is triggered, finds its way into the dynamic XQL that triggers the SAP call.

![image](/assets/2025-09-16/080.png)

In the refreshed event we process the `RETURN` table. It contains the message from SAP and we forward it to the end user.

![image](/assets/2025-09-16/090.png)

## Result and conclusion

We learned how to query an inventory document from SAP and submit the counted quantity back. The whole process can be seen in the video below. Please be aware of the fact that this is a sample to be used for demonstration. We still need to add some more functionality to turn our application into something that can be used in a production environment:

- Material text in addition to the material number
- Value help for selecting an inventory list
- Better checks to ensure the user has filled the text inputs correctly
- correct handling of error messages, not only displaying them

![image](/assets/2025-09-16/result.gif)
