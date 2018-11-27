import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd
import yaml

from torch import nn
from torch import optim
from torch.utils.data import Dataset

from resource import train
from resource import my_model
from resource import util 

with open("resource/config.yaml") as f:
  config = yaml.safe_load(f) 

DEBUG_MODE = config["DEBUG_MODE"]

device = torch.device("cpu")
  
dataset = train.dataset

X = dataset.tensor_features	  #Data: CPU/RAM
Y = dataset.tensor_results	  #Binary 1/0 was this CPU/RAM case thrashing, for BCELoss 


model = my_model.MyModel() 
optimizer = optim.Adam(model.parameters(), model.learning_rate, betas=(0.9, 0.999), eps=1e-08, weight_decay=0, amsgrad=False)
criterion = nn.BCEWithLogitsLoss(reduction = 'elementwise_mean', pos_weight = dataset.pos_weight)

model.train()

util.debug_print("Iter\t Loss")       
for i in range(model.training_iterations):

    Y_next = model(X)
    loss = criterion(Y_next, Y)

    if i % 500 == 0:
      util.debug_print(i, "\t", "%2.4f" % loss.item())
    
    optimizer.zero_grad()
    loss.backward(retain_graph = True) 
    optimizer.step() #updates params of lin reg model 

model.save_weights()