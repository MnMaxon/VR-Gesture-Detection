from keras.models import load_model
import numpy as np
from collections import deque
import os
import socket
import json
import time
import tensorflow as tf
from keras.backend.tensorflow_backend import set_session



def main():
    cwd = os.getcwd()
    cwd = 'D:/Unity Projects/ml-agents-master/QuickDraw-master'
    config = tf.ConfigProto()
    config.gpu_options.allow_growth = True  # dynamically grow the memory used on the GPU
    config.log_device_placement = False  # to log device placement (on which device the operation ran)
                                    # (nothing gets printed in Jupyter, only if you run it standalone)
    sess = tf.Session(config=config)
    set_session(sess)  # set this TensorFlow session as the default session for Keras

    model = load_model(cwd +'/QuickDraw.h5')

    with open(cwd + '/test.txt', 'r') as myfile:
        square=myfile.read().replace('\n', '')
    pred_probab, pred_class = keras_predict(model, np.array(stars_to_ar(square)))
    print(pred_class, pred_probab)
    
	
    host = 'localhost' 
    port = 50003
    backlog = 5 
    size = 1024 
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
    s.bind((host,port)) 
    s.listen(backlog) 
	
    while 1:
        print("Connecting to Server on " + str(port))
        client, address = s.accept() 
        print ("Client connected.")
        while 1:
            print("tick")
            time.sleep(.02)
            data = str(client.recv(size))
            data = data[2:len(data)-1]
            lines = data.split('--')
            data = lines[len(lines)-2]
			
            print("Received "+ data)

            if data == "Kill":
                client.send(str.encode("Bye!"))
                print ("Bye! Unity Sent: \"" + str(data)+"\"")
                client.close()
                return
            else:
                #pred_probab, pred_class = keras_predict(model, np.array(stars_to_ar(data)))
                indexes = top_predict(model, np.array(stars_to_ar(data)))
                #client.send(str.encode(str(pred_class) + " " + str(pred_probab)))
                client.send(str.encode(str(indexes[0]) + " " + str(indexes[1]) + " " + str(indexes[2]) + "-"))
                print(str(indexes[0]) + " " + str(indexes[1]) + " " + str(indexes[2]))

def stars_to_ar(str):
    digit = []
    for c in str:
        if c=='*': digit.append(1)
        elif c==' ': digit.append(0)
    return digit
				
def keras_predict(model, image):
    processed = keras_process_image(image)
    #processed = image
    print("processed: " + str(processed.shape))
    pred_probab = model.predict(processed)[0]
    pred_class = list(pred_probab).index(max(pred_probab))
    for i in range(len(pred_probab)): print(str(i)+": "+str(pred_probab[i]))
    return max(pred_probab), pred_class

def top_predict(model, image):
    processed = keras_process_image(image)
    #processed = image
    print("processed: " + str(processed.shape))
    pred_probab = model.predict(processed)[0]
    sorted = list(pred_probab)
    sorted.sort(reverse = True)
    pred_class = list(pred_probab).index(max(pred_probab))
    #for i in range(len(pred_probab)): print(str(i)+": "+str(pred_probab[i]))
    indexes = []
    for prob in sorted: indexes.append(list(pred_probab).index(prob))
    return indexes

def keras_process_image(img):
    image_x = 28
    image_y = 28
    img = np.array(img, dtype=np.float32)
    img = np.reshape(img, (-1, image_x, image_y, 1))
    return img

#keras_predict(model, np.zeros((50, 50, 1), dtype=np.uint8))
if __name__ == '__main__':
    main()
