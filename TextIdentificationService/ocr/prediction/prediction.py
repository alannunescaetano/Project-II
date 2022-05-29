import numpy as np
import imutils
import tensorflow as tf
from imutils.contours import sort_contours
from cv2 import cv2
from keras.models import load_model
 
class Prediction:
    @staticmethod
    def predict(image):
        tf.config.run_functions_eagerly(True)
        model_path = r"C:\Projetos\Mestrado\Project II\SourceCode\TextIdentificationService\model\trained_ocr_full.model"
        print("[INFO] loading OCR model...")
        model = load_model(model_path)

        gray = cv2.imdecode(image, cv2.IMREAD_GRAYSCALE)
        (thresh, bwImage) = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

        width, height = bwImage.shape
        totalpixels = width * height

        if cv2.countNonZero(bwImage) < totalpixels / 2:
            bwImage = np.invert(bwImage)

        blurred = cv2.GaussianBlur(bwImage, (5, 5), 0)
        edged = cv2.Canny(blurred, 30, 150)

        cnts = cv2.findContours(edged.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        cnts = imutils.grab_contours(cnts)
        cnts = sort_contours(cnts, method="left-to-right")[0]

        chars = []

        for c in cnts:
            (x, y, w, h) = cv2.boundingRect(c)

            if (w >= 5 and w <= 150) and (h >= 15 and h <= 120):
                cut = bwImage[y:y + h, x:x + w]

                (tH, tW) = cut.shape
                if tW > tH:
                    cut = imutils.resize(cut, width=32)
                else:
                    cut = imutils.resize(cut, height=32)

                (tH, tW) = cut.shape
                dX = int(max(0, 32 - tW) / 2.0)
                dY = int(max(0, 32 - tH) / 2.0)

                padded = cv2.copyMakeBorder(cut, top=dY, bottom=dY,
                    left=dX, right=dX, borderType=cv2.BORDER_CONSTANT,
                    value=(255, 255, 255))
                
                padded = cv2.resize(padded, (32, 32))
                padded = padded.astype("float32") / 255.0
                padded = np.expand_dims(padded, axis=-1)

                #cv2.imshow('teste', padded)
                #cv2.waitKey(0)

                chars.append((padded, (x, y, w, h)))

        boxes = [b[1] for b in chars]
        chars = np.array([c[0] for c in chars], dtype="float32")

        preds = model.predict(chars)

        labelNames = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
        labelNames = [l for l in labelNames]

        prediction = ""

        for (pred, (x, y, w, h)) in zip(preds, boxes):
            i = np.argmax(pred)
            prob = pred[i]

            if prob > 0.5:
                prediction += labelNames[i]

        return prediction.lower()
