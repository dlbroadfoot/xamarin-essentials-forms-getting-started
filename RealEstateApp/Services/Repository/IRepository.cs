using System;
using System.Collections.Generic;
using RealEstateApp.Models;

namespace RealEstateApp.Services.Repository
{
    public interface IRepository
    {
        IEnumerable<Agent> GetAgents();
        IEnumerable<Property> GetProperties();
        void SaveProperty(Property property);
        IObservable<Property> ObservePropertySaved();
    }
}