import pandas as pd
from collections import defaultdict
import math
import nltk

train_df = pd.read_csv("train.csv", encoding='ISO-8859-1')
new_data_df = pd.read_csv("test.csv", encoding='ISO-8859-1')  
combined_df = pd.concat([train_df, new_data_df], ignore_index=True)
df = combined_df[combined_df['text'].notna()]
train_df = combined_df[combined_df['text'].notna()]

def split_data_by_sentiment(data, sentiment):
    """
    Split the data DataFrame into separate lists based on sentiment.

    Parameters:
       data (DataFrame): The input DataFrame containing 'text' and 'sentiment' columns.
       sentiment (str): The sentiment label to filter the data.

    Returns:
        list: A list of text corresponding to the specified sentiment.
    """
    return data[data['sentiment'] == sentiment]['text'].tolist()

# Assuming df is your DataFrame containing 'text' and 'sentiment' columns
positive_data = split_data_by_sentiment(df, 'positive')
negative_data = split_data_by_sentiment(df, 'negative')
neutral_data = split_data_by_sentiment(df, 'neutral')

def preprocess_tweet(tweet):
    # Convert the tweet to lowercase
    tweet = tweet.lower()
    
    # Remove punctuation from the tweet using translation
    tweet = tweet.translate(str.maketrans("", "", string.punctuation))
    
    # Tokenize the tweet into individual words
    tokens = nltk.word_tokenize(tweet)
    
    # Initialize a Porter stemmer for word stemming
    stemmer = PorterStemmer()
    
    # Get a set of English stopwords from NLTK
    stopwords_set = set(stopwords.words("english"))
    
    # Apply stemming to each token and filter out stopwords
    tokens = [stemmer.stem(token) for token in tokens if token not in stopwords_set]
    
    # Return the preprocessed tokens
    return tokens

def preprocess_tweet(tweet):
    if isinstance(tweet, str):  # Check if tweet is a string
        return tweet.lower().split()  # Simple tokenization for illustration
        return []  # Return an empty list for non-string entries

def calculate_word_counts(tweets):
    # Initialize a defaultdict to store word counts
    word_count = defaultdict(int)
    
    # Iterate through each tweet
    for tweet in tweets:
        # Tokenize and preprocess the tweet
        tokens = preprocess_tweet(tweet)
        
        # Increment the count for each token
        for token in tokens:
            word_count[token] += 1
    
    return word_count

def calculate_likelihood(word_count, total_words, laplacian_smoothing=1):
    # Create an empty dictionary to store the likelihood values
    likelihood = {}
    
    # Get the number of unique words in the vocabulary
    vocabulary_size = len(word_count)

    # Iterate through each word and its corresponding count in the word_count dictionary
    for word, count in word_count.items():
        # Calculate the likelihood using Laplacian smoothing formula
        # Laplacian smoothing is used to handle unseen words in training data
        # The formula is (count + smoothing) / (total_words + smoothing * vocabulary_size)
        likelihood[word] = (count + laplacian_smoothing) / (total_words + laplacian_smoothing * vocabulary_size)

    # Return the calculated likelihood dictionary
    return likelihood

def calculate_log_prior(sentiment, data):
    # Calculate the natural logarithm of the ratio of tweets with the specified sentiment to the total number of tweets
    log_prior = math.log(len(data[data['sentiment'] == sentiment]) / len(data))
    
    # Return the calculated log prior
    return log_prior

def classify_tweet_with_scores(tweet, log_likelihood_positive, log_likelihood_negative, log_likelihood_neutral,
                               log_prior_positive, log_prior_negative, log_prior_neutral):
    # Tokenize and preprocess the input tweet
    tokens = preprocess_tweet(tweet)

    # Calculate the log scores for each sentiment category
    log_score_positive = log_prior_positive + sum([log_likelihood_positive.get(token, 0) for token in tokens])
    log_score_negative = log_prior_negative + sum([log_likelihood_negative.get(token, 0) for token in tokens])
    log_score_neutral = log_prior_neutral + sum([log_likelihood_neutral.get(token, 0) for token in tokens])

    # Store the sentiment scores in a dictionary
    sentiment_scores = {
        'positive': log_score_positive,
        'negative': log_score_negative,
        'neutral': log_score_neutral
    }

    # Determine the predicted sentiment based on the highest sentiment score
    predicted_sentiment = max(sentiment_scores, key=sentiment_scores.get)
    
    # Return the predicted sentiment and the sentiment scores
    return predicted_sentiment, sentiment_scores

positive_data = split_data_by_sentiment(train_df, 'positive')
negative_data = split_data_by_sentiment(train_df, 'negative')
neutral_data = split_data_by_sentiment(train_df, 'neutral')

word_count_positive = calculate_word_counts(positive_data)
word_count_negative = calculate_word_counts(negative_data)
word_count_neutral = calculate_word_counts(neutral_data)

total_positive = sum(word_count_positive.values())
total_negative = sum(word_count_negative.values())
total_neutral = sum(word_count_neutral.values())

likelihood_positive = calculate_likelihood(word_count_positive, total_positive)
likelihood_negative = calculate_likelihood(word_count_negative, total_negative)
likelihood_neutral = calculate_likelihood(word_count_neutral, total_neutral)

log_likelihood_positive = {word: math.log(prob) for word, prob in likelihood_positive.items()}
log_likelihood_negative = {word: math.log(prob) for word, prob in likelihood_negative.items()}
log_likelihood_neutral = {word: math.log(prob) for word, prob in likelihood_neutral.items()}

log_prior_positive = calculate_log_prior('positive', train_df)
log_prior_negative = calculate_log_prior('negative', train_df)
log_prior_neutral = calculate_log_prior('neutral', train_df)

def give_intent(text):
    test_tweet = text
    predicted_sentiment, sentiment_scores = classify_tweet_with_scores(
        test_tweet, 
        log_likelihood_positive, log_likelihood_negative, log_likelihood_neutral,
        log_prior_positive, log_prior_negative, log_prior_neutral
    )

    return predicted_sentiment 

#Other functions





