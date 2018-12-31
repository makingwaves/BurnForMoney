### Project structure

* **App** - client side application, covers both public and internal website.
* **BurnForMoney.Functions** - main function app, handles all operations related to athletes and activities.
* **BurnForMoney.Functions.Strava** - function app integrated with the Strava API, responsible for athletes registration and basic CRUD operations on the activities from Strava (via webhooks).
* **BurnForMoney.Functions.PublicApi** - api for public website.
* **BurnForMoney.Functions.InternalApi** - api for internal website.
* **BurnForMoney.Functions.ReadModel** - manages readonly data, creates projections using EventGrid subscription model.
* **BurnForMoney.Functions.Shared** - shared code.
* **BurnForMoney.Infrastructure** - domain logic.
* **ResourceGroup** - consists of ARM templates and Powershell scripts used to automatically create an environment.

### Build

Run `init_build.sh` script to build all the functions.

