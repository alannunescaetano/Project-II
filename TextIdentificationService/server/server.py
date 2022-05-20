from http.server import BaseHTTPRequestHandler, HTTPServer
import cv2
import numpy as np
import base64
from ocr.prediction.prediction import Prediction, PredictionResult
import json
from PIL import Image

class RequestHandler(BaseHTTPRequestHandler):
    def _set_headers(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()
    
    def do_POST(self):

        print('Received request')
        content_len = int(self.headers.get('Content-Length'))
        body = self.rfile.read(content_len)

        dict = json.loads(body)
        base64Image = dict["Data"]

        print(base64Image)

        decodedImage = base64.b64decode(base64Image)

        nparr = np.fromstring(decodedImage, np.uint8)
        decodedImage = cv2.imdecode(nparr, cv2.COLOR_BGR2GRAY)

        gray = cv2.cvtColor(decodedImage, cv2.COLOR_BGR2GRAY)
        blurred = cv2.GaussianBlur(gray, (5, 5), 0)
        (thresh, bwImage) = cv2.threshold(blurred, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

        if(cv2.countNonZero(bwImage)):
            

        cv2.imshow('img', finalImage)
        cv2.waitKey(0)

        #predictionResult = Prediction.predict(decodedImage)
        #print(json.dumps(predictionResult.__dict__).encode())

        self._set_headers()
        #self.wfile.write(json.dumps(predictionResult.__dict__).encode())
        #self.wfile.write(b'Ok')

        predictionResult = PredictionResult()
        predictionResult.Syllables.append("BA")
        predictionResult.Syllables.append("NA")
        predictionResult.Syllables.append("NA")

        self.wfile.write(json.dumps(predictionResult.__dict__).encode())


def run():
    server_address = ('10.72.252.23', 8088)
    httpd = HTTPServer(server_address, RequestHandler)
    httpd.serve_forever()


run()
