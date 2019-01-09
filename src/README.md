### Project structure

* **App** - client side application, covers both public and internal website.
* **BurnForMoney.Functions** - main function app, handles all operations related to athletes and activities.
* **BurnForMoney.Functions.Strava** - function app integrated with the Strava API, responsible for athletes registration and basic CRUD operations on the activities from Strava (via webhooks).
* **BurnForMoney.Functions.PublicApi** - api for public website.
* **BurnForMoney.Functions.InternalApi** - api for internal website.
* **BurnForMoney.Functions.Presentation** - manages views, creates projections using EventGrid subscription model.
* **BurnForMoney.Functions.Shared** - shared functions code.
* **BurnForMoney.Domain** - shared domain code.
* **BurnForMoney.Identity** - shared identity code.
* **BurnForMoney.Infrastructure** - shared infrastructure code.
* **ResourceGroup** - consists of ARM templates and Powershell scripts used to automatically create an environment.

### Build

Run `scripts/func_start_all.sh` script to build and run all the functions.

