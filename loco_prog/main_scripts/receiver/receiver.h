/*
  _receiver.h
  Created by Alan T. Grier, 23 September 2019.
*/

#include <Arduino.h>
#include <Radio.h>
#include <RH_RF69.h>
#include <TrainMotor.h>

// Radio parameters
#define CONTROLLER_ADDRESS <CONTROLLER_ADDRESS> // Controller's address
#define LOCOMOTIVE_ADDRESS <LOCOMOTIVE_ADDRESS> // Locomotive's address
#define RF69_FREQ 868.0
#define RF69_KEY new uint8_t [16] {0xa, 0xb, 0xa, 0xd, 0xc, 0xa, 0xf, 0xe, \
								   0xd, 0xe, 0xa, 0xd, 0xb, 0xe, 0xe, 0xf}
#define RFM69_CS 8
#define RFM69_INT 7
#define RFM69_RST 4

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
Radio radio(LOCOMOTIVE_ADDRESS, driver, RFM69_RST);
uint8_t buf[RH_RF69_MAX_MESSAGE_LEN];

// Other initialization
Lighting light_front = Lighting(PIN_LIGHT_FRONT);
Lighting light_rear = Lighting(PIN_LIGHT_REAR);
Lighting light_cab = Lighting(PIN_LIGHT_CAB);
TwoPinMotor locomotive = TwoPinMotor(PIN_MOTOR1, PIN_MOTOR2, &light_front, &light_rear);
long timer_disable = millis();

void setup();
void loop();
void readBatteryVoltage();
void shutdown();
void throttle(uint8_t* command);
