import requests
import sys
import pandas

BaseURL = "http://20.218.250.53:17545";
APIKey = "833a288049e1efe5527f10d81332746ccf484857";

response = requests.get(BaseURL + "/public-api/v1/auth/token", headers={'apiKey': APIKey})

if response.status_code != 200:
    sys.exit("Unable to obtain a access token, so quitting") 

accesstoken = response.json()["accessToken"]
print("Succesfully authorized until " + response.json()["validUntill"])

mySession = requests.Session()
mySession.headers.update({"Authorization": "Bearer " + accesstoken})


# Get all lists

response = mySession.get(BaseURL + "/public-api/v1/lists")

if response.status_code != 200:
    sys.exit("Unable to obtain list information")

for item in response.json():
    print(f"Table found: {item['name']}")

# Get data of a list

response = mySession.get(BaseURL + "/public-api/v1/lists/list?Name=stockinfo&SortColumn=MaterialNo&SortOrder=Asc")

if response.status_code != 200:
    sys.exit("Unable to obtain list data")


columns = [col['name'] for col in response.json()['columns']]
items = [{entry['column']: entry['value'] for entry in item} for item in response.json()['items']]

print(pandas.DataFrame(items, columns=columns))

# create a new record

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

# Edit a record

body = {
  "rowId": id,
  "listName": "stockinfo",
  "data": {
    "Quantity": 30
  }
}

response = mySession.put(BaseURL + "/public-api/v1/lists/items", json=body)

if response.status_code != 200:
    sys.exit("Unable to change a record. Return is " + response.reason)

print(response.json())
print(f"Record with id {id} was changed")

# delete a record

body = {
  "listName": "stockinfo",
  "rowId": id
}

response = mySession.delete(BaseURL + "/public-api/v1/lists/items", json=body)

if response.status_code != 200:
    sys.exit("Unable to delete a record. Return is " + response.reason)

print(f"Record with id {id} was deleted")

# Get table data with the help of SQL command

body = {
  "sql": "select Locked, count(*) as Counter from stockinfo group by locked"
}
response = mySession.post(BaseURL + "/public-api/v1/lists/list", json=body)

if response.status_code != 200:
    sys.exit("Unable to obtain list data")

columns = [col['name'] for col in response.json()['columns']]
items = [{entry['column']: entry['value'] for entry in item} for item in response.json()['items']]

print(pandas.DataFrame(items, columns=columns))



