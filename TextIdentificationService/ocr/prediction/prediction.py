# import the necessary packages
import cv2
import numpy as np
import imutils
from imutils.contours import sort_contours
from cv2 import cv2
from keras.models import load_model
import json

class PredictionResult:
    def __init__(self):
        self.chars = []
 
class Prediction:
    @staticmethod
    def predict():
        model_path = r"C:\Projetos\Mestrado\Project II\SourceCode\TextIdentificationService\model\trained_ocr_only_capital.model"
        print("[INFO] loading OCR model...")
        model = load_model(model_path)
        print(model_path)

        image_path = r"C:\Projetos\Mestrado\Project II\SourceCode\TextIdentificationService\images\.PNG"
        image = cv2.imread(image_path)
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        blurred = cv2.GaussianBlur(gray, (5, 5), 0)

        edged = cv2.Canny(blurred, 30, 150)

        cnts = cv2.findContours(edged.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        cnts = imutils.grab_contours(cnts)
        cnts = sort_contours(cnts, method="left-to-right")[0]

        chars = []

        for c in cnts:
            (x, y, w, h) = cv2.boundingRect(c)

            if (w >= 20 and w <= 150) and (h >= 15 and h <= 120):
                roi = gray[y:y + h, x:x + w]
                thresh = cv2.threshold(roi, 0, 255,
                    cv2.THRESH_BINARY | cv2.THRESH_OTSU)[1]
                (tH, tW) = thresh.shape

                if tW > tH:
                    thresh = imutils.resize(thresh, width=32)
                else:
                    thresh = imutils.resize(thresh, height=32)

                (tH, tW) = thresh.shape
                dX = int(max(0, 32 - tW) / 2.0)
                dY = int(max(0, 32 - tH) / 2.0)

                padded = cv2.copyMakeBorder(thresh, top=dY, bottom=dY,
                    left=dX, right=dX, borderType=cv2.BORDER_CONSTANT,
                    value=(255, 255, 255))
                
                padded = cv2.resize(padded, (32, 32))
                padded = padded.astype("float32") / 255.0
                padded = np.expand_dims(padded, axis=-1)

                chars.append((padded, (x, y, w, h)))

        boxes = [b[1] for b in chars]
        chars = np.array([c[0] for c in chars], dtype="float32")

        preds = model.predict(chars)

        labelNames = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        labelNames = [l for l in labelNames]

        predictionResult = PredictionResult()

        for (pred, (x, y, w, h)) in zip(preds, boxes):
            i = np.argmax(pred)
            prob = pred[i]
            print("prob:",prob)
            if prob > 0.8:
                print(labelNames[i])
                predictionResult.chars.append(labelNames[i])

        return predictionResult
