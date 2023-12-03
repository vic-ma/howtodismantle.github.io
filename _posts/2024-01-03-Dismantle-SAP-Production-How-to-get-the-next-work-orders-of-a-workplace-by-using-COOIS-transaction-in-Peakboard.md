---
layout: post
title: Dismantle SAP Production - How to get the next work orders of a workplace by using COOIS transaction in Peakboard
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-01-03/title.png
read_more_links:
  - name: Dismantle SAP Production - How to determine workplace capacity
    url: /Dismantle-SAP-Production-How-to-determine-workplace-capacity.html
  - name: How to make the SAP system fit for report execution
    url: /How-to-make-the-SAP-system-fit-for-report-execution.html
downloads:
  - name: SAPWorkplaceOperations.pbmx
    url: /assets/2024-01-03/SAPWorkplaceOperations.pbmx
---

In another article, we discussed [how to get the capacity of a workplace](/Dismantle-SAP-Production-How-to-determine-workplace-capacity.html). This article covers a common way to list or process the work orders of a workplace.

We will use the data from a transaction COOIS to get the work orders. Technically, we are executing the report behind the COOIS transaction. This report is called `PPIO_ENTRY`.

If you don't know how to find the name of the report from the transaction name, just call the transaction and go to the menu system -> status and find the text field for `program`. That's the report name. If you do this for a COOIS transaction, you will find `PPIO_ENTRY`.

To execute a report in Peakboard, you need to do a small installation on the SAP side to get it working. All the details are explained in [this article](/How-to-make-the-SAP-system-fit-for-report-execution.html).

## Understanding COOIS

COOIS is the number one reporting transaction for getting an overview of all production in general. There are a lot of options to select production data from different levels (header, material, operations, etc.).

The select options and the selection screen might vary depending on the SAP release. This article uses the 2019 S4/HANA COOIS transaction, which might look different from the reader's SAP system. That doesn't matter, because the general principle explained in this article remains the same for all common SAP releases over the last few years.

Let's assume we're interested in the operations of a certain workplace. So we fill out the selection screen according to the given plant and workplace. We also limit the operations on "not confirmed" and "partly confirmed."

The status values to chose here depends heavily on the customization of the SAP production module. The values in the screenshot are only valid for this particular SAP system.

We must also take care of the layout. The layout determines which column are selected. Under certain circumstances it makes sense to create an individual layout only for use with Peakboard. All columns we need later in the application should be covered.

![image](/assets/2024-01-03/010.png)

![image](/assets/2024-01-03/015.png)

Let's try out the selection and check the result.

Side note: It might be necessary to apply additional filters on the result *after* the selection. If we want to do this, we must save these filters together with the layout. Then, we must apply the layout in the selection screen.

![image](/assets/2024-01-03/020.png)

After having checked the result, we go back to the selection and save the selection as variant. A good practice is to use the workplace in the selection as part of the name of the variant.

![image](/assets/2024-01-03/030.png)

![image](/assets/2024-01-03/035.png)

## Call the report from Peakboard

In Peakboard Designer, we create another SAP data source and apply the following XQL, which executes the report `PPIO_ENTRY` with the just created variant `PB_Z_ASM3`.

{% highlight sql %}
EXECUTE REPORT 'PPIO_ENTRY' USING 'PB_Z_ASM3'
{% endhighlight %}

This screenshot shows the data source together with the preview. This should match what we checked directly in the COOIS transaction earlier.

Note: The name of the report columns are language dependent. So if we change the login language, it will change the column names and mess up all your metadata.

![image](/assets/2024-01-03/040.png)

## Final result

If we use a regular table to and adjust the column width, format and naming of the columns we can easily create and cool table with the workplace's operations. The screenshot shows the table in design and runtime.

![image](/assets/2024-01-03/050.png)

![image](/assets/2024-01-03/060.png)

## Additional hints

Here are some more things to consider:

1. If for whatever reasons the COOIS transaction does not fullfill your needs, you can try to build a query and use this query instead.
2. The pattern described in this article is fully language dependant. This is also true for the system status values. For example when you limit the operations to "Confirmed", the english status is CNF while the German status is RÜC. So the selection value must be changed when the login language changes.
3. Using reports for data selection give you any date value in the format of the current language. The typical SAP date format YYYYMMDD can't be expected here. You need to convert it in Peakboard tothe format you like.
4. In this sample we have chosen the level of "Order Headers" for the selection. Depending on the use case it might make sense to chose for "operations" or other levels.