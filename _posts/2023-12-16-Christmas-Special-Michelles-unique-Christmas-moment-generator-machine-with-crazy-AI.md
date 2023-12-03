---
layout: post
title: Christmas Special - Michelle's unique Christmas moment generator machine with crazy AI
date: 2023-03-01 12:00:00 +0200
tags: api ai
image: /assets/2023-12-16/title.png
read_more_links:
  - name: Dev guide for image generation with Open AI
    url: https://platform.openai.com/docs/guides/images/usage?context=node&lang=curl
downloads:
  - name: ChristmasMomentGeneratorMachine.pbmx
    url: /assets/2023-12-16/ChristmasMomentGeneratorMachine.pbmx
---

This article marks the one-year anniversary of this blog!
The first article was published in December 2022. 2023 was clearly the year of the rise of artificial intelligence (AI). That's why we're dedicating this article on to how to use AI.

Unlike our other articles, we will first take a look at the result. What we're building is a unique Christmas moment generator machine. You submit your current mood, you favorite color, style, and your favorite animal. Then, DALL-E will generate a unique image that reflects these keywords. In this article, we'll have a look behind the scenes on how to connect Peakboard to the OpenAI Large Language Model (LLM) for generating images.

![image](/assets/2023-12-16/result.gif)

## The API of Open AI

For using the API of Open AI we need a token. To generate a token we need an account and also paid plan on the Open AI website. 

![image](/assets/2023-12-16/010.png)

The API endpoint we are using is https://api.openai.com/v1/images/generations. It needs an POST HTTP call with certain body. In the body the 'prompt' describes the situation. In our sample we request a Christmas scenery together with a colour style (dark), an animal (terrier) and a mood (happy).

{% highlight json %}
{
    "model": "dall-e-3",
    "prompt": "Create a typical christmas scenery and please use the following keywords to characterize the image: christmas tree, dark, sealyham terrier, happy",
    "n": 1,
    "size": "1024x1024"
  }
{% endhighlight %}

If we try out our sample statement we get the following answer. The most important part is the URL. Under this location the generated image is stored to be requested for a certain amount of time. That's all the API knowledge we need to go ahead.

![image](/assets/2023-12-16/020.png)

## Building the Peakboard app

The app is relatively simple. First we need three variable lists. They contain the potential values for all three combo boxes: Animals, Colors, Moods. The lists are bound to the corresponding combo boxes. Beside the three lists we create a single string variable called JSonResponse. We will need it later.

![image](/assets/2023-12-16/030.png)

The image control is used to display the generated image. We use a dummy web resource with a link to a black square. So in the initial stage (before the first image is dynamically generated and displayed) the image control just shows the black square.

![image](/assets/2023-12-16/035.png)

## Building the API call

The actual API call can be found in the code behind the 'Generate' button. Here we go:

1. The prompt is generated. The three variable components are taken from the combo boxes to make the prompt sounds like the one in the sample
2. The prompt is placed within the JSon string at the correct place, so that the resulting JSon string is well formed and looks like the sample.
3. This is the actual call to the API endpoint. We need to add two headers: The Authorization/Bearer header contains the API token and the Content-Type header informs the API that we're sending our request in JSon format.
4. The result is written into the JSonString variable.

![image](/assets/2023-12-16/040.png)

Now let's have a look at the 'Refreshed' script of the JSonResponse string. Every time this variable is changed by the API call script, this Lua scirpt is processed to get the URL from the JSon string (see screenshot). Why are we doing this? Wouldn't it be better to put the JSon processing just right after the API call? It would be much better and easier, however, the Peakboard Building Blocks don't offer a block for parsing JSon. That's we need to do the trick with the LUA script to use the 'json.parse' function. It's simply only available when writing LUA directly, not in Building Blocks

{% highlight lua %}
local jsonContent = json.parse(data.JsonResponse)
local url = jsonContent["data"][1]["url"]
screens['Screen1'].MainImage.source = url
{% endhighlight %}

![image](/assets/2023-12-16/050.png)

## conclusion

Beside the nice Christmas theme this example shows how easy it is to use a Large Language Model from within a Peakboard app. The actual point that needs creativity is how to build the prompt and turn structured attributes (in this case the combo boxes) to a prompt that generates the right outcome.

Last but not least: If you celebrate, Merry Christmas to you, to all others: Enjoy you holidays and the last days of the year! See you in 2024!

Love, Michelle


