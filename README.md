# Burn for money

## Documentation

GitHub wiki: https://github.com/makingwaves/BurnForMoney/wiki

Sharepoint: https://makingwaves.sharepoint.com/sites/BurnforMoney

## Supported systems

|                 |  Strava  | Endomondo | Runkeeper |
| :-------------: | :------: | :-------: | :-------: |
|       Run       | &#x2714; | &#x2718;  | &#x2718;  |
|      Ride       | &#x2714; | &#x2718;  | &#x2718;  |
|      Walk       | &#x2714; | &#x2718;  | &#x2718;  |
|  Winter sports  | &#x2714; | &#x2718;  | &#x2718;  |
|  Water sports   | &#x2714; | &#x2718;  | &#x2718;  |
|   Team sports   | &#x2718; | &#x2718;  | &#x2718;  |
|       Gym       | &#x2714; | &#x2718;  | &#x2718;  |
|      Hike       | &#x2714; | &#x2718;  | &#x2718;  |
| Fitness / Dance | &#x2714; | &#x2718;  | &#x2718;  |
|      Other      | &#x2714; | &#x2718;  | &#x2718;  |

## Installation

### Requirements

* [Azure Developer Tools](https://azure.microsoft.com/en-us/tools/)
* [.NET Core SDK](https://www.microsoft.com/net/download)
* [Azure Storage Emulator](https://go.microsoft.com/fwlink/?LinkId=717179&clcid=0x409)
* [PowerShell](https://www.microsoft.com/web/handlers/webpi.ashx/getinstaller/WindowsAzurePowershellGet.3f.3f.3fnew.appids)
* [Azure Functions and Web Jobs Tools](https://marketplace.visualstudio.com/items?itemName=VisualStudioWebandAzureTools.AzureFunctionsandWebJobsTools) - Visual Studio only
* [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools) - recommended for Mac OS / Linux users, optional for Windows users
* IDE: [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/) / [Visual Studio Code](https://visualstudio.microsoft.com/downloads/)

### Design

* UX: https://makingwaves.invisionapp.com/share/Y5OB37AQ349
* Design: https://makingwaves.invisionapp.com/share/TXNW58VVRU2#/screens/318202851_bfm_Home

### Environments

* **Test:**
  * Website: https://burnformoney-test.azurewebsites.net/
  * Api: https://burnformoneyfunc-test.azurewebsites.net/
* **Production:**
  - Website: https://bfm.makingwaves.pl/
  - Api: https://burnformoneyfunc-prod.azurewebsites.net/


### Diagrams

#### User authorization - Strava

![User authorization - Strava](../master/docs/user-authorization-strava.png)

#### Collecting activities

![Collecting activities](../master/docs/collecting-activities.png)



## Code of conduct

MIT License

Copyright (c) 2018 Making Waves

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.