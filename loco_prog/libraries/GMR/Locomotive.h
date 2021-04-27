/*
  Locomotive.h
  Created by Alan T. Grier, 23 September 2019.
*/

#ifndef Locomotive_h
#define Locomotive_h

#include <Arduino.h>
#include <Radio.h>

class Locomotive
{
public:
  Locomotive(int address, int ledPin, Radio *radio);
  void reverse();           // Change direction to -1
  void forward();           // Change direction to 1
  void setSpeed(int speed); // Set the speed of the locomotive
  void sendThrottle();      // Send throttle command with current speed and direction
  void sendEStop();         // Send E-Stop command
  int speed() { return _speed; }
  int direction() { return _direction; }
  int ledPin() { return _ledPin; }

private:
  int _speed = 0;     // Positive number from 0 to 255
  int _direction = 1; // -1 for reverse, +1 for forward
  int _address;       // Positive number
  int _ledPin;        // Arudino PIN of status LED
  Radio *_radio;      // Radio manager
};

class Controller
{
public:
  Controller(int led0, int led1, int max_speed, int num_locomotives,
         Locomotive *locomotives);
  void setCurrent(int current_train);                // Set the currently selected locomotive by array index
  void setSpeed(int speed, int direction);           // Set the speed and direction
  void sendThrottles();                              // Send throttle commands to each train
  void eStopAll();                                   // E-Stop all locomotives
  void indicatorLED(int state, int previous = -1);   // Set the indicator LEDs
  int current_train() { return _current; }           // Get the current train
  int current_speed() { return _current_speed(); }   // Get the current train's speed
  int current_direction() { return _current_dir(); } // Get the current train's direction

private:
  int _current_led() { return _locomotives[_current].ledPin(); };
  int _current_speed() { return _locomotives[_current].speed(); };
  int _current_dir() { return _locomotives[_current].direction(); };
  Locomotive *_locomotives;
  int _num_locomotives;
  int _current = -1;
  int _led0;
  int _led1;
  int _maxSpeed;
};

// Indicator LED states
#define FORWARDS 0
#define REVERSE 1
#define STOP 2
#define IDLE 3
#define WARNING 4
#define RUNNING 5
#define THROTTLE 6

#endif
