---
layout: post
title: Influx and Chill - Writing and Querying Time Series Data with Peakboard
date: 2023-03-01 00:00:00 +0300
tags: api
image: /assets/2025-08-16/title.png
image_header: /assets/2025-08-16/title_landscape.png
bg_alternative: true
read_more_links:
  - name: Influx Docker Image
    url: https://hub.docker.com/_/influxdb
  - name: Influx API
    url: https://docs.influxdata.com/influxdb/v2/api/v2/
  - name: Influx Query
    url: https://docs.influxdata.com/influxdb/v2/query-data/execute-queries/influx-api/
downloads:
  - name: MyInflux.pbmx
    url: /assets/2025-08-16/MyInflux.pbmx
---
Unlike typical multi-purpose databases like SQL Server or mySQL, Influx is built and designed for a special purpose: Storing data points or measurements that happen at dedicated point in time. Influx can handle them in large scale. It was initially built by InflusData, a tech comapny located in the Bay Area.
In today's article we will have a look on how to read and query data from a influx databases.

## Setting up Influx

The easiest way to get a Influx DB is to install quick [docker image](https://hub.docker.com/_/influxdb). Here's a typical prompt to get it working within 1 minute.

{% highlight text %}
docker run -d 
  --name influxdb 
  -p 8086:8086 
  -v influxdb_data:/var/lib/influxdb2 
  -e DOCKER_INFLUXDB_INIT_MODE=setup 
  -e DOCKER_INFLUXDB_INIT_USERNAME=admin 
  -e DOCKER_INFLUXDB_INIT_PASSWORD=supersecret 
  influxdb:2
{% endhighlight %}

After setup, the Influx DB is listening under the port 8086, so it can be access at `http://localhost:8086/`. We need to create an organisation first and within the organisation we need to create out first bucket to store the data.

![image](/assets/2025-08-16/010.png)

After that we create an API token for reading and writing the data from the outside. We find this option in the "API Token" section.

![image](/assets/2025-08-16/020.png)

That's all we need to prepare as minimum requirement.

## Writing data

Inluf DB offers a very easy-to-use and powerful API for reading and writing data. The details can be checked [here](https://docs.influxdata.com/influxdb/v2/api/v2/). The call we're using for sending data will be `/api/v2/write?org=LosPollosHermanos&bucket=DismantleBucket&precision=s`. We need to provide the organsation along with the bucket the URL. The precision `s` means that the data point will be stored with a precison of one second.

We will send a HTTP POST to submit the data. The body is a very specific format as shown in the example. The actual name of the measure will be `Temperature`. As there might be several places to detected this measure we name the place `lab`, Furthermore we submit the actual value. The exact format and syntax can be checked in the linked documentation.

{% highlight text %}
temperature,sensor=lab value=26.3
{% endhighlight %}

In our sample Peakboard app we just let the user write a value.

![image](/assets/2025-08-16/030.png)

The next screenshot shows the Building Block behind the submit button. We use a placeholder text to inject the user's value into the body of the HTTP call. Beside the body we need to addtwo headers: 

- `Content-Type` must be set to `application/json`
- `Authorization` must be set to `Token MyAPITokenFromTheInflusAdminPortal`

![image](/assets/2025-08-16/040.png)

After testing this code we should be able to find the submitted value in the Influx Data Explorer.

![image](/assets/2025-08-16/050.png)

## Query data

Querying data is actually also relatively straight foward and can be done with one single API call. However the returned data is not in a JSON format that is convenient to use right away. It comes in weird CSV format that needs some more processing intelligence as there are multiple header lines and other stuff to get rid of before processing the raw data. That's why it is recommended to use the [InfluxDB Extension](https://templates.peakboard.com/extensions/InfluxDB/index). With the help of this extension all data formatting issues are solved very elegantly.

The screenshot below shows the extension in action. Here's are the paramters to be filled:

- `URL` need to be filled with the URL of the query API call including the orgnisation name, e.g. `http://localhost:8086/api/v2/query?org=LosPollosHermanos`
- `Token` is API Token
- `FluxQuery` is the query string to decibe the reuqested data

The query we use in our sample is simple: First we provide the name of the bucket, the `range` descibes the time range (last 2 hours) of a certain sensor (`temperature`) with a certain attribute (`value`). And we want to aggregate the data and detec the maximum value within the given time range.

{% highlight text %}
from(bucket: "DismantleBucket")
  |> range(start: -2h)
  |> filter(fn: (r) => r._measurement == "temperature")
  |> filter(fn: (r) => r._field == "value")
  |> max()
{% endhighlight %}

The screenshot shows the result set. Beside the two time stamps (start and end of the query period), the actual value is provided in the `_value` column. In out sample the output is only one row, but this depends on the query. Let's assume our sensor would have two different attibutes to provide (tempearture and accuracy of the temperature), we would see twi rows here.

![image](/assets/2025-08-16/060.png)

## Result

We learned how easy it easy it is to write and read to and from Influx databases. Influx is a very easy to use database and it can scale up to huge sizes. However it should be chosen very carefully. Data that doesn't fit into the typical structure of time based measurement data values are better to be stored in other databases.
