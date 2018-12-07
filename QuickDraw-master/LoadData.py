import numpy as np
import os
import pickle

files = os.listdir("data")
x = []
x_load = []
y = []
y_load = []


def load_data():
    count = 0
    for file in files:
        file = "data\\" + file
        x = np.load(file)
        x = x.astype('float32') / 255.
        print(x.shape[1])
        x = x[0:10000, :]
        for j in range(x.shape[0]):
            for i in range(x.shape[1]):
                if x[j][i] < .5: x[j][i] = 0.
                else: x[j][i] = 1.
        x_load.append(x)
        y = [count for _ in range(10000)]
        count += 1
        y = np.array(y).astype('float32')
        y = y.reshape(y.shape[0], 1)
        print(y.shape[1])
        y_load.append(y)

    return x_load, y_load


features, labels = load_data()
features = np.array(features).astype('float32')
labels = np.array(labels).astype('float32')
features=features.reshape(features.shape[0]*features.shape[1],features.shape[2])
labels=labels.reshape(labels.shape[0]*labels.shape[1],labels.shape[2])


with open("features", "wb") as f:
    pickle.dump(features, f, protocol=4)
with open("labels", "wb") as f:
    pickle.dump(labels, f, protocol=4)
