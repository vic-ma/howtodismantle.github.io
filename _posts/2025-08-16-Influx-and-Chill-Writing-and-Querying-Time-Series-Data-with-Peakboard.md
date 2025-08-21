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
InfluxDB is a time-series database. Unlike general-purpose databases like MySQL, InfluxDB is designed specifically to deal with timestamped data---and it can scale to handle large volumes of data. It was created by InfluxData, a company based in the Bay Area.

In this article, we'll explain how to build a Peakboard app that reads from and writes to an InfluxDB database.

## Set up InfluxDB

### Create an InfluxDB container

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

Once the container is running, InfluxDB listens on port 8086.

### Set up the database

To set up the database, we go to `http://localhost:8086/`, in a web browser.
Then, we [create an organization](https://docs.influxdata.com/influxdb/v2/admin/organizations/create-org/).

Then, we create a bucket to store data in:

![image](/assets/2025-08-16/010.png)

Next, click the *API TOKENS* tab and [generate a new API token](https://docs.influxdata.com/influxdb/v2/admin/tokens/create-token/) for external reads and writes.

![image](/assets/2025-08-16/020.png)

Now, you can write data to your InfluxDB database.

## Write data

InfluxDB exposes an HTTP API for reads and writes. To insert data, we use the [`api/v2/write` endpoint](https://docs.influxdata.com/influxdb/v2/write-data/developer-tools/api/):
```
POST /api/v2/write?org=LosPollosHermanos&bucket=DismantleBucket&precision=s
```

The query parameters tell InfluxDB where to store our data:

| Parameter | Description |
| --------- | ----------- |
| `org`     | The organization to store the data in.
| `bucket`  | The bucket to store the data in.
| `precision` | The precision of the timestamp (`s` means seconds).

The request body contains the data we want to store. It uses [InfluxDB's line protocol](https://docs.influxdata.com/influxdb/v2/reference/syntax/line-protocol/).

The following is an example request body that follows this protocol. The measurement is `temperature`, the sensor is called `lab`, and the temperature value is `26.3`.

{% highlight text %}
temperature,sensor=lab value=26.3
{% endhighlight %}

In our Peakboard app, the user enters the temperature value they want to store:

![image](/assets/2025-08-16/030.png)

Here are the Building Blocks behind the submit button:
![image](/assets/2025-08-16/040.png)

We use a placeholder in the request body and replace it with the user's actual value. We also add two headers:
- `Content-Type`, which is set to `text/plain`.
- `Authorization`, which is set to `Token <YourInfluxAPIToken>`.

After sending the request, we can see the submitted value in the Influx Data Explorer:

![image](/assets/2025-08-16/050.png)

## Query data

To query data, we use the [`api/v2/query` endpoint](https://docs.influxdata.com/influxdb/v2/query-data/execute-queries/influx-api/#send-a-flux-query-request). However, this data comes in a CSV format with multiple header lines, so it requires extensive parsing. To avoid all that hassle, we use the [InfluxDB extension](https://templates.peakboard.com/extensions/InfluxDB/index), which handles the parsing for us.

We install the InfluxDB extension. Then, we create a new `InfluxDbQueryCustomList` data source. We provide the following parameters:

| Parameter   | Description |
| ----------- | ----------- |
| `URL`       | The URL of the query API call, including the organization name. For example: `http://localhost:8086/api/v2/query?org=LosPollosHermanos`
| `Token`     | The API token that authenticates us.
| `FluxQuery` | The query string that specifies the data we want.

The following is our example query: 

{% highlight text %}
from(bucket: "DismantleBucket")
  |> range(start: -2h)
  |> filter(fn: (r) => r._measurement == "temperature")
  |> filter(fn: (r) => r._field == "value")
  |> max()
{% endhighlight %}

It does the following:
1. Read from `DismantleBucket`
1. Limit the range to the last two hours
1. Filter for the `temperature` measurement
1. Filter for the `value` field
1. Return the maximum value in that window.

The following screenshot shows the response to our query. There are two timestamps for the start and end of the query period. And the actual value appears in the `_value` column. Our example outputs a single row---but additional fields or sensors would produce multiple rows.

![image](/assets/2025-08-16/060.png)

## Result

You just saw how easy it is to write to and read from InfluxDB with Peakboard. InfluxDB scales to massive sizes and is simple to use, but it's best reserved for time-based measurements. Data without timestamps is better suited for other types of databases.