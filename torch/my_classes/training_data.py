import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd
import win32file


from torch import nn
from torch import optim
from torch.utils.data import Dataset


class TrainingDataset(Dataset):
  
  #Read in data, split testdata between features and binary results
  def __init__(self, csv_file):

    self.dataframe = pd.read_csv(csv_file)
    self.rows = self.dataframe.shape[1] - 1
    self.features = self.dataframe.iloc[:, 0:self.rows]
    self.results = self.dataframe.iloc[:, self.rows:]  
    self.tensor_features = torch.tensor(self.features.values, dtype = torch.float)
    self.tensor_results = torch.tensor(self.results.values, dtype = torch.float)
    self.normalize_data()

    self.total_rows = self.dataframe.shape[0]
    self.total_wins = torch.sum(self.tensor_results)
    self.pos_weight = self.total_rows / self.total_wins

  def normalize_data(self):
    self.mean = self.tensor_features.mean()
    self.std = self.tensor_features.std()  
    self.tensor_features = torch.sub(self.tensor_features, self.mean)
    self.tensor_features = torch.div(self.tensor_features, self.std)

  #Gets called each polling interval to load newest test vector
  def reload_data(self):
    with open('data/training_set.csv', 'r') as train_data_csv:
        self.__init__(train_data_csv)



with open('data/training_set.csv', 'r') as train_data_csv:
    training_dataset = TrainingDataset(train_data_csv)

    