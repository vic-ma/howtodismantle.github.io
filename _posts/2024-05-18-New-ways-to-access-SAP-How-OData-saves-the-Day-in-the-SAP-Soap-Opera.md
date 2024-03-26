---
layout: post
title: New ways to access SAP - How OData saves the Day in the SAP Soap Opera
date: 2023-03-01 12:00:00 +0200
tags: sap
image: /assets/2024-05-18/title.png
read_more_links:
  - name: How to build an RFC-based CRUD OData service
    url: https://www.techippo.com/search/label/OData%20Service?&max-results=8
  - name: Creating a simple OData service in SAP
    url: https://community.sap.com/t5/technology-blogs-by-members/introduction-to-odata-and-how-to-implement-them-in-abap/ba-p/13474383
  - name: More fancy SAP articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/sap
  - name: How to build the Z_PB_DELIVERY_MONITOR function
    url: /SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html
downloads:
  - name: SAPOdata.pbmx
    url: /assets/2024-05-18/SAPOdata.pbmx
---
In this blog, we've talked a lot about how to connect Peakboard and SAP. We've always used Peakboard's built-in SAP integration, which is based on SAP's RFC protocol. In more than 95% of all cases, this is the best choice for a perfect, smooth, and fast SAP connection.

However, there are circumstances where it makes sense to not use RFC, but rather OData. For example, it makes sense to use OData if OData is a company standard and external systems are not allowed to use a direct SAP connection. 

This article covers how to use OData to connect to SAP. But once again, choosing OData over RFC should only be done when there are very good and unavoidable reasons for doing so. In general, OData is considered slower and much harder to set up properly than RFC.
 
## Configure the SAP side

Building OData services from scratch is explained very well in other tutorials, so we won't explain that here. Instead, take a look at one of these tutorials:

* [Build a simple OData service that exposes a table](https://community.sap.com/t5/technology-blogs-by-members/introduction-to-odata-and-how-to-implement-them-in-abap/ba-p/13474383)
* [Build a CRUD OData service bsed on function modules](https://www.techippo.com/search/label/OData%20Service?&max-results=8)

SAP's OData endpoint is mainly configured in the `SEGW` transaction. A service consists of one or more OData entities. Behind each entity, there is some kind of function that fills the entity with life.

The following screenshot shows two entities of our example service:
* One exposes the `SFLIGHT` table
* The other is based on the RFC function `Z_PB_DELIVERY_MONITOR`, which has the table `T_DELIVERIES`.
We already discussed the internal details of Z_PB_DELIVERY_MONITOR in [this article](/SAP-How-to-build-a-perfect-RFC-function-module-to-be-used-in-Peakboard.html).
In the bottom left corner you see the operations that can be used to be applied on an entity set. For the data query (which is out main purpose) the operation is GetEntitySet.

![image](/assets/2024-05-18/010.png)

We can jump directly to the ABAP workbench by right clicking on the SLFIGHT GetEntitySet service implementation ("Go to ABAP workbench"). The next screenshot shows the actual ABAP code that is called every time the entity set is queried and needs to be filled with a database select command.

![image](/assets/2024-05-18/020.png)

The second example works a bit different. Here we have a mapping to an existing function module called Z_DELIVERY_MONITOR. The screenshot shows the mapping of the T_DELIVERIES table output. If the caller submits a filter to the entity for the attribute VSTEL (which is the Shipping Point) it's submitted to the input parameter I_VSTEL of the function module.

![image](/assets/2024-05-18/030.png)

Again, this is not a tutorial on how to build these services. Please refer to the links mentioned earlier.

## Testing the OData service within SAP

SAP offers a nice option to test and debug all OData services. The transaction is called "/IWFND/MAINT_SERVICE". The screenshot shows how to get to the test environment.

![image](/assets/2024-05-18/040.png)

You can test all OData operations for all entities of a service, and even try out filters and other additions. The screenshot shows a GET request for the SFLIGHT entity. On the bottom right corner you can spot the actual table data within the XML answer of the service.

![image](/assets/2024-05-18/050.png)

## Using the Service in Peakboard

Using the two services within Peakboard is pretty straight forward and almost needs no additional explanation. They are used just as any other OData endpoint.
The screenshot shows the delivery entity. As we learned earlier the service accepts the shipping point VSTEL as filter. Any other filter doesn't have any effect as it is not processed in the current implementation.

![image](/assets/2024-05-18/060.png)

The entity for the SLFIGHT table works the same. However any kind of filter is not processed as there's no implementation for the filter (see ABAP code above). The screenshot shows that the filter is ignored.

![image](/assets/2024-05-18/070.png)

## conclusion

Using OData instead of RFC should be considered very carefully. Creating a proper OData service needs a deep understanding and lots of effort to do. We need definetely very good reasons to do it. If in doubt, we always go for he traditional RFC way...


