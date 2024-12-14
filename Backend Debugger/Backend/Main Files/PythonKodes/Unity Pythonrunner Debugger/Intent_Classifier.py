import sys
import time
from transformers import AutoTokenizer, AutoModelForSequenceClassification, TextClassificationPipeline

def initialize_classifier():
    model_name = 'qanastek/XLMRoberta-Alexa-Intents-Classification'
    tokenizer = AutoTokenizer.from_pretrained(model_name)
    model = AutoModelForSequenceClassification.from_pretrained(model_name)
    return TextClassificationPipeline(model=model, tokenizer=tokenizer)

def main():
    classifier = initialize_classifier()
    running = True
    
    while running:
        try:
            user_input = input("Enter a command (or 'quit' to exit): ")
            if user_input.lower() == 'quit':
                running = False
                break
            res = classifier(user_input)
            print(f"Input: {user_input}")
            print(f"Intent Classification: {res}")
            time.sleep(0.1)
        
        except Exception as e:
            print(f"An error occurred: {e}")
    
    print("Intent classification loop ended.")

if __name__ == "__main__":
    main()


#import sys
#import os
#import traceback
#from transformers import AutoTokenizer, AutoModelForSequenceClassification, TextClassificationPipeline
#import UnityEngine

#def normalize_path(path):
#    """Converts path to use consistent separators."""
#    return os.path.normpath(path)

#def initialize_classifier():
#    try:
#        # Move two levels up from 'Assets' and then into the 'models' directory
#        project_root = os.path.dirname(os.path.dirname(UnityEngine.Application.dataPath))
#        classification_folder_path = os.path.join(project_root, 'models', 'XLMRoberta-Alexa-Intents-Classification')
#        classification_folder_path = normalize_path(classification_folder_path)

#        UnityEngine.Debug.Log(f'Loading from path: {classification_folder_path}')

#        # Load tokenizer and model from the specified folder
#        tokenizer = AutoTokenizer.from_pretrained(classification_folder_path,local_files_only=True)
#        model = AutoModelForSequenceClassification.from_pretrained(classification_folder_path)

#        # Return the classification pipeline
#        return TextClassificationPipeline(model = model, tokenizer = tokenizer)

#    except Exception as init_error:
#        UnityEngine.Debug.LogError(f'Classifier initialization error: {str(init_error)}')
#        raise


#try:
#    input_text = "Hello"
#    if not input_text or input_text.strip() == '':
#        raise ValueError('Input text cannot be empty')

#    # Initialize the classifier (only once)
#    classifier = initialize_classifier()

#    # Perform classification
#    UnityEngine.Debug.Log(f'Classifying text: {input_text}')
#    result = classifier(input_text)
#    if not result:
#        raise ValueError('Classification failed or returned no results')

#    intent_label = result[0]['label']
#    confidence = result[0]['score']

#    # Format and log the output
#    output_message = f'[INTENT_OUTPUT] Label: {intent_label}, Confidence: {confidence:.4f}'
#    UnityEngine.Debug.Log(output_message)

#except Exception as e:
#    # Log detailed error information
#    error_msg = f'Classification error: {str(e)}\n{traceback.format_exc()}'
#    UnityEngine.Debug.LogError(f'[INTENT_ERROR] {error_msg}')

## Ensure the standard output is flushed for Unity compatibility
#sys.stdout.flush()
