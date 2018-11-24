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
  
dataset = my_data.training_dataset

X = dataset.tensor_features		#Data: CPU/RAM
Y = dataset.tensor_results	
	#Binary 1/0 was this CPU/RAM case thrashing, for BCELoss 

model = my_model.MyModel()
# optimizer = optim.SGD(model.parameters(), model.learning_rate) 
optimizer = optim.Adam(model.parameters(), model.learning_rate, betas=(0.9, 0.999), eps=1e-08, weight_decay=0, amsgrad=False)

#rename, move to model
pweight = Y.shape[0] / torch.sum(Y)

criterion = nn.BCEWithLogitsLoss(reduction = 'elementwise_mean', pos_weight = pweight )

model.train()

#delete later
Y_next = model(X)
loss = criterion(Y_next, Y)
optimizer.zero_grad()
loss.backward(retain_graph = True)
optimizer.step() #updates params of lin reg model 

for i in range(model.training_iterations):
    Y_next = model(X)

    loss = criterion(Y_next, Y)

    if i % 500 == 0:
      print(i, loss.item())
    
    optimizer.zero_grad()
    loss.backward(retain_graph = True)
    optimizer.step() #updates params of lin reg model 

model.save_weights()
