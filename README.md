# AI-Integrated-NPC-Terraria
* For our Thesis

# Setup:

## Requirements:
-Steam
-Terraria
-tModloader
-.NET Core
-Python

## Required Mods (Install it through the tModLoader workshop or Steam workshop of tModLoader):
-Hero's Mod (https://steamcommunity.com/sharedfiles/filedetails/?id=2564645933&searchtext=Hero%27s+MOD)

-Cheat Sheet (https://steamcommunity.com/sharedfiles/filedetails/?id=2563784437&searchtext=cheat+sheet)

## Not needed but recommended to have installed:
-Visual Studio Code


## Intalling the API:
1. Install the python libraries for API (prefered to be installed in a venv): 
	pip install torch fastapi pydantic uvicorn numpy
2. Download the Model checkpoint from Mega, and place it alongside the transformer_chat.py, transformer_model.py, and WORDMAP_corpus.json
   	(https://mega.nz/file/pvdlzRTA#wAtKnwawnQro5KU_P1XoDeiNaQ45bHlCQSWMpSuYK14)
   ![image](https://github.com/user-attachments/assets/b8293a16-898c-46a0-80f7-200e76ea26c2)

4. Run the API:
	python transformer_chat.py

![image](https://github.com/user-attachments/assets/e708b788-b5be-4e73-86e8-9fc379527db5)


- Once the API is running the web-ui should be accessible when the NPC's chat button is clicked

## Installing the Modded NPC in Terraria:
1. To install the mod put the NPCTry folder in the directory:
	Documents\My Games\Terraria\tModLoader\ModSources
2. Once the folder is in the specified directory, open tModLoader then go to the following:
	Workshop > Develop Mods
3. The mod folder NPCTry should appear, click Build + Reload, this should install the modded NPC in the game
4. To spawn the NPC, click the left arrow menu at the bottom of the screen.

![image](https://github.com/user-attachments/assets/f6de83f9-1ea3-4c06-8ebd-4c0042e593de)


5. In the menus click the green slime button,  to show the NPC Browser.

![image](https://github.com/user-attachments/assets/7efd376d-51cf-4421-9722-7c4e7624c3c8)


7. Search "Antithesis", click it to spawn the NPC.

![image](https://github.com/user-attachments/assets/16c1ed9d-2493-4fc9-be11-5beb7a428528)


## Interacting with the NPC:
- The NPC has four buttons as of now: 
	- "Shop" - Shows the item shop of the NPC
	- "Close" - Closes the menu box
	- "CHAT" - Opens the Web-UI through the steam overlay (Make sure steam overlay is enabled)
	- "Happiness" - Current mood (Not fully functional yet)



# NPC Feature/Info
* Name: Ophelia - The name Ophelia comes from the Greek opheleia, meaning “advantage” or “help”.
* Age: 20
* Sex: Female
* Characteristic: Knowledgeable in Terraria Mechanics
* Appearance: Goth
* Lore: Daughter of Cthulu (Moon Lord) (shhhhhhhhh)
