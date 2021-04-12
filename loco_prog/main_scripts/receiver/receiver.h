/*
  _receiver.h
  Created by Alan T. Grier, 23 September 2019.
*/

#include <Arduino.h>
#include <RHReliableDatagram.h>
#include <RH_RF69.h>
#include <TrainMotor.h>

// Radio parameters
#define CLIENT_ADDRESS 101 // Controller's address
#define SERVER_ADDRESS 202 // Locomotive's address
#define RF69_FREQ 868.0
#define RFM69_CS 8
#define RFM69_INT 7

// Pin assignments
#define PIN_BATTERY A0 // IO Pin for reading battery voltage
#define PIN_LIGHT_FRONT 9
#define PIN_LIGHT_REAR 10
#define PIN_LIGHT_CAB 11
#define PIN_AUX_FUNC 12
#define PIN_MOTOR1 5
#define PIN_MOTOR2 6
#define PIN_FUNC1 A1
#define PIN_FUNC2 A2

// Battery Undervoltage Threshold
// Target voltage is < 16 V (5S pack)
// Voltage divider with 953 Ohm and 5500 Ohm reisitors
//IO voltage as 16V is 2.25V, corresponding to analogRead() = 700
#define BATTERY_THRESHOLD 712

// Radio initialization
RH_RF69 driver(RFM69_CS, RFM69_INT);
RHReliableDatagram manager(driver, SERVER_ADDRESS);
uint8_t buf[RH_RF69_MAX_MESSAGE_LEN];

// Other initialization
Lighting light_front = Lighting(PIN_LIGHT_FRONT);
Lighting light_rear = Lighting(PIN_LIGHT_REAR);
Lighting light_cab = Lighting(PIN_LIGHT_CAB);
TwoPinMotor locomotive = TwoPinMotor(PIN_MOTOR1, PIN_MOTOR2, &light_front, &light_rear);
long timer_disable = millis();
