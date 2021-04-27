/*
  TrainMotor.cpp
  Created by Alan T. Grier, 23 September 2019.
*/

#include "Arduino.h"
#include "TrainMotor.h"

// Class for operating a light
Lighting::Lighting(int LightPin)
{
  _pin = LightPin;
  pinMode(_pin, OUTPUT);
  Off();
}

// Turn light off with digitalWrite
void Lighting::Off()
{
  digitalWrite(_pin, LOW);
}

// Turn light on with digitalWrite
void Lighting::On()
{
  digitalWrite(_pin, HIGH);
}

// Turn light on with analogWrite
void Lighting::Set(int value)
{
  analogWrite(_pin, value);
}

// Ramp the analogWrite on an IO pin from a specific value to a specific value
// Set inverse to True when the motor driver control needs to be inverted (i.e. LOW is full speed)
void _rampSpeed(int pin, int start, int target, bool inverse = false)
{
  if (start == target)
  {
    return;
  }
  int speed = start;
  int dir = (target - start) / abs(target - start);
  int move_amt = 4;
  int delay_amt = 8;
  if (!inverse)
  {
    analogWrite(pin, speed);
  }
  else
  {
    analogWrite(pin, 255 - speed);
  }
  while (speed != target)
  {
    speed = speed + (move_amt * dir);
    if (dir == 1 && speed > target)
    {
      speed = target;
    }
    else if (dir == -1 && speed < target)
    {
      speed = target;
    }
    if (speed == 0)
      if (!inverse)
      {
        digitalWrite(pin, LOW);
      }
      else
      {
        digitalWrite(pin, HIGH);
      }
    else
    {
      if (!inverse)
      {
        analogWrite(pin, speed);
      }
      else
      {
        analogWrite(pin, 255 - speed);
      }
    }
    delay(delay_amt);
  }
}

SingleMotor::SingleMotor(int speedPin, int motorPin)
{
  pinMode(speedPin, OUTPUT);
  pinMode(motorPin, OUTPUT);
  _speedPin = speedPin;
  _motorPin = motorPin;
  _motorSpeed = 0;
  digitalWrite(_motorPin, LOW);
  analogWrite(_speedPin, _motorSpeed);
  _isEnabled = false;
}

void SingleMotor::setSpeed(int motorSpeed)
{
  if (motorSpeed == _motorSpeed)
    return;
  analogWrite(_speedPin, motorSpeed);
  _motorSpeed = motorSpeed;
}

void SingleMotor::disable()
{
  if (!_isEnabled)
    return;
  digitalWrite(_motorPin, LOW);
  _isEnabled = false;
}

void SingleMotor::enable()
{
  if (_isEnabled)
    return;
  digitalWrite(_motorPin, HIGH);
  _isEnabled = true;
}

DualMotor::DualMotor(int speedPin, int motor1Pin, int motor2Pin)
{
  pinMode(speedPin, OUTPUT);
  pinMode(motor1Pin, OUTPUT);
  pinMode(motor2Pin, OUTPUT);
  _speedPin = speedPin;
  _motor1Pin = motor1Pin;
  _motor2Pin = motor2Pin;
  _motorSpeed = 0;
  _motorDirection = 1;
  digitalWrite(_motor1Pin, LOW);
  digitalWrite(_motor2Pin, LOW);
  analogWrite(_speedPin, _motorSpeed);
  _isEnabled = false;
}

void DualMotor::setSpeed(int motorSpeed)
{
  int _msp = abs(motorSpeed);
  int _dir = (motorSpeed < 0) ? -1 : 1;
  if (_msp == _motorSpeed && _dir == _motorDirection)
    return;
  if (_msp != _motorSpeed && _dir == _motorDirection)
  {
    _rampSpeed(_speedPin, _motorSpeed, _msp);
    _motorSpeed = _msp;
    _isEnabled = true;
    return;
  }
  _rampSpeed(_speedPin, _motorSpeed, 0);
  if (_motorDirection == 1)
  {
    digitalWrite(_motor2Pin, LOW);
    digitalWrite(_motor1Pin, HIGH);
  }
  else
  {
    digitalWrite(_motor1Pin, LOW);
    digitalWrite(_motor2Pin, HIGH);
  }
  _rampSpeed(_speedPin, 0, _msp);
  _motorSpeed = _msp;
  _isEnabled = true;
}

void DualMotor::disable()
{
  if (!_isEnabled)
    return;
  digitalWrite(_motor1Pin, LOW);
  digitalWrite(_motor2Pin, LOW);
  _isEnabled = false;
}

void DualMotor::enable()
{
  if (_isEnabled)
    return;
  if (_motorDirection == 1)
  {
    digitalWrite(_motor2Pin, LOW);
    digitalWrite(_motor1Pin, HIGH);
  }
  else
  {
    digitalWrite(_motor1Pin, LOW);
    digitalWrite(_motor2Pin, HIGH);
  }
  _isEnabled = true;
}

// Bi-directional motor with Two control pins (A, B) and lights that correspond to forward/rearward movement
TwoPinMotor::TwoPinMotor(int motor1Pin, int motor2Pin, Lighting *front = 0, Lighting *rear = 0)
{
  pinMode(motor1Pin, OUTPUT);
  pinMode(motor2Pin, OUTPUT);
  _motor1Pin = motor1Pin;
  _motor2Pin = motor2Pin;
  _motorSpeed = 0;
  _motorDirection = _motor1Pin;
  digitalWrite(_motor1Pin, HIGH);
  digitalWrite(_motor2Pin, HIGH);
  _isEnabled = false;
  _front = front;
  _rear = rear;
}

// Set the speed and direction of the motor, from -255 to 255
void TwoPinMotor::setSpeed(int motorSpeed)
{
  int msp = abs(motorSpeed);
  int dir = (motorSpeed < 0) ? _motor2Pin : _motor1Pin;
  if (msp == _motorSpeed && dir == _motorDirection) // Speed and direction are the same
    return;
  _setLights(dir);
  if (dir == _motorDirection) // Direction is the same, speed is different
  {
    _rampSpeed(_motorDirection, _motorSpeed, msp, true); // Change speed
    _motorSpeed = msp;
    _isEnabled = true;
    return;
  }
  // At this point in the method, we must be changing direction
  _rampSpeed(_motorDirection, _motorSpeed, 0, true); // Ramp to zero, in order to change direction
  _motorSpeed = msp;
  _motorDirection = dir;
  _setLights(dir);
  _rampSpeed(_motorDirection, 0, _motorSpeed, true); // Ramp from zero to new speed and direction
  _isEnabled = true;
}

// Turn off the motor, zeroing the speed if selected
void TwoPinMotor::disable(bool zeroSpeed = false)
{
  if (zeroSpeed)
  {
    setSpeed(0);
    _front->Off();
    _rear->Off();
  }
  if (!_isEnabled)
    return;
  digitalWrite(_motor1Pin, HIGH);
  digitalWrite(_motor2Pin, HIGH);
  _isEnabled = false;
}

// Turn on the motor with a previously set speed and direction
void TwoPinMotor::enable()
{
  if (_isEnabled)
    return;
  // The following digitalWrite selects the non-driving pin to write HIGH
  // For forwards, _motorDirection = _motor1Pin, so _motor2Pin is selected
  // For reverse, _motorDirection = _motor2Pin, so _motor1Pin is selected
  digitalWrite(_motor1Pin + _motor2Pin - _motorDirection, HIGH);
  analogWrite(_motorDirection, 255 - _motorSpeed);
  _isEnabled = true;
}

// Set the lights per current direction
void TwoPinMotor::_setLights(int dir)
{
  if (dir == _motor2Pin)
  {
    if (_rear != NULL)
      _rear->On();
    if (_front != NULL)
      _front->Off();
  }
  else
  {
    if (_front != NULL)
      _front->On();
    if (_rear != NULL)
      _rear->Off();
  }
}
