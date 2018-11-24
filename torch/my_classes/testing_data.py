import torch
import torch.nn as nn
import torchvision
import numpy as np
import pandas as pd
import win32file


from . import training_data

from torch import nn
from torch import optim
from torch.utils.data import Dataset

class TestingDataset(Dataset):
    def __init__(self, fileHandle, mean, std):
        self.fileHandle = fileHandle
        self.mean = mean
        self.std = std
        self.test_data = self.parse_pipe_data()    
        print("\n", self.test_data)
        self.tensor_features = torch.tensor(self.test_data)
        self.normalize_data()

    def normalize_data(self):
        self.tensor_features = torch.sub(self.tensor_features, self.mean)
        self.tensor_features = torch.div(self.tensor_features, self.std)
        
    def reload_data(self):
        self.__init__(self.fileHandle, self.mean, self.std)           

    def parse_pipe_data(self):
        left, data = win32file.ReadFile(fileHandle, 4096)
        # print("DATA: ", data)
        first_split = data.decode().strip().split(",")
        string_vector = [first_split[0].strip().split(" ")[1], first_split[1].strip(), first_split[2].strip()]
        return [float(i) for i in string_vector]

fileHandle = win32file.CreateFile("\\\\.\\pipe\\pytorchModel", win32file.GENERIC_READ | win32file.GENERIC_WRITE, 0, None, win32file.OPEN_EXISTING, 0, None)

testing_dataset = TestingDataset(fileHandle, training_data.training_dataset.mean, training_data.training_dataset.std)





# while(1):
# 	try:
# 		with open('data/testing_set.csv', 'r') as test_data_csv:
# 			break
# 	except (PermissionError, FileNotFoundError):	
# 		print("FUCK")
# 		pass #Keep trying until it works
