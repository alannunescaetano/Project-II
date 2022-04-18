# set the matplotlib backend so figures can be saved in the background
import cv2
import numpy as np
from tensorflow.keras.datasets import mnist
import matplotlib.pyplot as plt
from sklearn.metrics import classification_report
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelBinarizer
from tensorflow.keras.optimizers import SGD
from imutils import build_montages

from models import ResNet
# from typing_extensions import Required
import matplotlib
matplotlib.use("Agg")

# initialize the number of epochs to train for, initial learning rate,
# and batch size
EPOCHS = 1
INIT_LR = 1e-1
BS = 128

def load_az_dataset(dataset_path):
    # initialize the list of data and labels
    data = []
    labels = []

    # loop over the rows of the A-Z handwritten digit dataset
    file = open(dataset_path, "r")
    print(file)

    for row in open(dataset_path, "r"):
        # parse the label and image from the row
        split_row = row.split(",")
        label = int(split_row[0])
        image = np.array([int(x) for x in split_row[1:]], dtype="uint8")

        # images are represented as single channel (grayscale) images
        # that are 28x28=784 pixels -- we need to take this flattened
        # 784-d list of numbers and reshape them into a 28x28 matrix
        image = image.reshape((28, 28))

        # update the list of data and labels
        data.append(image)
        labels.append(label)

    # convert the data and labels to NumPy arrays
    data = np.array(data, dtype="float32")
    labels = np.array(labels, dtype="int")

    # return a 2-tuple of the A-Z data and labels
    return (data, labels)

def load_zero_nine_dataset():
    # load the MNIST dataset and stack the training data and testing
    # data together (we'll create our own training and testing splits
    # later in the project)
    ((trainData, trainLabels), (testData, testLabels)) = mnist.load_data()
    data = np.vstack([trainData, testData])
    labels = np.hstack([trainLabels, testLabels])
    # return a 2-tuple of the MNIST data and labels
    return (data, labels)

def load_train_data():
    # load all datasets
    az_dataset_path = r"C:\Projetos\Mestrado\Project II\SourceCode\TextIdentificationService\datasets\a_z\A_Z Handwritten Data.csv"
    (azData, azLabels) = load_az_dataset(az_dataset_path)

    # (digitsData, digitsLabels) = load_zero_nine_dataset()

    # the MNIST dataset occupies the labels 0-9, so let's add 10 to every A-Z label to ensure the A-Z characters are not incorrectly labeled as digits
    #azLabels += 10

    # stack the A-Z data and labels with the MNIST digits data and labels
    #data = np.vstack([azData, digitsData])
    #labels = np.hstack([azLabels, digitsLabels])

    return (azData, azLabels)

def pre_process_dataset(data, labels):
    # each image in the A-Z and MNIST digts datasets are 28x28 pixels;
    # however, the architecture we're using is designed for 32x32 images,
    # so we need to resize them to 32x32
    data = [cv2.resize(image, (32, 32)) for image in data]
    data = np.array(data, dtype="float32")

    # add a channel dimension to every image in the dataset and scale the
    # pixel intensities of the images from [0, 255] down to [0, 1]
    data = np.expand_dims(data, axis=-1)
    data /= 255.0

    # convert the labels from integers to vectors
    le = LabelBinarizer()

    labels = le.fit_transform(labels)
    ounts = labels.sum(axis=0)

    # account for skew in the labeled data
    classTotals = labels.sum(axis=0)
    classWeight = {}

    # loop over all classes and calculate the class weight
    for i in range(0, len(classTotals)):
        classWeight[i] = classTotals.max() / classTotals[i]

    # partition the data into training and testing splits using 80% of
    # the data for training and the remaining 20% for testing
    (trainX, testX, trainY, testY) = train_test_split(data,
                                                    labels, test_size=0.20, stratify=None, random_state=42)

    return (trainX, testX, trainY, testY)





