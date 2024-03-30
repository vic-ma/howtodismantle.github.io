---
layout: post
title: Brick by Brick - Writing data back to Databricks from a Peakboard application
date: 2023-03-01 12:00:00 +0200
tags: bi
image: /assets/2024-07-13/title.png
read_more_links:
  - name: Brick by Brick - Connecting Databricks and Peakboard
    url: /Brick-by-Brick-Connecting-Databricks-and-Peakboard.html
  - name: Databricks Extension
    url: https://templates.peakboard.com/extensions/Databricks/en
  - name: Databricks API / REST Documentation
    url: https://docs.databricks.com/api/workspace/statementexecution/executestatement
downloads:
  - name: DatabricksDataInsert.pbmx
    url: /assets/2024-07-13/DatabricksDataInsert.pbmx
---
Some time ago we already discussed how to [query data from Databricks](/Brick-by-Brick-Connecting-Databricks-and-Peakboard.html). Back then we used the Databricks extension to call and process a restfull http endpoint for submitting a select statement to the database.

In this article we will learn  how to insert data into a Databricks table. We use the same endpoint for executing this command, however in that case we don't need any extension to do so. We just use the built-in function of calling http to accomplish this task.

In out sample we will submit sensor data to the table.

## The Databricks side

We will need an access token for the call. It's already explained how to get this in the [first Databricks article](/Brick-by-Brick-Connecting-Databricks-and-Peakboard.html).

ALso we will need a table to store the data. Here's the sql command to create the table:

{% highlight sql %}
CREATE TABLE sensor_data (
  timestamp DATE,
  sensorid INT,
  temperature DECIMAL(5, 2),
  humidity DECIMAL(5, 2)
);
{% endhighlight %}

And here's the empty table after having created it in the Databricks workbench.

![image](/assets/2024-07-13/010.png)

## The call

The call to insert the data into the table looks like this:

{% highlight sql %}
INSERT INTO sensor_data (timestamp, sensorid, temperature, humidity) 
VALUES ('2023-01-01', 1, -2.35, 76.29);
{% endhighlight %}

However we need to make it more dynamic. That's why we use these #[XXX]# placeholders instead of fixed values:

{% highlight sql %}
INSERT INTO sensor_data (timestamp, sensorid, temperature, humidity) 
VALUES (getdate(), #[sensorid]#, #[temperature]#, #[humidity]#);
{% endhighlight %}

From the first article we know, that the SQL command must be wrapped into a JSon call to be submitted to the endpoint. That's what we do:

{% highlight json %}
{
  "warehouse_id": "fcf3504ab6b8eb3b",
  "statement": "INSERT INTO sensor_data (timestamp, sensorid, temperature, humidity) VALUES (getdate(), #[sensorid]#, #[temperature]#, #[humidity]#);"
}
{% endhighlight %}

## Building the script

We're using Building Blocks to create the call. The actual project is quite simple (feel free to download it [here](/assets/2024-07-13/DatabricksDataInsert.pbmx)). We start with a dynamic placeholder replacment block and put in the string we prepared earlier. Because of the placeholders it generates these sockets for each placeholder where the dynamic values can be plugged in. Of course in our sample we still use fixed values to replace the placeholders. This is only to showcase how dynamic placeholders work.

![image](/assets/2024-07-13/020.png)

We use the prepared JSon string to submit it to the http endpoint. Beside the actual URL we also need to provide the access token (see part 1). In the second part we prcoess the response by using a JPath. The actual response by Databricks contains a lot of information about the call and potential errors. But the path "status.state" is precisely where the correct value can be found to check, if the call has succeeded (string is then "SUCCEEDED").

![image](/assets/2024-07-13/030.png)

## Result

Here's the final result. On the right side in the logging we can see the Databricks JSon response and also the "status.state" string ("SUCCEEDED"). It would be a nice exercise to implement the error case. Let's assume the state the "status.state" string is "FAILED", we can use the same JPath methodology to get the error message from the JSon string. [Here](Taming-the-wild-JSon-How-to-use-JPath-in-Peakboard-scripts.html) we can find out more about how use JPath.

![image](/assets/2024-07-13/040.png)

Finally we can find the inserted data in the Databricks table. Yay!

![image](/assets/2024-07-13/050.png)


