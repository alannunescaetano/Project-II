import pytesseract as tess
tess.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
from PIL import Image

img = Image.open('placa-perigo.png')
text = tess.image_to_string(img)

bounds = tess.image_to_boxes(img)

print(text)
print("--------------------")
print(bounds)