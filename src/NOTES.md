# Notes

## Status message decoding

Most (not all though, ugh) status messages follow a Regex-parsable syntax that end with the suffix` - Mobility` - for some reason... I've aggregated the following possible "event types" I harvested form a live system:

- `DC Close`
  - "Door Contact" closed. Sent when a sensor placed on windows and doors reports that a previously open state is now closed.  
- `DC Open`
  - "Door Contact" opened. Sent when a sensor placed on windows and doors reports that a previously closed state is now open.
- `IR Activity`
  - An infrared motion sensor has detected a motion event 

The following Regex can be used to extract the individual fields:

```regexp
^Zone:(\d*) ([a-zA-Z0-9 _.-]*), ([a-zA-Z0-9 _.-]*) - Mobility$
```

- Group 1: The zone number of the sensor location
- Group 2: The user-defined zone name of the sensor location
- Group 3: The "event ID" explained above