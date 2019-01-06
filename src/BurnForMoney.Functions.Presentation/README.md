> The project is responsible for presentation layer (read and writes).

### How it works?

Events are received through [Event Grid](https://azure.microsoft.com/en-us/services/event-grid/) subscriptions. Every event is processed by view that handles this type of event and typically it ends with changes in the SQL database.

#### How to create Event Grid subscription?

* Create a webhook function (e.g. [RankingSubscription](https://github.com/makingwaves/BurnForMoney/blob/master/src/BurnForMoney.Functions.Presentation/Functions/RankingSubscription.cs)) that would be triggered by Event Grid (use `EventGridTrigger` attribute).

  > The function must have exactly the same name as the [FunctionName].

* Create a subscription in Azure portal:
  * Resource group: `BurnForMoney_{environment},`
  * EventGrid topic: `bfmDomainEvents{environment}`,
  * Add Event Grid subscription:
    * name: `subscriptionName-v{version},`
    * event types: subscribe either to all or selected events,
    * endpoint type: `Web Hook`,
    * subscriber endpoint url: `https://bfmfunc-presentation-{environment}.azurewebsites.net/runtime/webhooks/EventGrid?functionName={functionName}&code={functionKey}`,
    * enable dead-lettering, choose `eventgrid-deadletter` as a container (located in the `bfmfunc{environment}` storage account).







