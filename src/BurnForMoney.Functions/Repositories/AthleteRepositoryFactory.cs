﻿using BurnForMoney.Domain;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.Repositories
{
    public static class AthleteRepositoryFactory
    {
        public static IRepository<Athlete> Create()
        {
            var configuration = ApplicationConfiguration.GetSettings();

            var repository = new Repository<Athlete>(EventStore.Create(configuration.ConnectionStrings.AzureWebJobsStorage,
                new EventsDispatcher(configuration.EventGrid.SasKey, configuration.EventGrid.TopicEndpoint)));

            return repository;
        }
    }
}