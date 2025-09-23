---
layout: post
title: Hub Flows IV - Peakboard Flows in Production - Asynchronous SAP Confirmation Processing
date: 2023-03-01 00:00:00 +0000
tags: hubflows sap
image: /assets/2025-09-24/title.png
image_header: /assets/2025-09-24/title_landscape.png
bg_alternative: true
read_more_links:
  - name: All articles about Hub Flows
    url: /category/hubflows
downloads:
  - name: SAPProdOrderConfQueueing.pbfx
    url: /assets/2025-09-24/SAPProdOrderConfQueueing.pbfx
---
In the part of our [Hub Flows series](/category/hubflows), we'll discuss another common design pattern: A message queue that sends messages asynchronously. Before continuing, make sure you understand the [basics of Hub Flows](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html) first.

## Why we need queueing?

The idea of queueing is to build an architecture in which message are generated from Peakboard applications, but they are not sent out directly to the desination system, but first stored somewhere and then processed asynchronoulsy. Some typical examples for this could be send out order confirmations for an ERP system or sending out alerts or emails to inform a user about a problem.

It would be possible to do this activity directly as soon as the need comes up in the Peakboard application. So why do we want to store it first and the prcoess it asynchronslouly? The most common reason for this is to not bother the user with this process. Storing all values first is done very fast and the user can keep on working. The actual processing takes longer but no user needs to wait. It happens in the background. The second common reason is when the destination system is not reachable or the processing of the message can't be done for other reasons (e.g. while the order to be confirmed is blocked). Then we want to re-try the processing after a while.

In today's article we will build an architecture to enable queueing. We assume that production order confirmation message are stored in a hub list. Then we build a Hub FLow that loops over all unprocessed message and sends them to SAP. The confirmation message is marked as done as soon as it's confirmed by the SAP system. If anything goes wrong the message is marked as errornous and with the next round of confirmation we will try to process it again until it's done successfully.

For more details about how to send a production order confirmation to SAP we can check back to an article from the past: [Dismantle SAP Production - Build a Production Order Confirmation Terminal with no code](/SAP-Production-Build-a-Production-Order-Confirmation-Terminal-with-no-code.html). We are using the same technique and SAP RFC function module in our example here.

## Preparing the queue table

The actual queue data is stored in a Hub table. For our example for the SAP production order confirmation we will need these columns:

- "ConfirmationNo" is the confirmation number in SAP that is used to identify an operation of a production order
- "YieldQuantity" is the quantity of usable pieces produced within this confirmation 
- "ScrapQuantity" is the quantity of unusable, scrap pieces within this confirmation
- "MachineTime" is the time the machine used to produce the goods
- "State" will identify the state of the confirmation: N - New - untouched conirmation, D - Done - succesfuly sent to SAP, E - Error - confirmation failed 
- "Message" contains the return message from SAP, e.g. the error message when the confiration is in errornious state.

The screenshot shows two untouched confirmations to be processed by our Hub FLows. How these entries are stored there should be not the question here. It can be any Peakboard based source.

![image](/assets/2025-09-24/010.png)

## Building the Hub Flow project

In our Hub FLow project we first set up a data source to previously introduced hub list. The filter should return all rows that are not "Done" which includes untouched rows as well as rows that have failed to be submitted in the past.

![image](/assets/2025-09-24/020.png)

We will need four variables for transfer the correct values in out SAP XQL statement later.

![image](/assets/2025-09-24/030.png)

For the SAP data source we use the following XQL statement. It contains placeholders that refer to the four variables we already created. The output is the DETAIL_RETURN table that contains the message from SAP.

{% highlight sql %}
EXECUTE FUNCTION 'BAPI_PRODORDCONF_CREATE_TT'
   TABLES
      TIMETICKETS = ((CONF_NO, YIELD, SCRAP, CONF_ACTIVITY2, CONF_ACTI_UNIT2, CONF_TEXT),
         ('#[ConfirmationNo]#', '#[YieldQuantity]#', '#[ScrapQuantity]#', 
            '#[MachineTime]#', 'H', 'Submitted by Peakboard')),
      DETAIL_RETURN INTO @RETVAL;

EXECUTE FUNCTION 'BAPI_TRANSACTION_COMMIT'
{% endhighlight %}

![image](/assets/2025-09-24/040.png)

The last component we need is the actual logic to call the SAP system. As shown in the screenshot we loop over all open confirmation rows. For each row we write the four necessary value into the variables and then reload the SAP data source to execute the statement. After this is done we can check the return message. If it's succesful (Return type = "I") we set the confirmation data row on "Done", if not, it's an error.

![image](/assets/2025-09-24/050.png)

## Building an deploying the flow

The next screenshot show the actual Hub Flow to put all our artifacts together. We execute the flow every 60 seonds. After having reloaded the open confirmations we just call the function that loops over every confirmation row (see last paragraph). That's all we need to do.

![image](/assets/2025-09-24/060.png)

The flow runs on regular basis right after being deployed to a Hub instance.

![image](/assets/2025-09-24/070.png)

## result and conclusion

The screenshot shows our confirmation list after the first execution of the Flow. We can see that one of the confirmations has been submitted to SAP successfully while the other one went into an error state. The Flow will automatically retry it in the next round.

![image](/assets/2025-09-24/080.png)

Our example shows a very simple way of queueing such kind of processes. To keep it simple we haven't implemented a complex error handling - we just retry it forever. Some more improvement could be to set up a counter and give up trying after 10 tries. The next improvement to be made could be to send an email to a responsible person in case an error comes up. 




