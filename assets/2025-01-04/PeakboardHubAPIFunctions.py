import requests
import sys
import pandas

BaseURL = "https://api.peakboard.com";
APIKey = "87eee08b31f0e3b2549c80cf22a6636b66d5066a";

response = requests.get(BaseURL + "/public-api/v1/auth/token", headers={'apiKey': APIKey})

if response.status_code != 200:
    sys.exit("Unable to obtain a access token, so quitting") 

accesstoken = response.json()["accessToken"]
print("Succesfully authorized until " + response.json()["validUntill"])

mySession = requests.Session()
mySession.headers.update({"Authorization": "Bearer " + accesstoken})


# Get all functions of a box

response = mySession.get(BaseURL + "/public-api/v1/box/functions?boxId=PB0000PT")

if response.status_code != 200:
    sys.exit("Unable to obtain the functions of a box")

print(response.json())

for item in response.json():
    print(f"Function found: {item['Name']}")

# Call a function

body = {
  "boxId": "PB0000PT",
  "functionName": "SubmitAlarm",
  "parameters": [
    {
      "name": "AlarmTime",
      "value": 10
    },
    {
      "name": "AlarmMessage",
      "value": "We have a serious problem here"
    }
  ]
}

response = mySession.post(BaseURL + "/public-api/v1/box/function", json=body)

if response.status_code != 200:
    sys.exit("Unable to call the function. Return is " + response.reason)

print("Alarm set succesfully....")

## Call a function with return value

body = {
  "boxId": "PB0000PT",
  "functionName": "IsAlarmActive"
}

response = mySession.post(BaseURL + "/public-api/v1/box/function", json=body)

if response.status_code != 200:
    sys.exit("Unable to call the function. Return is " + response.reason)

print(f"Is alarm set? -> {response.text}")
