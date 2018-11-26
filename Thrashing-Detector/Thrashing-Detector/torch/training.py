import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd

from torch import nn
from torch import optim
from torch.utils.data import Dataset

from my_classes import training_data
from my_classes import my_model


device = torch.device("cpu")
  
dataset = training_data.training_dataset

X = dataset.tensor_features	  #Data: CPU/RAM
Y = dataset.tensor_results	  #Binary 1/0 was this CPU/RAM case thrashing, for BCELoss 


model = my_model.MyModel() 
optimizer = optim.Adam(model.parameters(), model.learning_rate, betas=(0.9, 0.999), eps=1e-08, weight_decay=0, amsgrad=False)
criterion = nn.BCEWithLogitsLoss(reduction = 'elementwise_mean', pos_weight = dataset.pos_weight)

model.train()

for i in range(model.training_iterations):
    Y_next = model(X)

    loss = criterion(Y_next, Y)

    #Debug info, delete later
    if i % 500 == 0:
      print(i, loss.item())
    
    optimizer.zero_grad()
    loss.backward(retain_graph = True) #Do I need retain_graph? Look into
    optimizer.step() #updates params of lin reg model 

model.save_weights()
