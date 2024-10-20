import asyncio
import torch
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from fastapi.responses import FileResponse
from fastapi.staticfiles import StaticFiles
from fastapi import BackgroundTasks
import json
from transformer_model import remove_punc, Dataset, MultiHeadAttention, FeedForward, EncoderLayer, DecoderLayer, Embeddings, Transformer, AdamWarmup, LossWithLS, evaluate
from Ninobayot.intent_recognition import give_intent
from Ninobayot.playerimpression import get_player_impression, set_player_impression
from datetime import datetime, timedelta
from pydantic import BaseModel

# Loading the model
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
checkpoint = torch.load('checkpoint_499.pth.tar', map_location=torch.device('cpu'))
transformer = checkpoint['transformer']

#Storing html activity
html_active = "inactive"
html_status = "offline"

#Storing location
npc_name = "null"
location =  {}
distance = 0.0

with open('WORDMAP_corpus.json', 'r') as j:
    word_map = json.load(j)

# Initialize FastAPI app
app = FastAPI()

origins = [
    "http://localhost",
    "http://localhost:8000",
    "http://127.0.0.1:8000",
    "http://127.0.0.1",
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Allows all origins
    allow_credentials=True,
    allow_methods=["*"],  # Allows all methods
    allow_headers=["*"],  # Allows all headers
)

class Text_Request(BaseModel):
    question: str

class Text_Response(BaseModel):
    intent: str
    answer: str
    PI: str

class NPCLocRequest(BaseModel): #Location Request
    npcName: str
    location: dict
    distanceToTarget: float

class NPCLocGet(BaseModel):
    distance: float

class HTML_Check_Status(BaseModel):
    message: str
    html_active: str
    html_status: str  # Assuming this field exists based on the error message


@app.post("/npc_location")
def npc_location(request: NPCLocRequest):
    global distance
    
    print(request) #checking errors
    

    npc_name = request.npcName
    location = request.location
    distance = request.distanceToTarget

    print(f"Received location for {npc_name}: {location} with distance {distance}")
    return {"message": "Location received", "npcName": npc_name, "location": location, "distance": distance}

#Function to receive location to HTML
@app.post("/get_npc_location", response_model = NPCLocGet) #getting location
def get_npc_location(data: dict):

    global distance

    print(f"HTML Received location with distance {distance}")
    
    return NPCLocGet(distance = distance)

@app.post("/generate_text", response_model=Text_Response)
def generate_text(request: Text_Request):
    question = remove_punc(request.question)

    intent = give_intent(question)

    set_player_impression(intent)
    PI = get_player_impression()

    max_len = 200
    enc_qus = [word_map.get(word, word_map['<unk>']) for word in question.split()]
    question = torch.LongTensor(enc_qus).to(device).unsqueeze(0)
    question_mask = (question != 0).to(device).unsqueeze(1).unsqueeze(1)
    sentence = evaluate(transformer, question, question_mask, int(max_len), word_map)
    
    return Text_Response(answer=sentence, intent=intent, PI = PI)

# Serve the static HTML file
@app.get("/", response_class=FileResponse)
def read_index():
    return "static/main.html"

@app.post("/page_opened", response_model=HTML_Check_Status)
def page_opened(data: dict, background_tasks: BackgroundTasks):
    global html_active
    global html_status
    global last_status_change_time

    status = data.get("status")

    if status == "page_opened":
        # Update status and last change time
        html_active = "active"
        html_status = "online"
        last_status_change_time = datetime.now()
        
        # Start a background task to check for inactivity
        background_tasks.add_task(check_status_inactivity)
        return HTML_Check_Status(message="webpage active", html_active=html_active, html_status=html_status)

    elif status == "check":
        # Keep the status active and update last change time
        return HTML_Check_Status(message="webpage check", html_active=html_active, html_status=html_status)

    # If the status is not recognized, set to inactive
    html_active = "inactive"
    html_status = "offline"
    last_status_change_time = datetime.now()  # Update time when setting to inactive
    return HTML_Check_Status(message="webpage inactive", html_active=html_active, html_status=html_status)

# Background task to check inactivity
async def check_status_inactivity():
    global html_active
    global html_status
    global last_status_change_time

    while True:
        # Check if the time since the last status change exceeds the timeout
        if datetime.now() - last_status_change_time > timedelta(seconds=10):  # Set timeout (10 seconds)
            html_active = "inactive"
            html_status = "offline"
            break

        await asyncio.sleep(1)

# Mount the static files directory
app.mount("/static", StaticFiles(directory="static"), name="static")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
