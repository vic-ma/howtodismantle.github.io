---
layout: post
title: Influx and Chill - Writing and Querying Time Series Data with Peakboard
date: 2023-03-01 05:00:00 +0300
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
Unlike typical multipurpose databases like SQL Server or MySQL, Influx is built and designed for a special purpose: storing data points or measurements that happen at a dedicated point in time. Influx can handle them on a large scale. It was initially built by InfluxData, a tech company located in the Bay Area.
In today's article we will look at how to read and query data from an Influx database.

## Setting up Influx

The easiest way to get an Influx DB is to install a quick [Docker image](https://hub.docker.com/_/influxdb). Here's a typical prompt to get it working within 1 minute.

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

After setup, the Influx DB listens on port 8086, so it can be accessed at `http://localhost:8086/`. We need to create an organization first, and within the organization we need to create our first bucket to store the data.

![image](/assets/2025-08-16/010.png)

After that we create an API token for reading and writing the data from the outside. We find this option in the "API Token" section.

![image](/assets/2025-08-16/020.png)

That's all we need to prepare as a minimum requirement.

## Writing data

InfluxDB offers a very easy-to-use and powerful API for reading and writing data. The details can be checked [here](https://docs.influxdata.com/influxdb/v2/api/v2/). The call we're using for sending data will be `/api/v2/write?org=LosPollosHermanos&bucket=DismantleBucket&precision=s`. We need to provide the organization along with the bucket in the URL. The precision `s` means that the data point will be stored with a precision of one second.

We will send an HTTP POST to submit the data. The body is a very specific format as shown in the example. The measurement will be `Temperature`. As there might be several places to detect this measure, we name the place `lab`. Furthermore, we submit the actual value. The exact format and syntax can be checked in the linked documentation.

{% highlight text %}
temperature,sensor=lab value=26.3
{% endhighlight %}

In our sample Peakboard app we just let the user write a value.

![image](/assets/2025-08-16/030.png)

The next screenshot shows the Building Block behind the submit button. We use a placeholder text to inject the user's value into the body of the HTTP call. Besides the body, we need to add two headers:

- `Content-Type` must be set to `application/json`
- `Authorization` must be set to `Token MyAPITokenFromTheInfluxAdminPortal`

![image](/assets/2025-08-16/040.png)

After testing this code we should be able to find the submitted value in the Influx Data Explorer.

![image](/assets/2025-08-16/050.png)

## Query data

Querying data is also relatively straightforward and can be done with one single API call. However, the returned data is not in a JSON format that is convenient to use right away. It comes in a weird CSV format that needs some more processing intelligence as there are multiple header lines and other stuff to get rid of before processing the raw data. That's why it is recommended to use the [InfluxDB Extension](https://templates.peakboard.com/extensions/InfluxDB/index). With the help of this extension all data formatting issues are solved very elegantly.

The screenshot below shows the extension in action. Here are the parameters to be filled:

- `URL` needs to be filled with the URL of the query API call including the organization name, e.g. `http://localhost:8086/api/v2/query?org=LosPollosHermanos`
- `Token` is the API token
- `FluxQuery` is the query string to describe the requested data

The query we use in our sample is simple: first we provide the name of the bucket, the `range` describes the time range (last 2 hours) of a certain sensor (`temperature`) with a certain attribute (`value`). We then aggregate the data and detect the maximum value within the given time range.

{% highlight text %}
from(bucket: "DismantleBucket")
  |> range(start: -2h)
  |> filter(fn: (r) => r._measurement == "temperature")
  |> filter(fn: (r) => r._field == "value")
  |> max()
{% endhighlight %}

The screenshot shows the result set. Besides the two timestamps (start and end of the query period), the actual value is provided in the `_value` column. In our sample the output is only one row, but this depends on the query. Let's assume our sensor would have two different attributes to provide (temperature and accuracy of the temperature); we would see two rows here.

![image](/assets/2025-08-16/060.png)

## Result

We learned how easy it is to write to and read from Influx databases. Influx is a very easy-to-use database and it can scale up to huge sizes. However, it should be chosen very carefully. Data that doesn't fit into the typical structure of time-based measurement values is better stored in other databases.
