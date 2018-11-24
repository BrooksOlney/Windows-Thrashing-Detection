import time
import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd

from torch import nn
from torch import optim
from torch.utils.data import Dataset

from my_classes import my_data
from my_classes import my_model

MAX_COUNT = 30
POLLING_INTERVAL = 0.125

class ThrashingDetector:
	def __init__(self):
		self.counter = 0

	def predict(self, prediction):
		step = 1 if prediction > 0.50 else -1
		self.counter = max(0, min(MAX_COUNT, self.counter + step))

device = torch.device("cpu")

dataset = my_data.testing_dataset

X = dataset.tensor_features


model = my_model.MyModel()
model.load_weights() 
model.eval()

detector = ThrashingDetector()

i = 0

#while(1): #for testing, delete later
while(detector.counter < MAX_COUNT):
	time.sleep(POLLING_INTERVAL)

	dataset.reload_data()
	
	test_tensor_data = dataset.tensor_features
	
	prediction = model.predict(test_tensor_data).item()
	
	detector.predict(prediction)
	print(detector.counter)
	print(prediction * 100, "%")
	print("Count: ", i)
print("THRASHING!!!")
