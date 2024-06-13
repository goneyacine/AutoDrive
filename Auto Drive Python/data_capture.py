import socket
import json
import struct
import numpy as np
import matplotlib.pyplot as plt
import cv2 
import pickle
s = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
s.bind((socket.gethostname(),2183))
      


 

            
s.listen(1)
while True:
  client_socket, client_address = s.accept()
  print(f"Accepted connection from {client_address}") 
  frames = []
  player_inputs = []
  while True:
         data = client_socket.recv(120016) 
         if len(data) == 120016:
          inputs = struct.unpack('4I',data[:16])
          player_inputs.append(inputs)
          frame = np.frombuffer(data[16:], dtype=np.uint8)
          frame = frame.reshape((200, 200, 3))
          frame = cv2.rotate(frame, cv2.ROTATE_180)
          frame = cv2.resize(frame,(64,64))
          frames.append(frame)
         if not data:
             with open('data/player_inputs.txt', 'w') as file:
                     file.write(str(player_inputs))
             
             for i, frame in enumerate(frames):
              cv2.imwrite(f'data/frames/frame_{i}.png', frame)
             break
      