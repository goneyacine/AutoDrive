import tensorflow as tf
import numpy as np


class model:
    def __init__(self) -> None:
        pass
    def setup(self):
      #  base = tf.keras.applications.MobileNetV3Small(input_shape=(200,200,3),include_top=False,weights='imagenet')
        
        #base.trainable = False
        input  = tf.keras.Input(shape=(64,64,3))
        conv1 = tf.keras.layers.Conv2D(32,(4,4),strides=(2,2),padding='same')(input)
        conv1 = tf.keras.layers.BatchNormalization()(conv1)
        conv1 = tf.keras.layers.LeakyReLU(0.2)(conv1)
        conv2 = tf.keras.layers.Conv2D(64,(4,4),strides=(2,2),padding='same')(conv1)
        conv2 = tf.keras.layers.BatchNormalization()(conv2)
        conv2 = tf.keras.layers.LeakyReLU(0.2)(conv2)
        conv3 = tf.keras.layers.Conv2D(128,(4,4),strides=(2,2),padding='same')(conv2)
        conv3 = tf.keras.layers.BatchNormalization()(conv3)
        conv3 = tf.keras.layers.LeakyReLU(0.2)(conv3)
        conv4 = tf.keras.layers.Conv2D(256,(4,4),strides=(2,2),padding='same')(conv3)
        conv4 = tf.keras.layers.BatchNormalization()(conv4)
        conv4 = tf.keras.layers.LeakyReLU(0.2)(conv4)
        flatten_layer = tf.keras.layers.Flatten()(conv4)
        fc_layer_1 = tf.keras.layers.Dense(units=128,activation='relu')(flatten_layer)
        fc_layer_2 = tf.keras.layers.Dense(units=64,activation='relu')(fc_layer_1)
        output = tf.keras.layers.Dense(units=4,activation='linear')(fc_layer_2)
        self.model = tf.keras.models.Model(inputs=input,outputs=output)
    def train(self,train_ds,epochs_=5,autosave_model=True,output_file='cnn_model.keras'):
        if self.model == None:
            print('Error : model is not initialzed.')
            return
        self.model.fit(train_ds,epochs=epochs_)
        if autosave_model:
           print('saving model...')
           self.model.save(output_file)
           print('model saved.')
    def compile(self):
        self.model.compile(optimizer='adam',loss='mse',metrics=['accuracy','mse'])
    