---
layout: post
title: Because Nobody Likes MI05 - How to handle SAP Inventory
date: 2023-03-01 03:00:00 +0000
tags: sap usecase
image: /assets/2025-09-16/title.png
image_header: /assets/2025-09-16/title_landscape.png
bg_alternative: true
read_more_links:
  - name: SAP-related articles
    url: /category/sap
downloads:
  - name: SAPInventory.pbmx
    url: /assets/2025-09-16/SAPInventory.pbmx
---


We've covered many different ways of [integrating SAP with Peakboard](/category/sap) on this blog. In this article, we'll look at SAP's [physical inventory component](https://help.sap.com/docs/SAP_S4HANA_ON-PREMISE/91b21005dded4984bcccf4a69ae1300c/1e61bd534f22b44ce10000000a174cb4.html)---which lets warehouse staff do a physical inventory count, and then record those numbers into SAP.

In the past, workers had to carry around paper lists to do inventory counts. And once they finished their counts, they had to type the numbers into SAP manually. But today, we'll use Peakboard to build a modern, tablet-based app for recording inventory counts into SAP. This makes the whole workflow paperless and much more streamlined.

## The SAP side

This is what an inventory document looks like in SAP:
![image](/assets/2025-09-16/010.png)

In SAP, we use the `MI01` transaction to create a new inventory document. Normally, the next step is for warehouse staff to do an inventory count, and then use the `MI05` transaction to enter the numbers into the inventory document. But we'll build an applciation to replace the `MI05` step and submit the inventory numbers directly to SAP.

SAP provides a set of BAPIs to process inventory documents:
* `BAPI_MATPHYSINV_GETDETAIL` returns all the items in an inventory document.
* `BAPI_MATPHYSINV_COUNT` makes changes to an inventory document. 

Let's look at the XQL statements that we use to call these BAPIs.

### `BAPI_MATPHYSINV_GETDETAIL`

For `BAPI_MATPHYSINV_GETDETAIL`, we enter the inventory number and fiscal year to specify the inventory document we want. We also specify that we only want the `ITEMS` table.

{% highlight test %}
EXECUTE FUNCTION 'BAPI_MATPHYSINV_GETDETAIL'
   EXPORTS
      PHYSINVENTORY = '0100000004',
      FISCALYEAR = '2025'
   TABLES
      ITEMS INTO @RETVAL
{% endhighlight %}

For `BAPI_MATPHYSINV_COUNT`, we send the following information, so that SAP can identify the proper document:
* The inventory number
* The fiscal year
* The date that the inventory count was performed

We also pass in an `ITEMS` table, which contains the updated stock counts. Each row contains the following columns:
* The item number
* The material number
* The counted quantity
* The unit

The following example shows a call with one item---but later, the script generates the table rows dynamically, to handle any number of items:

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

Now, let's build the Peakboard application. To create the UI, we add a couple of simple controls onto the canvas:
* A text box for the inventory number.
* A text box for the fiscal year.
* A button to load the document, based on the inventory number and fiscal year that the user entered.

Once the document is loaded, the user enters the updated inventory counts. Then, they press a submit button to send the new data back to SAP.

We add a styled list to the center of the screen. This shows all the line items from the inventory document. It makes the data easy to scan and provides a familiar layout for warehouse staff.

![image](/assets/2025-09-16/020.png)

The styled list is bound to a variable list. This variable list contains the items that the application is currently processing. In addition, we use three scalar variables that feed the XQL statements (a pattern you may recognize from other SAP-based apps that we've built on this blog). These variables let us update the inventory number, fiscal year, and dynamic table content---without rewriting the XQL.

![image](/assets/2025-09-16/030.png)

## Get the inventory document from SAP

To query the inventory document, we use a standard SAP data source, configured with the XQL shown above. We use placeholders, to keep the query dynamic, so it can reference whatever inventory number and fiscal year the user typed in. The text boxes on the canvas are bound to variables, and those variables are plugged into the XQL when the data source is refreshed.

![image](/assets/2025-09-16/040.png)

In the refresh script, we loop through the raw data returned by SAP, copy the fields we need into the variable list, and use that list as the backend for the UI. This keeps the interface responsive and allows the user to edit the counts directly in the list.

![image](/assets/2025-09-16/050.png)

The *Load Document* button triggers a refresh of the dynamic data source. When pressed, the placeholders are replaced with the current variable values and the application pulls the latest data from SAP.

![image](/assets/2025-09-16/060.png)

## Submit the inventory count to SAP

As noted earlier, we call `BAPI_MATPHYSINV_COUNT` to submit the inventory count, and then `BAPI_TRANSACTION_COMMIT` to finalize the update. All XQL resides in a standard SAP data source with placeholders for the inventory number, fiscal year, and a `CountTablePayload` that holds a dynamic number of table rows based on the document's items. This setup keeps the script flexible, no matter how many items the inventory document contains.

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

The *Submit* button iterates over the items table and builds a string representing the table content in the format required by the XQL. This string is stored in the `CountTablePayload` variable, and when the data source is triggered the placeholder is replaced with the generated string so the proper SAP call is sent.

![image](/assets/2025-09-16/080.png)

In the refresh event, we process the `RETURN` table, extract the SAP message, and display it to the user. That way the operator immediately sees whether the submission was successful or if something went wrong.

![image](/assets/2025-09-16/090.png)


## Result and conclusion

We learned how to query an inventory document from SAP and submit the counted quantities back to the system. The video below shows the entire process from loading the document to sending the counts. Please remember this example is meant for demonstration only; a production-ready solution would need additional features such as material texts, value help for selecting a list, better validation for user input, and proper error handling rather than simply displaying messages:

- Material text in addition to the material number
- Value help for selecting an inventory document
- Better checks to ensure the user has filled the text inputs correctly
- Proper handling of error messages, not just displaying them

![image](/assets/2025-09-16/result.gif)
