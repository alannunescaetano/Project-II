from http.server import BaseHTTPRequestHandler, HTTPServer
import numpy as np
import base64
from ocr.prediction.prediction import Prediction
import json
from syllables import Syllables

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

        decodedImage = base64.b64decode(base64Image)

        nparr = np.fromstring(decodedImage, np.uint8)

        word = Prediction.predict(nparr)
        print("Predicted: "+word)

        predictionResult = PredictionResult()
        predictionResult.Syllables = Syllables.getSyllables(word)

        print(json.dumps(predictionResult.__dict__).encode())

        self._set_headers()
        self.wfile.write(json.dumps(predictionResult.__dict__).encode())

class PredictionResult:
    def __init__(self):
        self.Syllables = []

def run():
    server_address = ('192.168.1.18', 8088)
    httpd = HTTPServer(server_address, RequestHandler)
    httpd.serve_forever()


run()
