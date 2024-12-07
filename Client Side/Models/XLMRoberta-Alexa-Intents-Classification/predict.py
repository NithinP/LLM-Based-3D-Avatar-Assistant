from transformers import AutoTokenizer, AutoModelForSequenceClassification, TextClassificationPipeline

model_name = 'qanastek/XLMRoberta-Alexa-Intents-Classification'
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForSequenceClassification.from_pretrained(model_name)
classifier = TextClassificationPipeline(model=model, tokenizer=tokenizer)

res = classifier("réveille-moi à neuf heures du matin le vendredi")
print(res)
