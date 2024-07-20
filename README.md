# AI-Integrated-NPC-Terraria
* For our Thesis

# Setup:

## Requirements:
-Steam
-Terraria
-tModloader
-.NET Core
-Python

## Required Mods (Install it through the tModLoader workshop or Steam worlshop of tModLoader):
-Hero's Mod
-Cheat Sheet

## Not needed but recommended to have installed:
-Visual Studio Code


## Intallling the API:
1. Install the python libraries for API (prefered to be installed in a venv): 
	pip install torch fastapi pydantic uvicorn numpy
2. Run the API:
	python transformer_chat.py

- Once the API is running the web-ui should be accessible when the NPC's chat button is clicked

## Installing the Modded NPC in Terraria:
1. To install the mod put the NPCTry folder in the directory:
	Documents\My Games\Terraria\tModLoader\ModSources
2. Once the folder is in the specified directory, open tModLoader then go to the following:
	Workshop > Develop Mods
3. The mod folder NPCTry should appear, click Build + Reload, this should install the modded NPC in the game
4. To spawn the NPC, click the left arrow menu at the bottom of the screen.
5. In the menus click the green slime button,  to show the NPC Browser.
6. Search "Antithesis", click it to spawn the NPC.

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
