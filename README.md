# AccessControl Adapter Implementation Sample

This repository contains a sample implementation demonstrating how to create an AccessControl adapter that facilitates communication between an AccessControl system and a VMS. Please see [Generic Access Control Adapter Integration API Guide](https://github.com/A-H-Software-House-Inc/GenericAccessControlAdapterAPIDocumentation).

## Prerequisites

Before diving in, make sure you have the [SimpleAccessControlEmulator](https://github.com/A-H-Software-House-Inc/SimpleAccessControlEmulator) up and running. This emulator mimics the functionality of a real AccessControl system and is crucial for testing your adapter implementation.

## Implementing Your Own AccessControl Adapter

To build your adapter, implement the `IAccessControlAdapter` interface. A test implementation for the SimpleAccessControlEmulator is provided in the `AccessControlAdapter` class. Here is a rundown of the required methods:

- `public Dictionary<string, ItemData> GetItems();`  
  Returns a dictionary containing all AccessControl items (doors, zones, partitions, outputs). The key is the item ID, and the value is the item data.

- `public Dictionary<string, UserData> GetUsers();`  
  Returns a dictionary of AccessControl users. The key is the user ID, and the value is the user data.

- `public Dictionary<string, EventData> GetEvents();`  
  Returns a dictionary of AccessControl events. The key is the event ID, and the value is the event data.

- `public Dictionary<string, ActionData> GetActions();`  
  Returns a dictionary of AccessControl actions (system or item actions). The key is the action ID, and the value is the action data.

- `public Dictionary<string, ParameterData> GetParameters();`  
  Returns a dictionary of AccessControl parameters used in actions. The key is the parameter ID, and the value is the parameter data.

- `public ActionData? GetAction(string id);`  
  Retrieves the action data for the specified ID.

- `public ParameterData? GetParameter(string id);`  
  Retrieves the parameter data for the specified ID.

- `public void FireAccessControlEvents();`  
  Collects and pushes AccessControl event notifications to the VMS.

- `public bool ExecuteSystemAction(ActionData actionData, Dictionary<string, string> actionParams, out string errorMessage);`  
  Executes a system action within the AccessControl system.

- `public bool ExecuteItemAction(ActionData actionData, ItemData itemData, Dictionary<string, string> actionParams, out string errorMessage);`  
  Executes an item action within the AccessControl system.

## Adapter Settings

Configure your adapter using the `AccessControlAdapterSettings.json` file. The table below details each configuration parameter:

| Parameter Name                    | Description                                                                                                    |
| --------------------------------- |:--------------------------------------------------------------------------------------------------------------:|
| logLevel                          | Minimum log level severity (trace, debug, info, warn, error, fatal)                                            |
| logFile                           | Path to the log file                                                                                           |
| accessControlHost                 | SimpleAccessControlEmulator address                                                                            |
| accessControlPort                 | SimpleAccessControlEmulator port                                                                               |
| accessControlAdapterHttpPort      | HTTP port on the localhost where the AccessControl adapter will be accessible                                  |
| accessControlAdapterHttpsPort     | HTTPS port on the localhost where the AccessControl adapter will be accessible                                 |
| useHttpsServer                    | Set to `true` if an HTTPS connection should be used between the VMS and the AccessControl adapter              |
| accessControlAdapterIntegrationId | Integration ID (obtained from Luxriot) needed to connect your AccessControl adapter with the VMS               |
| accessControlPollTimeout          | Timeout (in milliseconds) for obtaining data from the AccessControl system                                     |
| responsePageSize                  | Maximum number of elements on one page of the adapter's response                                               |

## Connecting the Adapter to VMS

1. Open the Console and navigate to the Security Systems configuration.
2. Add a new security system configuration.
3. Set the appropriate connection settings for the Access Control adapter.

## Running as a Windows Service

To run the Access Control Adapter Sample as a Windows Service:

1. **Publish the Application:**  
   Right-click on the `AccessControlAdapterSample` project in Visual Studio and select **Publish**.

2. **Install the Service:**  
   Open PowerShell as an Administrator and run the following command (adjust the path as needed):
   
   sc.exe create "Access Control Adapter Sample Service" binpath= "C:\Path\To\AccessControlAdapterSample.exe"
   
   Your adapter will then run as a Windows Service, silently doing its magic in the background.



