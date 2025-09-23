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

## What is a message queue?

A [message queue](https://www.ibm.com/think/topics/message-queues) accepts messages from producers. It temporarily stores the messages, before sending them to the target of the message.

Here's an example of how you could use a message queue with Peakboard:
1. A user uses a Peakboard app to confirm an order.
1. The Peakboard app sends an order confirmation to a message queue. The target is the company's ERP system.
1. The message queue processes and temporarily stores the order confirmation.
1. The message queue sends the order confirmation to the ERP system.

Of course, you could do the exact same thing without a message queue, and in less steps:
1. A user uses a Peakboard app to confirm an order.
1. The Peakboard app sends an order confirmation to the company's ERP system.

So why would you want want to store it first and then send it asynchronously? The biggest reason for is so that the Peakboard app isn't blocked.

If the Peakboard app sends the order confirmation directly (synchronously) to the ERP system, then the app has to wait for the ERP system to process the order and respond to the app. While the Peaboard app is waiting for the ERP system, it cannot do anything else. It is *blocked*.

On the other hand, if the Peakboard app sends the order confirmation to a message queue, then it only has to wait for the message queue to store the order confirmation. This is much faster than waiting for the ERP system to process the message. As soon as the message queue is finished storing the message, the Peakboard app can get back to work.

Then, some time later, the message queue sends the order confirmation to the ERP system. But from the Peakboard app's point of view, this all happens *asynchronously*. It's not involved in the process at all.

Another reason to use a message queue is so that the queue can handle any problems with the message not being accepted. For example, if the target (like an ERP system) is not reachable, or if the message can't be processed for other reasons (e.g. the order is blocked)---then we want to re-send the message after a while. It would be annoying to have our Peakboard app do this itself. With a message queue, the Peakboard app doesn't have to worry about it at all. As soon as it sends the message, it no longer needs to worry about it. It's the message queue's job to send and re-send it, if needed.

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




