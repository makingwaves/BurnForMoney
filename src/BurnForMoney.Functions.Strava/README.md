> The project manages all operations related to data exchange between Strava and the BurnForMoney application.

### How it works?

At the time of registration of the new user, all his historical activities are pulled starting from the first day of the month. All subsequent changes are supported using the [Webhook Events API](https://developers.strava.com/docs/webhooks/).

#### How to test Webhook Events API?

Webhook Events API requires active subscription with publicly available address where webhooks events will be sent. In order to test it locally, local url must be exposed and it may be achieved by using [ngrok](https://ngrok.com/).

First, authenticate ngrok account (it's one-time operation) (`~/addons/executables/auth_ngrok.sh`) using authentication token stored in the KeyVault (`burnformoneykvdev`). And then start ngrok tunnel (`~/addons/executables/start_ngrok_7072.sh`).

The last operation requires to create a new subscription. Copy forwarding url from ngrok process (it changes every time when you start up ngrok) and call function:

```
curl -X POST \
  http://localhost:7072/api/strava/subscription/create \
  -d '{
	"hostname": "https://{code}.ngrok.io"
}'
```

