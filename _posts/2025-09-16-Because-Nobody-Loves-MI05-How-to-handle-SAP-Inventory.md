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
  - name: xxx.pbmx
    url: /assets/2025-09-16/xxx.pbmx
---
We already talked about many [SAP related use cases](/category/sap) on this blog. Today we want talk about what SAP calls a physcial inventory. This is a process when a person walks around in the warehouse and counts goods. After doing so the quantity of goods that is supposed to be in the warehouse is corrected for the real quantity. In the good old days this was done with the help of paper lists. Today we will leanr how to build a paperless version that runs on a tablet.

## The SAP side

In SAP the main transaction will be MI01 for creating a new inventory list. Our application will replace the tranaction MI05 which is used to submitted the quantity of the counted goods. The screenshots show this document and how it looks like in SAP.

![image](/assets/2025-09-16/010.png)

SAP offers a set of BAPIs to process inventory documents. We will use BAPI_MATPHYSINV_GETDETAIL to get all items of inventory document aftr the use has entered the inventory number (and the fiscal year to make it unique). After the user provded the counts we use BAPI_MATPHYSINV_COUNT to submit the result back to SAP.

let's have a look at the XQL statement we will use to call these BAPIs. For the BAPI_MATPHYSINV_GETDETAIL we just the table ITEMS after submitting inventroy number ans fscial year. Here's an example of that call:

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

For the UI part we place some simple controls on our Peakboard canvas. The user is supposed to provide inveatory number and year at the top, load the document, type in his counts and then submit it back to SAP.

In the center there's styled list to show the items.

![image](/assets/2025-09-16/020.png)

This styled list for the items is bound to a variable list to store all the items that currently process by the application. The three scalar variables are used later make the XQL calls dynmic. It's the same pattner as with other SAP application that use XQL.

![image](/assets/2025-09-16/030.png)

## Getting the inventory document from SAP

For querying the inventory document from SAP we use a regular SAP data source with the above mentioned XQL. However we use placeholders to make the XQL dynamic. The textboxes are bound to the variable and the variables are used in the XQL.

![image](/assets/2025-09-16/040.png)

In the refreshed script we loop through the raw data that is coming from SAP and replicate the fields we need later into the variable list and that in turn is used as the backend for the UI presentation.

![image](/assets/2025-09-16/050.png)

The code behind the `Load Document` button is nothing else than to trigger a refresh of the dynmic data source. That's all.

![image](/assets/2025-09-16/060.png)

## Submitting the count to SAP

As above ementioned we will use BAPI_MATPHYSINV_COUNT to submit the counted numbers followed by BAPI_TRANSACTION_COMMIT to confirm the call. We put all our XQL into a regular SAP data source. However we leave some placeholders in the command. Beside the inventory number and fiscal year we also use a placeholder called `CountTablePayload`. It contains a dynmic number of table rows, depending of how many items the invetory documents brings along.

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

Let's discuss the code behind the `Submit` button. First we loop through items table and build a string for the table content according to the XQL that necessary to put the counted quantity into the correct form. This string is stored in the `CountTablePayload`variable and then, when the data source is triggere, finds its way into the dynmic XQL that trigger sthe SAP call.

![image](/assets/2025-09-16/080.png)

In the refreshed event we process the `RETURN` table. It contains the message from SAP and we foward it to the end user.

![image](/assets/2025-09-16/090.png)

## result and conclusion

We learned how to query an inventory document from SAP and submit the counted quantity back. The whole process can be seen in the video below. Please beware of the fact that this is a sample to be used for demonstration. We still need to add some more functionality to turn out application into something that can be used in a production environment:

- Material text in addtion to the material number
- Value help for selecting an invetory list
- Better check if the user has filled the text input correctly
- correct handling of error messages, not only displaying them

![image](/assets/2025-09-16/result.gif
