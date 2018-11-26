import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd
import time
import win32pipe

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

  def normalize_data(self):
    self.mean = self.tensor_features.mean()
    self.std = self.tensor_features.std()  
    self.tensor_features = torch.sub(self.tensor_features, self.mean)
    self.tensor_features = torch.div(self.tensor_features, self.std)

  #Gets called each polling interval to load newest test vector
  def reload_data(self):
    with open('data/training_set.csv', 'r') as train_data_csv:
        self.__init__(True, train_data_csv)



class TestingDataset(Dataset):
    def __init__(self, csv_file, mean, std):
        self.mean = mean
        self.std = std
        self.read_last_line(csv_file)
        self.test_data = [float(i) for i in self.last_line]    
        self.tensor_features = torch.tensor(self.test_data)
        self.normalize_data()

    def normalize_data(self):
        self.tensor_features = torch.sub(self.tensor_features, self.mean)
        self.tensor_features = torch.div(self.tensor_features, self.std)
        
    def reload_data(self):
        while(1):
            try:
                with open('data/testing_set.csv', 'r') as test_data_csv:
                    self.__init__(test_data_csv, self.mean, self.std)
                    break

            except (PermissionError, FileNotFoundError):
                print("EXCEPTION RELOADING")    
                pass # Keep trying until it works            

    def read_last_line(self, csv_file):
        while(1):
            try:
                self.last_line = csv_file.readlines()[-1].strip().split(",")
                break
            except (PermissionError, FileNotFoundError, IndexError):
                print("EXCEPTION READ LAST LINE")
                time.sleep(0.125)
                pass # Keep trying until it works


with open('data/training_set.csv', 'r') as train_data_csv:
    training_dataset = TrainingDataset(train_data_csv)

while(1):
    try:
        with open('data/testing_set.csv', 'r') as test_data_csv:
            testing_dataset = TestingDataset(test_data_csv, training_dataset.mean, training_dataset.std)
            break
    except (PermissionError, FileNotFoundError):
        print("EXCEPTION WHILE OPENING")
        pass #Keep trying until it works

        
