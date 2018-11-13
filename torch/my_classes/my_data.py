import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd

from torch import nn
from torch import optim
from torch.utils.data import Dataset


class TrashingDataset(Dataset):
  def __init__(self, csv_file):
    
    #Read in data, split testdata between features and binary results
    self.dataframe = pd.read_csv(csv_file)
    
    self.rows = self.dataframe.shape[1] - 1
    
    self.metrics = self.dataframe.iloc[:, 0:self.rows]
    self.results = self.dataframe.iloc[:, self.rows:]
    
    self.tensor_metrics = torch.tensor(self.metrics.values, dtype = torch.float)
    self.tensor_results = torch.tensor(self.results.values, dtype = torch.float)
    
    self.batch_size_metrics = self.tensor_metrics.size()[0]
    self.dimension_count_results = self.tensor_results.size()[1]

    self.batch_size_metrics = self.tensor_metrics.size()[0]
    self.dimension_count_results = self.tensor_results.size()[1]
  
  def __len__(self):
    return len(self.dataframe)


with open('data/training_set.csv', 'r') as csv_file:
  dataset = TrashingDataset(csv_file)