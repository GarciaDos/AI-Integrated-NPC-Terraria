import torch
from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from fastapi.responses import FileResponse
from fastapi.staticfiles import StaticFiles
import json
from transformer_model import remove_punc, Dataset, MultiHeadAttention, FeedForward, EncoderLayer, DecoderLayer, Embeddings, Transformer, AdamWarmup, LossWithLS, evaluate
from Ninobayot.intent_recognition import give_intent

# Loading the model
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
checkpoint = torch.load('checkpoint_499.pth.tar', map_location=torch.device('cpu'))
transformer = checkpoint['transformer']

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

@app.post("/generate_text", response_model=Text_Response)
def generate_text(request: Text_Request):
    question = remove_punc(request.question)

    intent = give_intent(question)

    max_len = 200
    enc_qus = [word_map.get(word, word_map['<unk>']) for word in question.split()]
    question = torch.LongTensor(enc_qus).to(device).unsqueeze(0)
    question_mask = (question != 0).to(device).unsqueeze(1).unsqueeze(1)
    sentence = evaluate(transformer, question, question_mask, int(max_len), word_map)
    
    return Text_Response(answer=sentence, intent=intent)

# Serve the static HTML file
@app.get("/", response_class=FileResponse)
def read_index():
    return "static/main.html"

# Mount the static files directory
app.mount("/static", StaticFiles(directory="static"), name="static")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
