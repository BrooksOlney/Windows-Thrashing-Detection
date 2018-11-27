import time
import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd
import yaml

from torch import nn
from torch import optim
from torch.utils.data import Dataset

from resource import test
from resource import my_model
from resource import util

with open("resource/config.yaml") as f:
  config = yaml.safe_load(f)

MAX_COUNT = config["MAX_COUNT"]
POLLING_INTERVAL = config["POLLING_INTERVAL"]
PREDICTION_CONFIDENCE = config["PREDICTION_CONFIDENCE"]

class ThrashingDetector:
	def __init__(self):
		self.counter = 0

	def predict(self, prediction):
		step = 1 if prediction > PREDICTION_CONFIDENCE else -1
		self.counter = max(0, min(MAX_COUNT, self.counter + step))
		self.is_thrashing = True if self.counter == MAX_COUNT else 0

device = torch.device("cpu")

dataset = test.dataset

model = my_model.MyModel()
model.load_weights() 
model.eval()

detector = ThrashingDetector()

while(detector.counter < MAX_COUNT):
	time.sleep(POLLING_INTERVAL)

	dataset.reload_data()

	prediction = model.predict(dataset.tensor_features).item()
	
	detector.predict(prediction)

	util.debug_print("\nCPU: ", dataset.CPU, "\tRAM: ", dataset.RAM, "\tFaults: ", dataset.faults)
	util.debug_print("Count: \t",detector.counter)
	util.debug_print("Prediction: \t", prediction * 100, "%")

if detector.is_thrashing == True:
	print("\n*************************")
	print("THRASHING!!!")
	print("*************************")

