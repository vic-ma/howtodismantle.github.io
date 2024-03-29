---
layout: post
title: Brick by Brick - Connecting Databricks and Peakboard
date: 2023-03-01 12:00:00 +0200
tags: bi
image: /assets/2024-06-19/title.png
read_more_links:
  - name: Brick by Brick - Writing data back to Databricks from a Peakboard application
    url: /Brick-by-Brick-Writing-data-back-to-Databricks-from-a-Peakboard-application.html
  - name: Databricks Extension
    url: https://templates.peakboard.com/extensions/Databricks/en
  - name: Databricks API / REST Documentation
    url: https://docs.databricks.com/api/workspace/statementexecution/executestatement
downloads:
  - name: DatabricksProductQuery.pbmx
    url: /assets/2024-06-19/DatabricksProductQuery.pbmx
---
[Databricks](https://en.wikipedia.org/wiki/Databricks) is a California based BI company. In this article we give a quick example on how to connect to Databricks from Peakboard and get data.
In general Databricks offers a wide selection of different methods to access the data stored inside for literally any type of client. As Peakboard supports using ODBC drivers it would be obvious to use ODBC to connect. However we choose a different way and use the REST API endpoint to submit queries and receive data. We're doing this because configuring the ODBC is possible yet quite stressful. So using the herein explained extension is easy and straightforward. The sample system we're using is a Microsoft Azure based Databricks instance.

## Configuring Databricks

By default the Rest endpoint and Rest API is available, but before we can use it, we need to generate a token. To do so, we go into the Databricks workbench -> User Setting -> Developer -> Access Tokens -> Manage -> Generate Token. We write down the generated token for later use.

![image](/assets/2024-06-19/010.png)

The second attribute we will need later is the Warehouse ID of the Warehouse instance we want to access. We find this ID through the menu of "SQL Warehouse", then click on the warehouse name. The ID can be found in the detail screen.

![image](/assets/2024-06-19/020.png)

## Setting up the Databricks Extension

We will use the REST endpoint of Databricks to query the data. So actually we can use the JSon datasource to query the data and process. The tricky thing is, that in the JSon response of a query and the interpretation of the JSon string is not easy and strightforward because the actual data and data description like datatypes and other metadata is stored at different places.
This problem is solved by the Databricks extension, that can be installed with a click of button. More information about how to install an extension be found [here](https://help.peakboard.com/data_sources/Extension/en-ManageExtension.html).

![image](/assets/2024-06-19/030.png)

## Understanding the Databricks Rest Service

The Databricks instance comes with a REST webservice that exposes any kind DWH function to the outside. The endpoint we're looking for is "api/2.0/sql/statements/". So the whole URL looks like this (depending on the user's actual host name):

{% highlight url %}
https://adb-7067375420864287.7.azuredatabricks.net/api/2.0/sql/statements/
{% endhighlight %}

The query is done through a POST request containing a simple JSon with the DWH ID and also the actual SQL statement. When referring to tables we must always provide a qualified name with Schema information. Here's a sample of a typical query.

{% highlight json %}
{
  "warehouse_id": "fcf3504ab6b8eb3b",
  "statement": "select * from pbws.default.products"
}
{% endhighlight %}

The documentation of this API function can be found [here](https://docs.databricks.com/api/workspace/statementexecution/executestatement).

## Setting up the datasource

With the preperation of the URL, the access token (aka Bearer Token) and the JSon body with the SQL statement encapsulated it's easy to fill the necessary parameters and execute the query.

![image](/assets/2024-06-19/040.png)

## result and conclusion

This article comes with a sample pbmx file with two Databricks sources (products and sales transaction) that are joined to one single table.
It might sounds strange not to use the ODBC driver and stick to the native REST calls but in real life it turned out that the REST option is far easier to handle. Especially when there are additional complexity like dynamic parameters, additional execution attributes, etc....

![image](/assets/2024-06-19/050.png)








