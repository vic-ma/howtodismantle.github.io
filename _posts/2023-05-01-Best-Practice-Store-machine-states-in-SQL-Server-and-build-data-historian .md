---
layout: post
title: Best Practice: Store machine states in SQL Server and build data historian 
date: 2023-05-01 00:00:00 +0200
tags: sqlserver tutorial
image: /assets/2023-05-01/example-img.jpeg
---
What are machine states and why I want to build data historian?

It is very common to use Peakboard to collect all kinds of states for artefacts. These could be a state of a machine (running, stopped), or the state of person (working, having a break) or the state of a door (open, closed). There are uncountable examples in the space of industrial applications for the state of a certain thing and very often people want to store how these states changed over the time. Usually because they want to do data analysis later like "How long has the machine been running in total" or "What was the aggregated working time of this person this week". Uncountable examples.

This article explains the basic pattern that is practically used to store a machine state in a database in the most simpliest way. Real life could be a bit more complicated, but the basic pattern is always the same.

What tables do I need?

For the basic pattern we need two tables: One that has one row per machine to store things like the machine name and the current state. The other columns can be irgnored and are need for another sample.

/assets/2023-05-01/010.png

Here is a bit of sample data. We're talking about three machines, one of them is already running.

/assets/2023-05-01/011.png

The second table we need is MachineStateHistory where we store the state changes. As you see in the screenshot, we have an abstract identity column (ID) that serves as an abtract primary key. TS is the time stamp when a new state is set, MachineName obvisuosly the name of the machine of which the new state is applied, and the state itself.

/assets/2023-05-01/020.png

How to set a new state?

When a machine is going into a new state we could just send update and insert commands to the database to reflect the new state in the data. However this exactly what we won't do. We will build and call a stored procedure to do the job for you. If you have never build an SP, feel free to google how to do that.
Here's the code of our simple SP. The caller has to provide the machine name and the new state, then the SP is changing the state in the machine table and also adding a row to the historian data to store the information from which point in time the new state is valid.

CREATE PROCEDURE [dbo].[SetNewState]
	@MachineName nvarchar(20),
	@State nvarchar(20)
AS
BEGIN
	
	Update Machines set state=@State where machinename=@MachineName
	insert into MachineStateHistory (TS, MachineName, State) values(getdate(),@MachineName, @State)

END

Why are we using a Stored Procedure instead of just direct updates? There are many advantages:

1.) The logic of a state change is implemented and maintained at exactly one place. In a perfect world all callers are not allowed to change the data directly but only thorugh SP to maintain full control and consitancy.
2.) The state change is one single transaction. Either both data changes are conducted or none of them. So you can assure that every state change always an entry in the change tracking table.
3.) This is the fastest way to the state change in terms of database resources because calling this SP is just one single call. In real life these kind of SPs are a bit more complicated and sometimes do tens of DB calls in one single state change. So saving DB resources can play a big role when speed matters.

How to call the Stored Procedure from Peakboard?

After setting up the DB part let's have a look at the Peakboard part. A typical pattern is to access the machine table in a data source. You probably need the data for your visualizaion anyway.

/assets/2023-05-01/030.png

Here's a sample on how to call our stored procedure behind a Start / Stop button. There's a special block for calling the SPs. You simply select the SP and the parameters are shown where you can just put the machine name and the new state (the Start button will send a RUN and the stop button with send a STOP state).
AFter calling the SP we do a data source referesh to make sure, the new state is re-fetched from he DB:

/assets/2023-05-01/032.png

After playing around with Start / Stop and calling our SP several times you will see, that the history table fills up with data like his:

/assets/2023-05-01/035.png

Conclusion

The sample above shows how to build and fill a table to store state changes. It is clear, that real world data might be a bit more complicated (more states, more logic, more restrcitions), but the basic pattern is usually the same. Using stored procedure for these kind of data changes is a very powerful and easy understandable tool.

{% highlight ruby %}
def print_hi(name)
  puts "Hi, #{name}"
end
print_hi('Tom')
#=> prints 'Hi, Tom' to STDOUT.
{% endhighlight %}

Check out the [Jekyll docs][jekyll-docs] for more info on how to get the most out of Jekyll. File all bugs/feature requests at [Jekyll’s GitHub repo][jekyll-gh]. If you have questions, you can ask them on [Jekyll Talk][jekyll-talk].

[jekyll-docs]: https://jekyllrb.com/docs/home
[jekyll-gh]:   https://github.com/jekyll/jekyll
[jekyll-talk]: https://talk.jekyllrb.com/
