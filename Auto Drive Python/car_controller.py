import socket
import struct
from pynput.keyboard import Controller
import cv2
import tensorflow as tf
import numpy as np

controller = Controller()
model_ = tf.keras.models.load_model('auto drive python/cnn_model.keras')
s = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
s.bind((socket.gethostname(),2184))
s.listen(1)
while True:
  client_socket, client_address = s.accept()
  print(f"Accepted connection from {client_address}") 
  while True:
         data = client_socket.recv(120000) 
         if len(data) == 120000:
          frame = np.frombuffer(data, dtype=np.uint8)
          frame = frame.reshape((200, 200, 3))
          frame = cv2.rotate(frame, cv2.ROTATE_180)
          frame = cv2.resize(frame,(64,64))
          with tf.device('/cpu:0'):
           prediction = model_(frame[None,...]).numpy()
           if prediction[0,0] > 0.5:
              controller.press('w')
           else:
              controller.release('w')
           if prediction[0,1] > 0.5:
              controller.press('s')
           else:
              controller.release('s')
           if prediction[0,2] > 0.5:
              controller.press('a')
           else:
              controller.release('a')  
           if prediction[0,3] > 0.5:
              controller.press('d')
           else:
             controller.release('d') 
   
      