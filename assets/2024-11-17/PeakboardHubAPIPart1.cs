using Newtonsoft.Json.Linq;

const string BaseURL = "http://20.218.250.53:17545";
const string APIKey = "8a541fc7b45def276fead783e348815ea46f6740";

using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Add("apiKey",APIKey);
    HttpResponseMessage response = client.GetAsync(BaseURL + "/public-api/v1/auth/token").Result;
    var responseBody = response.Content.ReadAsStringAsync().Result;
    if (response.IsSuccessStatusCode)
    {
        var rawdata = JObject.Parse(responseBody);
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + rawdata["accessToken"]?.ToString());
        client.DefaultRequestHeaders.Remove("apiKey");
        Console.WriteLine("Access Token succesfully received " + rawdata["accessToken"]?.ToString());
    }
    else                {
        Console.WriteLine("Error during authentification");
        Console.WriteLine(responseBody.ToString());
        return;
    }

    response = client.GetAsync(BaseURL + "/public-api/v1/box").Result;
    responseBody = response.Content.ReadAsStringAsync().Result;

    if (response.IsSuccessStatusCode)
    {
        JArray rawlist = JArray.Parse(responseBody);
        Console.WriteLine($"Found {rawlist.Count} boxes");
        foreach(var row in rawlist)
            Console.WriteLine($"{row["name"]} - {row["boxID"]} - {row["runtimeVersion"]} - {row["state"]}");
    }
    else
        Console.WriteLine("Error during call -> " + response.StatusCode + response.ReasonPhrase);

    Console.WriteLine("Finished, yay!");                    
}


