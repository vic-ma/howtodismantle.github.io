---
layout: post
title: Catch Me If You Can - The ultimate Guide on how to handle Exceptions in LUA
date: 2023-03-01 12:00:00 +0200
tags: lua
image: /assets/2024-06-11/title.png
read_more_links:
  - name: More fancy LUA articles
    url: https://how-to-dismantle-a-peakboard-box.com/category/lua
  - name: Expect the unexpected - How to handle SAP exceptions in LUA scripting
    url: /Expect-the-unexpected-How-to-handle-SAP-exceptions-in-LUA-scripting.html

    https://www.lua.org/pil/8.4.html#:~:text=If%20you%20need%20to%20handle,call)%20to%20encapsulate%20your%20code.&text=Of%20course%2C%20you%20can%20call,...%20else%20...
downloads:
  - name: ExceptionHandling.pbmx
    url: /assets/2024-06-11/ExceptionHandling.pbmx
---
Often things don't turn out as expected and so they break unexpectedly - that's true for every real life and also true for every piece of software. We won't solve the real-life-related problem in his article, but we will for code. When code breaks, we say, that an exception is happening. And when the program anticipates this to happen, it can do this by "catching an exception" and react accordingly, e.g. present the user a proper error message or just retry the action that caused the unexpected situation.

A couple of months ago, we already discussed exception in the context of SAP with [this article](/Expect-the-unexpected-How-to-handle-SAP-exceptions-in-LUA-scripting.html).

## Exceptions in LUA code

In most modern programming languages there's a special command to encapsulate a code block and then jump to a dedicated part of the code when the exception happens (like try-catch in C#). In LUA it's a bit different. Here we need to encapsulate the code into a so called pcode function. The technical details are well explained in [this article](https://www.lua.org/pil/8.4.html#:~:text=If%20you%20need%20to%20handle,call)%20to%20encapsulate%20your%20code.&text=Of%20course%2C%20you%20can%20call,...%20else%20...). That sounds much more complicated than it is. Let's jump into a general sample like the one below. The actual code is monitored and the whole block returns a variable called "success" which is either true or false. The second variable "result" is an object with the attributes "message" for the actual error message and "type". Apart from the type that happen in a SAP context, the two types can be

1. "LUA", which means the exception happened within the LUA code, e.g. trying to access an element within a list that doesn't exist.
2. "SYS:XXX", which is set, when the exception "XXX" happens somewhere in the runtime environment, e.g. when we try to connect to a data server that doesn't respond, it's filled with "SYS:SqlException". With the the help of "XXX" we can even distuinguish between different types or sources of runtime exceptions.

{% highlight lua %}
local success, result = trycatch(function()

--- Here is the actual code to be monitored

end)

if success then
  -- Everxthing work as expected
else
   -- The execution failed 
   peakboard.log(result.message)
   peakboard.log(result.type)
end
{% endhighlight %}

Here's a real life sample of trying to connect to a non-existing SQL Server:

{% highlight lua %}
local success, result = trycatch(function()
      local con = connections.getfromid("bwV8uGLHlHv9ZgAc5FtzmqRrspU=")
      con.open()
      con.executenonquery('INSERT INTO [Machines] ([MachineName]) VALUES (\'MyMachine\')')
      con.close()
end)

if success then
   screens['Screen1'].txtOutput.text = 'Everything is good!'
else
   screens['Screen1'].txtOutput.text = 'Type: ' .. result.type .. '\nMessage: ' .. result.message
end
{% endhighlight %}

And here's the result in the final Peakboard app:

![image](/assets/2024-06-11/010.png)

Let's check another sample that generates a "LUA" and not a "SYS" exception though accessing non-existing data artifact.

{% highlight lua %}
local success, result = trycatch(function()
      local Dummy = ''
      Dummy = data.MyEmptyList[5].Column_1
end)

if success then
   screens['Screen1'].txtOutput.text = 'Everything is good!'
else
   screens['Screen1'].txtOutput.text = 'Type: ' .. result.type .. '\nMessage: ' .. result.message
end
{% endhighlight %}

And the result:

![image](/assets/2024-06-11/020.png)

## try/catch in Building Blocks

The following screnshot shows the same sample as mentioned earlier, but as Build Blocks version. The actual code is put into the try/catch frame (marked with "1"). There are two branches, one for success, one for failure. And also there are three variables generated that can be used in case of failure (marked with "2"). "error message" and "type" is the same as mentioned above. "code" is only used in the SAP context.

![image](/assets/2024-06-11/030.png)

## conclusion

Catching exceptions makes sense in lot of contexts because it allows to build apps that can react to unexpected things to happen. With the built-in functions of Peakboard in both Building Blocks and pure LUA it's quite easy to wrap code into a try/catch blocks.


