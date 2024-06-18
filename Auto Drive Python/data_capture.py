import socket
import struct
import numpy as np
import cv2 
import ast
import time

s = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
s.bind((socket.gethostname(),2183)) 
s.listen(1)
while True:
  client_socket, client_address = s.accept()
  print(f"Accepted connection from {client_address}") 
  frames = []
  player_inputs = []
  mvt_data = []
  while True:
         data = client_socket.recv(3096) 
         if len(data) == 3096:
          inputs = struct.unpack('3I',data[:12])
          player_inputs.append(inputs)
          mvt_data.append(struct.unpack('fff',data[12:24]))
          frame = np.frombuffer(data[24:], dtype=np.uint8)
          frame = frame.reshape((32, 32, 3))
          frames.append(frame) 
         if not data:
             print(len(frames),len(player_inputs))
             with open('data/player_inputs.txt', 'wr') as file:
                     content = file.read()
                     if not content:
                      file.write(str(player_inputs))
                     else:
                      player_inputs += ast.literal_eval(content)
                      file.write(str(player_inputs))
             with open('data/mvt_data.txt', 'wr') as file:
                     content = file.read()
                     if not content:
                      file.write(str(mvt_data))
                     else:
                      mvt_data += ast.literal_eval(content)
                      file.write(str(mvt_data))
             for i, frame in enumerate(frames):
                 cv2.imwrite(f'data/frames/frame_{time.time() + i}.png', frame)
             print('done...')
             break
      