/*
  Radio.cpp
  Created by Alan T. Grier, 23 September 2019.
*/

#include "Radio.h"

// Wrapper around RadioHead Driver
Radio::Radio(int address, RHGenericDriver *driver)
{
  _address = address;
  _driver = driver;

  _driver->init();
  _driver->setThisAddress(_address);
  _driver->setHeaderFrom(_address);
}

// Send message of length to address
bool Radio::send(uint8_t *message, uint8_t length, uint8_t to)
{
  _driver->setHeaderTo(to);
  return _driver->send(message, length);
}

// Receive message of length from address
bool Radio::receive(uint8_t *message, uint8_t *length, uint8_t *from)
{
  if (_driver->recv(message, length))
  {
    if (from)
      *from = _driver->headerFrom();
    return true;
  }
  return false;
}

// Check if messages are available
bool Radio::available()
{
  return _driver->available();
}
