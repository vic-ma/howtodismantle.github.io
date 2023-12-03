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
The first article was published in December 2022. Now, 2023 was clearly the year of the rise of artificial intelligence (AI). That's why we're dedicating this article on to how to use AI.

We will build a unique Christmas moment generator machine. You submit your current mood, a color scheme, and an animal. Then, [DALL-E](https://en.wikipedia.org/wiki/DALL-E) will generate a unique image that reflects these keywords. In this article, we'll have a look behind the scenes on how to connect Peakboard to the OpenAI Large Language Model (LLM) for generating images.

![image](/assets/2023-12-16/result.gif)

## The OpenAI API

To use the OpenAI API, we need an API token. To generate a token, we need an OpenAI account with a paid plan. 

![image](/assets/2023-12-16/010.png)

The API endpoint we use is `https://api.openai.com/v1/images/generations`. It requires an HTTP POST request with a certain body. In the body, the `prompt` describes the situation. In our sample request, we ask for a Christmas scene with a color scheme (dark), an animal (terrier), and a mood (happy).

{% highlight json %}
{
    "model": "dall-e-3",
    "prompt": "Create a typical christmas scenery and please use the following keywords to characterize the image: christmas tree, dark, sealyham terrier, happy",
    "n": 1,
    "size": "1024x1024"
}
{% endhighlight %}

If we try out our sample request, we get the following answer. The most important part is the URL. The generated image is stored in this URL for a certain amount of time. That's all the API knowledge we need to proceed.

![image](/assets/2023-12-16/020.png)

## Build the Peakboard app

The Peakboard app is relatively simple. First, we need three variable lists. These contain the potential values for all three combo boxes: animals, colors, and moods. The lists are bound to the corresponding combo boxes.

We also create a single string variable called JsonResponse. We will need it later.

![image](/assets/2023-12-16/030.png)

The image control is used to display the generated image. We use a dummy web resource with a link to a black square. So in the initial stage (before the first image is dynamically generated and displayed), the image control just shows the black square.

![image](/assets/2023-12-16/035.png)

## Build the API call

The actual API call can be found in the code behind the "Generate" button. This is what it does:

1. Generate the prompt. The three variable components are taken from the combo boxes to make the prompt sound like the one in the sample.
2. Place the prompt in the JSON string at the correct location, so that the resulting JSON string is well-formed and looks like the sample.
3. This is the actual call to the API endpoint. We add two headers: The Authorization/Bearer header that contains the API token, and the Content-Type header that informs the API that we're sending our request in JSON format.
4. Write the result into the JsonResponse variable.

![image](/assets/2023-12-16/040.png)

Now let's have a look at the "Refreshed" script of the JsonResponse string. Each time JsonResponse is changed by the API call script, the following Lua script executes. The script gets the image URL from the JSON string and displays the image on the screen.

Why do we need to do this? Wouldn't it be better to put the JSON processing right after the API call? It would be much better and easier, but the Peakboard Building Blocks don't offer a block for parsing JSON. So we need to have a LUA script in order to use the `json.parse` function.

{% highlight lua %}
local jsonContent = json.parse(data.JsonResponse)
local url = jsonContent["data"][1]["url"]
screens['Screen1'].MainImage.source = url
{% endhighlight %}

![image](/assets/2023-12-16/050.png)

## Conclusion

Besides the nice Christmas theme, this example shows how easy it is to use a Large Language Model from within a Peakboard app. The actual point that needs creativity is how to build the prompt and turn structured attributes (in this case the combo boxes) into a prompt that generates the right output.

Last but not least: If you celebrate it, Merry Christmas to you. And to all others: Enjoy your holidays and the last days of the year! See you in 2024!

Love, Michelle


