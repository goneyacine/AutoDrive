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
         data = client_socket.recv(3084) 
         if len(data) == 3084:
          mvt_data = struct.unpack('fff',data[:12])
          mvt_data = np.array(mvt_data)
          frame = np.frombuffer(data[12:], dtype=np.uint8)
          frame = frame.reshape((32, 32, 3))
          frame = frame[:,:,0]
          with tf.device('/cpu:0'):
           prediction = model_(inputs=[frame[None,...],mvt_data[None,...]]).numpy()
           if prediction[0,0] > 0.5:
              controller.press('w')
           else:
              controller.release('w')
           if prediction[0,1] > 0.5:
              controller.press('a')
           else:
              controller.release('a')
           if prediction[0,2] > 0.5:
              controller.press('d')
           else:
              controller.release('d')  
      