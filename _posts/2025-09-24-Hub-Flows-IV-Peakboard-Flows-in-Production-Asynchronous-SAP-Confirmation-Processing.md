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
In this part of our [Hub Flows series](/category/hubflows), we'll look at another common design pattern: A message queue that sends messages asynchronously. Before continuing, make sure you understand the [basics of Hub Flows](/Hub-FLows-I-Getting-started-and-learn-how-to-historize-MQTT-messages.html).

## What is a message queue?

A [message queue](https://www.ibm.com/think/topics/message-queues) accepts messages from producers. It temporarily stores the messages, before sending them to the target of the message.

Here's an example of how you could use a message queue with Peakboard:
1. A user uses a Peakboard app to confirm an order.
1. The Peakboard app sends an order confirmation to a Hub list, which acts as the message queue.
1. A Hub Flow loops over the order confirmations in the Hub list, and processes each one, sending them to the company's ERP system.

Of course, you could do the exact same thing without a message queue, and in fewer steps:
1. A user uses a Peakboard app to confirm an order.
1. The Peakboard app sends an order confirmation to the company's ERP system.

So why would you want to use a message queue at all?

### Avoid blocking

The biggest reason to use a message queue is to avoid blocking the Peakboard app. If the app sends the order confirmation directly (synchronously) to the ERP system, then the app has to wait for the ERP system to process the order and respond to the app. While the Peaboard app is waiting for the ERP system, it cannot do anything else. It is *blocked*.

On the other hand, if the Peakboard app sends the order confirmation to a message queue, then it only needs to wait for the message queue to store the confirmation (and for a Hub list, this is very quick). As soon as the message queue finishes storing the message, the Peakboard app can get back to work.

Then, some time later, a Hub Flow sends all the unprocessed order confirmations to the ERP system. But this is all the Flow's job. The Peakboard app is not involved in this process at all and does not have to worry about it.

### Handle failures automatically

Another reason to use a message queue is so that the Hub Flow can handle any problems with the message not being accepted. For example, if the target (like an ERP system) is unreachable, or if the message can't be processed for other reasons (e.g. the order is blocked)---then we want to send the message again, after a while.

Normally, our Peakboard app would have to handle these unexpected events and error cases itself. But with a message queue, the Peakboard app doesn't need to worry about it at all. As soon as the app sends the message to the queue, it's no longer the app's responsibility. It's the Hub Flow's job to send the message to the ERP system, handle any failures, and re-send the message, if needed.


## Let's build an example

Now, let's build an example. Assume that there is a Peakboard app that sends order confirmation messages to an SAP system directly.

Our job is to do the following:
1. Create a Hub list (message queue), where the app can send the confirmations instead.
1. Create a Hub Flow that loops over all unprocessed messages in the Hub list, and send them to SAP.

For more details about how to send an order confirmation to SAP, take a look at our [SAP order confirmation article](/SAP-Production-Build-a-Production-Order-Confirmation-Terminal-with-no-code.html). We'll use the same techniques that are discussed in that article. But the focus of this article is on the Hub Flow.

## Create the message queue

We create a new [Hub list](https://help.peakboard.com/hub/Lists/en-hub_new-list.html) to act as our message queue. Our messages are SAP order confirmations. So, we add the following columns to the Hub list:

| Column | Description |
| ------ | ----------- |
| `ConfirmationNo`| Confirmation number in SAP that identifies the operation of a production order.
| `YieldQuantity` | Quantity of usable pieces in the confirmation.
| `ScrapQuantity` | Quantity of unusable scrap pieces in the confirmation.
| `MachineTime`   | The amount of machine time used used to produce the goods.
| `State`         | State of the confirmation: `N` (new), `D` (done), or `E` (error).
| `Message`       | Response message from SAP

Note that the last two columns (`State` and `Message`) have nothing to do with the order confirmation itself. Instead, they are used by our Hub Flow to keep track of which messages have been processed, and which messages still need to be processed.

The following screenshot shows an example of what this list might look like. There are two confirmations in the list: one was processed by the Flow successfully (`State = D`), and the other ran into an error (`State = E`).

![image](/assets/2025-09-24/010.png)

## Build the Hub Flow

Now, let's build the Hub Flow. We create a new Hub Flow project.

### Add the Hub list data source
Next, we add a data source for our Hub List. We use this filter: `State ~= D`. This means that the data source includes all rows in the Hub List that don't have a `State` of `D`.

In other words, the data source includes all rows with a `State` of `N` (new confirmations that the Flow has not touched) or `E` (confirmations that the Flow tried to process but resulted in an error). This way, the data source only includes order confirmations that we still need to process.

![image](/assets/2025-09-24/020.png)


### Create variables
We create four variables, which we will use in our SAP XQL statement:
* `ConfirmationNo`
* `YieldQuantity`
* `ScrapQuantity`
* `MachineTime`

![image](/assets/2025-09-24/030.png)

### Create the SAP data source
Now, we create our SAP data source. We use this data source to send the order confirmations to SAP.

We add the following XQL statement to the data source. It has placeholders for the variables that we created.
We also store the `DETAIL_RETURN` table. This table contains the response status and message from SAP, which we will need later.

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



### Write the confirmation processing script

Finally, we write the Building Blocks script that processes the confirmations in the Hub list:

![image](/assets/2025-09-24/050.png)

Here's how the script works:
1. Loop over each row in our Hub list data source. (Remember, the data source already filters for the confirmations that we still need to process.) For each row:
    1. Set our four variables to the values in the row.
    1. Reload our SAP data source, in order to execute the XQL command and send the order confirmation to SAP.
    1. Check the response status from SAP:
        * If the status is `I`, then the order confirmation succeeded. Set the state of the Hub list row to `D`, for *done*. Then, set the message of the Hub list row to SAP's response message.
        * Otherwise, the order confirmation failed. Set the state of the Hub list row to `E`, for *error*. Then, set the message of the Hub list row to SAP's response message.

## Deploy the Flow

Now, let's deploy our Flow. We add a periodic trigger and set it to 60 seconds. This means that the Flow will execute automatically every 60 seconds.

Next, we add two steps to run, whenever the Flow executes:
1. Reload the `OpenConfirmations` data source. This updates our Hub list data source, to ensure we have the latest data.
1. Run our list processing function.

![image](/assets/2025-09-24/060.png)

Next, we deploy our Flow to a Hub instance.  And as you can see, our Flow now runs on regular basis:

![image](/assets/2025-09-24/070.png)


## Result and conclusion

The following screenshot shows our Hub list after the first execution of the Flow. As you can see, one of the confirmations was submitted to SAP successfully, and the other one had an error (the Flow will automatically retry it, the next time it runs).

![image](/assets/2025-09-24/080.png)

To keep our example simple, we didn't implement any complex error handling---we just keep re-sending order confirmations forever. So an improvement could be to set up a counter for how many times we failed to send an order confirmation to SAP. And if it fails 10 times, then we give up and send an email notification to someone. Another improvement could be to send an email to someone if any error occurs at all.

Once you understand the basic design pattern, you can extend it however you like.