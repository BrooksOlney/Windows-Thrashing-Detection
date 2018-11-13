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

device = torch.device("cpu")

dataset = my_data.dataset

X = dataset.tensor_metrics
Y = dataset.tensor_results


model = my_model.MyModel()
model.load_weights()

def is_thrashing(prediction):
  if prediction > 0.50:
    print("THRASHING: %.2f" % (prediction * 100) + "%")
  else:
    print("NOT THRASHING: %.2f" % (prediction * 100) + "%")

    
model.eval()

test_tensor_data = torch.FloatTensor([3, 3])

prediction = model(test_tensor_data).item()

is_thrashing(prediction)