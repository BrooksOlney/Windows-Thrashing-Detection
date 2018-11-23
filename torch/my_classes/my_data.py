import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd

from torch import nn
from torch import optim
from torch.utils.data import Dataset


class TrashingDataset(Dataset):
  
  #Read in data, split testdata between features and binary results
  def __init__(self, is_training, csv_file):
    
    self.is_training = is_training

    if is_training:
        self.dataframe = pd.read_csv(csv_file)
        self.rows = self.dataframe.shape[1] - 1
        self.features = self.dataframe.iloc[:, 0:self.rows]
        self.results = self.dataframe.iloc[:, self.rows:]  
        self.tensor_features = torch.tensor(self.features.values, dtype = torch.float)
        self.tensor_results = torch.tensor(self.results.values, dtype = torch.float)
    else:
        self.read_last_line(csv_file)
        self.test_data = [float(i) for i in self.last_line]    
        self.tensor_features = torch.tensor(self.test_data)

  #Gets called each polling interval to load newest test vector
  def reload_data(self):
    if self.is_training:
        with open('data/training_set.csv', 'r') as train_data_csv:
            self.__init__(True, train_data_csv)
    else:
        while(1):
            try:
                with open('data/testing_set.csv', 'r') as test_data_csv:
                    self.__init__(False, test_data_csv)
                    break
            except (PermissionError, FileNotFoundError):
                pass # Keep trying until it works

  def read_last_line(self, csv_file):
    while(1):
        try:
            self.last_line = csv_file.readlines()[-1].strip().split(",")
            break
        except (PermissionError, FileNotFoundError):
            pass # Keep trying until it works
            


with open('data/training_set.csv', 'r') as train_data_csv:
    training_dataset = TrashingDataset(True, train_data_csv)

with open('data/testing_set.csv', 'r') as test_data_csv:
    testing_dataset = TrashingDataset(False, test_data_csv)
