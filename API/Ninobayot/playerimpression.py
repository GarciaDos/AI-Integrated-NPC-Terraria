Player_Impression = "none"
PI_points = 0

def get_player_impression():
    global PI_points  # Declare PI_points as global to access the global variable
    global Player_Impression  # Declare Player_Impression as global

    if -4 <= PI_points <= -2:
        Player_Impression = "negative"
    elif -1 <= PI_points <= 1:
        Player_Impression = "neutral"
    elif 2 <= PI_points <= 4:
        Player_Impression = "positive"
    else:
        Player_Impression = "wtf"

    return Player_Impression 

def set_player_impression(intent):
    global PI_points  # Declare PI_points as global

    if intent == "positive":
        if PI_points < 4:  # Increment if not at max
            PI_points += 1
    elif intent == "negative":
        if PI_points > -4:  # Decrement if not at min
            PI_points -= 1