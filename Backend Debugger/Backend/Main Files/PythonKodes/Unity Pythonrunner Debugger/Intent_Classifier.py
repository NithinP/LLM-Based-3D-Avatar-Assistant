from transformers import AutoTokenizer, AutoModelForSequenceClassification, TextClassificationPipeline
import UnityEngine
def classify(input_text):

    UnityEngine.Debug.Log(f'Received input text: {input_text}')
    model_name = 'qanastek/XLMRoberta-Alexa-Intents-Classification'
    tokenizer = AutoTokenizer.from_pretrained(model_name)
    model = AutoModelForSequenceClassification.from_pretrained(model_name)
    classifier = TextClassificationPipeline(model=model, tokenizer=tokenizer)

    res = classifier(input_text)
    UnityEngine.Debug.Log(res[0]['label'])

    return res[0]['label']

if __name__ == "__main__":
    classify(" + $"\"{inputText}\"" + @")
