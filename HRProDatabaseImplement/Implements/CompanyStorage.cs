using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using Microsoft.EntityFrameworkCore;

namespace HRproDatabaseImplement.Implements
{
    public class CompanyStorage : ICompanyStorage
    {
        public CompanyViewModel? Delete(CompanyBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Companies.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Companies.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }

        public CompanyViewModel? GetElement(CompanySearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Companies
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Name) && x.Name == model.Name) || (model.Id.HasValue && x.Id == model.Id))?
                .GetViewModel;
        }

        public List<CompanyViewModel> GetFilteredList(CompanySearchModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Companies
                .Where(x => x.Name.Equals(model.Name))
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public List<CompanyViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Companies
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public int? Insert(CompanyBindingModel model)
        {
            var newCompany = Company.Create(model);
            if (newCompany == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Companies.Add(newCompany);
            context.SaveChanges();
            return newCompany.Id;
        }

        public CompanyViewModel? Update(CompanyBindingModel model)
        {
            using var context = new HRproDatabase();
            var company = context.Companies
                .FirstOrDefault(x => x.Id == model.Id);
            if (company == null)
            {
                return null;
            }
            company.Update(model);
            context.SaveChanges();
            return company.GetViewModel;
        }
    }
}
