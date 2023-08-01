---
layout: post
title: MS Graph API - Dismantle O365 group calendars with MS Graph
date: 2023-08-12 03:00:00 +0200
tags: msgraph api
image: /assets/2023-08-12/title.png
read_more_links:
  - name: MS Graph API Documentation - Get the presence information for multiple users.
    url: https://learn.microsoft.com/en-us/graph/api/calendar-list-events?view=graph-rest-1.0&tabs=http
  - name: MS Graph API Documentation - Use query parameters to customize responses
    url: https://learn.microsoft.com/en-us/graph/query-parameters?tabs=http
  - name: MS Graph API - Understand the basis and get started
    url: /MS-Graph-API-Understand-the-basis-and-get-started.html
---
This article covers a common use case for the combination of MS Graph API and Peakboard: Accessing and processing calendar event data. Beside this we will understand how to use $filter parameter in the Graph API.

Please make sure to read through the basics of using MS Graph API in Peakboard: [MS Graph API - Understand the basis and get started]({% post_url 2023-06-09-MS-Graph-API-Understand-the-basis-and-get-started %})

Office 365 comes with lots of calendars (e.g. different variaties of personal (potentially shared) calendars). This article refers to group calendars that can be typically found to be shared for the whole company or at least within a team. Please note, that a group calendar is typically NOT used to reflect the events of resource like a meeting room. This artefact is called a 'room' and is queried differently.

## The calls

The API call to get the events of the group calendar will need the ID of the group. So first we have to find out this group ID by calling this API. It will list all available group within the tenant.

{% highlight url %}
https://graph.microsoft.com/v1.0/groups
{% endhighlight %}

As it's not necessary to repeat this call, we just call the API once in the Graph explorer and just note the ID for further usage.

![image](/assets/2023-07-21/010.png)

use the ID to form the second call, which gives back the events:

https://graph.microsoft.com/v1.0/groups/dbacbdb8-8252-4f3f-98b6-d5ab1c972bd1/calendar/events

This events API offers addtional filters (see links and documentation)
we add this filter to make sure it desiplays the right events

https://graph.microsoft.com/v1.0/groups/dbacbdb8-8252-4f3f-98b6-d5ab1c972bd1/calendar/events?$filter=start/DateTime gt '2023-06-01T00:00:00Z'

Now to he designer:

the datasource is just the call

But the events are unsorted. How to we do the sorting?

We do the sorting in the dataflow:

- remove unncessary columns
- add a new column only fort osrting
- format the sorting column
- sort with sorting column

thats it. Now we can bind the output pof the dataflow to a table





