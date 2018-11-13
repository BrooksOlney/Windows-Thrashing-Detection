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

X = dataset.tensor_metrics		#Data: CPU/RAM
Y = dataset.tensor_results		#Binary 1/0 was this CPU/RAM case thrashing, for BCELoss 

model = my_model.MyModel()
optimizer = optim.SGD(model.parameters(), model.learning_rate) 
criterion = nn.BCELoss(reduction = 'elementwise_mean')

for i in range(model.training_iterations):
    model.train()
    Y_next = model(X)
	
    loss = criterion(Y_next, Y)
    if i % 500 == 0:
      print(i, loss.item())
    
    optimizer.zero_grad()
    loss.backward()
    optimizer.step() #updates params of lin reg model 

model.save_weights()


######## DELETE LATER

def is_thrashing(prediction):
  if prediction > 0.50:
    print("THRASHING: %.2f" % (prediction * 100) + "%")
  else:
    print("NOT THRASHING: %.2f" % (prediction * 100) + "%")

  
model.eval()

test_tensor_data = torch.FloatTensor([3, 3])

prediction = model(test_tensor_data).item()

is_thrashing(prediction)