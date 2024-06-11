import socket
import json
s = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
s.bind((socket.gethostname(),2183))
      


 

            
s.listen(1)
while True:
  client_socket, client_address = s.accept()
  print(f"Accepted connection from {client_address}") 
  samples = []
  while True:
         data = client_socket.recv(24592) 
        ## print(data)
         if not data:
             print('end')
             break
         samples.append(data)
  with open("Dataset.json", 'w') as file:
        file.write(str(samples))
      