import requests

# 1. Configuration
TENANT_ID = 'XXX'
CLIENT_ID = 'XXX'
CLIENT_SECRET = 'XXX'
ENVIRONMENT = 'production' # or 'production'
COMPANY_ID = '435b8205-7cd0-f011-8bce-6045bdc89f91' # Find this via the /companies endpoint

# 2. Get Access Token
token_url = f"https://login.microsoftonline.com/{TENANT_ID}/oauth2/v2.0/token"
token_data = {
    'grant_type': 'client_credentials',
    'client_id': CLIENT_ID,
    'client_secret': CLIENT_SECRET,
    'scope': 'https://api.businesscentral.dynamics.com/.default'
}

token_res = requests.post(token_url, data=token_data)
access_token = token_res.json().get('access_token')
# print("Access Token:", access_token)

# 4. List Companies
companies_url = f"https://api.businesscentral.dynamics.com/v2.0/{ENVIRONMENT}/api/v2.0/companies"
companies_response = requests.get(companies_url, headers=headers)

if companies_response.status_code == 200:
    print("Companies:", companies_response.json())
else:
    print(f"Error listing companies: {companies_response.status_code} - {companies_response.text}")

# 5. Fetch Data (Example: Customers)
api_url = f"https://api.businesscentral.dynamics.com/v2.0/{ENVIRONMENT}/api/v2.0/companies({COMPANY_ID})/customers"
headers = {
    'Authorization': f'Bearer {access_token}',
    'Content-Type': 'application/json'
}

response = requests.get(api_url, headers=headers)

if response.status_code == 200:
    print(response.json())
else:
    print(f"Error: {response.status_code} - {response.text}")