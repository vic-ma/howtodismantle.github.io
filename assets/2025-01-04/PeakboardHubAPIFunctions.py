import requests
import sys
import pandas

BaseURL = "http://20.218.250.53:17545";
APIKey = "CLPD7jq1le5I6E61WZ3dGRzH961Eqprkq26qJDY0";

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

for item in response.json():
    print(f"Table found: {item['Name']}")

# Call a function

body = {
    "listName": "stockinfo",
    "data": {
        "MaterialNo": "0815",
        "Quantity": 5,
        "locked": False 
    }
}

response = mySession.post(BaseURL + "/public-api/v1/lists/items", json=body)

if response.status_code != 200:
    sys.exit("Unable to create a new record. Return is " + response.reason)

id = response.json()["addedItem"][0]["value"]

print("New record added under ID " + str(id))
