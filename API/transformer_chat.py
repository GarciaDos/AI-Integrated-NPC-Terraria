import torch
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import json
from transformer_model import Dataset, MultiHeadAttention, FeedForward, EncoderLayer, DecoderLayer, Embeddings, Transformer, AdamWarmup, LossWithLS, evaluate

# Loading the model
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
checkpoint = torch.load('checkpoint_499.pth.tar', map_location=torch.device('cpu'))
transformer = checkpoint['transformer']


with open('WORDMAP_corpus.json', 'r') as j:
    word_map = json.load(j)

# Initialize FastAPI app
app = FastAPI()

class Text_Request(BaseModel):
    question: str

class Text_Response(BaseModel):
    answer: str



@app.post("/generate_text", response_model=Text_Response)
def generate_text(request: Text_Request):
    question = request.question

    max_len = 200
    enc_qus = [word_map.get(word, word_map['<unk>']) for word in question.split()]
    question = torch.LongTensor(enc_qus).to(device).unsqueeze(0)
    question_mask = (question!=0).to(device).unsqueeze(1).unsqueeze(1)
    sentence = evaluate(transformer, question, question_mask, int(max_len), word_map)
    
    return Text_Response(answer=sentence)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)



