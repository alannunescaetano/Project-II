from http.server import BaseHTTPRequestHandler, HTTPServer

import simplejson as simplejson

from ocr.prediction.prediction import Prediction
import json

class RequestHandler(BaseHTTPRequestHandler):
    def _set_headers(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()
    
    def do_POST(self):

        print('Received request')
        content_len = int(self.headers.get('Content-Length'))
        body = self.rfile.read(content_len)

        print(body)

        #predictionResult = Prediction.predict()

        self._set_headers()
        #self.wfile.write(json.dumps(predictionResult.__dict__).encode())
        self.wfile.write(b'Ok')

        #print(json.dumps(predictionResult.__dict__).encode())

def run():
    server_address = ('192.168.1.22', 8088)
    httpd = HTTPServer(server_address, RequestHandler)
    httpd.serve_forever()

run()
