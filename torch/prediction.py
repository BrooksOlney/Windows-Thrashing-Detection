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

class ThrashingDetector:
	def __init__(self):
		self.counter = 0

	def predict(self, prediction):
		if prediction > 0.50:
			self.counter += 1
		else:
			self.counter -= 1

		if self.counter > 30:
			print("ALERT: THRASHING")
		elif self.counter < 0:
			self.counter = 0


device = torch.device("cpu")

dataset = my_data.testing_dataset

X = dataset.tensor_features


model = my_model.MyModel()
model.load_weights()

# def is_thrashing(prediction):
# 	if prediction > 0.50:
# 		print("THRASHING: %.2f" % (prediction * 100) + "%")
# 		return 1
# 	else:
# 		print("NOT THRASHING: %.2f" % (prediction * 100) + "%")
# 		return -1

 
model.eval()

detector = ThrashingDetector()

while(detector.counter < 10):
	time.sleep(1)

	dataset.reload_data()
	
	test_tensor_data = dataset.tensor_features
	
	prediction = model(test_tensor_data).item()
	
	detector.predict(prediction)
	
	print(detector.counter)

print("THRASHING!!!")
