import torch
import torch.nn as nn
import torch.nn.functional as F
import torchvision
import numpy as np
import pandas as pd

from torch import nn
from torch import optim
from torch.utils.data import Dataset

from . import my_data

class MyModel(nn.Module):
  def __init__(self):
    super(MyModel, self).__init__()
    
    self.input_dimensions = my_data.training_dataset.rows
    self.output_dimensions = 1
    self.hidden_layer = 2
    
    self.learning_rate = 0.01
    self.training_iterations = 25000 #NAN errors over 3000 currently
    
    self.linear_layer_1 = nn.Linear(self.input_dimensions, self.hidden_layer)
    self.linear_layer_2 = nn.Linear(self.hidden_layer, self.output_dimensions)
    
  #Whenever __call__ is made, this is run
  def forward(self, x):
    leaky_relu = nn.LeakyReLU(0.1)

    layer1 = leaky_relu(self.linear_layer_1(x))
    layer2 = leaky_relu(self.linear_layer_2(layer1))
    return layer2
  
  #Range: [0,1]
  def sigmoid(self, x):
    return 1 / (1 + torch.exp(-x))
  
  #Returns a % how likely input fits model
  def predict(self, inputs):
    return self.sigmoid(self.forward(inputs)) 

  def save_weights(self):
    torch.save(self.state_dict(), "data/neural_net.pt")
  
  def load_weights(self):
    self.load_state_dict(torch.load("data/neural_net.pt"))
