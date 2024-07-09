import requests
import json

url = "http://localhost:8000/generate_text"
payload = {
    "question": "Hello"
}

response = requests.post(url, json=payload)
response_data = response.json()
answer = response_data.get('answer')    
print(answer)




