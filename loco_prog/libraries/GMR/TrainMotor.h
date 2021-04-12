/*
  TrainMotor.h
  Created by Alan T. Grier, 23 September 2019.
*/

#ifndef TrainMotor_h
#define TrainMotor_h

class Lighting
{
public:
    Lighting(int LightPin);
    void On();           // Turn light on with digitalWrite
    void Off();          // Turn light off with digitalWrite
    void Set(int value); // Turn light on with analogWrite

private:
    int _pin; // Arduino pin that controls the light
};

// SingleMotor is currently out of date
class SingleMotor
{
public:
    SingleMotor(int speedPin, int motorPin);
    void setSpeed(int motorSpeed); // Speed from 0 to 255
    void disable();
    void enable();

private:
    int _speedPin;
    int _motorPin;
    int _motorSpeed;
    bool _isEnabled;
};

// DualMotor is currently out of date
class DualMotor
{
public:
    DualMotor(int speedPin, int motor1Pin, int motor2Pin);
    void setSpeed(int motorSpeed); // Speed from -255 to 255
    void disable();
    void enable();

private:
    int _speedPin;
    int _motor1Pin;
    int _motor2Pin;
    int _motorSpeed;
    int _motorDirection;
    bool _isEnabled;
};

class TwoPinMotor
{
public:
    TwoPinMotor(int motor1Pin, int motor2Pin, Lighting *front = 0, Lighting *rear = 0);
    void setSpeed(int motorSpeed);        // Set speed from -255 to 255, enabling motor if it isn't already
    void disable(bool zeroSpeed = false); // Turn off motor, with or without zeroing the speed
    void enable();                        // Turn on motor with previously set speed
    void _setLights(int dir);             // Set the lights per current direction

private:
    Lighting *_front;    // Front light
    Lighting *_rear;     // Rear light
    int _motor1Pin;      // Pin for controlling direction A on motor driver (positive speed)
    int _motor2Pin;      // Pin for controlling direction B on motor driver (negative speed)
    int _motorSpeed;     // Speed, from 0 to 255
    int _motorDirection; // The motor pin corresponding to the chosen direction (i.e. _motor2Pin for reverse)
    bool _isEnabled;     // Whether the locomotive is running
};

#endif
