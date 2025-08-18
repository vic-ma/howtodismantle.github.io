---
layout: post
title: Influx and Chill - Writing and Querying Time Series Data with Peakboard
date: 2023-03-01 05:00:00 +0300
tags: api bi
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
InfluxDB is a time-series database. Unlike general-purpose databases like SQL Server or MySQL, InfluxDB is designed specifically to deal with time-based data. InfluxDB can scale to handle large volumes of data. It was created by InfluxData, a company based in the Bay Area.

In this article, we'll explain how to build a Peakboard app that reads and writes data from an InfluxDB database.

## Setting up InfluxDB

The easiest way to get started with InfluxDB is by using the official [InfluxDB Docker image](https://hub.docker.com/_/influxdb). The following command spins up a container in under a minute and persists its data in a Docker volume.

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

Once the container is running, InfluxDB listens on port `8086`, at `http://localhost:8086/`. In the web UI, first create an organization and then your initial bucket to store data.

![image](/assets/2025-08-16/010.png)

Next, generate an API token for external reads and writes via the "API Tokens" section.

![image](/assets/2025-08-16/020.png)

With these steps complete, you're ready to send data.

## Writing data

InfluxDB exposes a straightforward HTTP API for reads and writes. To insert data, we POST to `/api/v2/write?org=LosPollosHermanos&bucket=DismantleBucket&precision=s`. The `org` and `bucket` parameters identify where the point will be stored, and `precision=s` indicates that timestamps use second resolution.

The request body uses InfluxDB's line protocol. In the example below the measurement is `temperature`, we tag the sensor as `lab`, and store the field value `26.3`.

{% highlight text %}
temperature,sensor=lab value=26.3
{% endhighlight %}

In our sample Peakboard app the user can enter a value.

![image](/assets/2025-08-16/030.png)

The next screenshot shows the Building Block behind the submit button. We use a placeholder text to inject the user's value into the body of the HTTP call. Besides the body, we need to add two headers:

- `Content-Type` must be set to `text/plain`
- `Authorization` must be set to `Token <YourInfluxAPIToken>`

![image](/assets/2025-08-16/040.png)

After sending the request you should see the submitted value in the Influx Data Explorer.

![image](/assets/2025-08-16/050.png)

## Query data

Querying data is just as straightforward. A single API call returns the results, but the response comes in a CSV format with multiple header lines that requires extra parsing. To avoid that hassle, use the [InfluxDB Extension](https://templates.peakboard.com/extensions/InfluxDB/index), which handles the formatting for you.

The screenshot below shows the extension in action. Provide the following parameters:

- `URL` needs to be filled with the URL of the query API call including the organization name, e.g. `http://localhost:8086/api/v2/query?org=LosPollosHermanos`
- `Token` is the API token
- `FluxQuery` is the query string to describe the requested data


Our sample query is straightforward: read from `DismantleBucket`, limit the range to the last two hours, filter for the `temperature` measurement and its `value` field, and then return the maximum value in that window.

{% highlight text %}
from(bucket: "DismantleBucket")
  |> range(start: -2h)
  |> filter(fn: (r) => r._measurement == "temperature")
  |> filter(fn: (r) => r._field == "value")
  |> max()
{% endhighlight %}


The screenshot shows the result set. Besides the two timestamps (start and end of the query period), the actual value appears in the `_value` column. Our sample outputs a single row, but additional fields or sensors would produce multiple rows.

![image](/assets/2025-08-16/060.png)

## Result

We've seen how easy it is to write to and read from InfluxDB. It scales to huge sizes and is simple to use, but it's best reserved for time-based measurements. Data without a timestamp is better stored in a different database.
