using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement.Models;
using HRProDataModels.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRproDatabaseImplement.Implements
{
    public class UserStorage : IUserStorage
    {
        public UserViewModel? Delete(UserBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Users.Include(x => x.Company).FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Users.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }

        public UserViewModel? GetElement(UserSearchModel model)
        {
            using var context = new HRproDatabase();
            if (!string.IsNullOrEmpty(model.Email))
            {
                return context.Users
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Email == model.Email)?
                .GetViewModel;
            }                     
            else
            {
                return context.Users
                .Include(x => x.Company)
                .FirstOrDefault(x => (x.Id == model.Id))?
                .GetViewModel;
            }
        }

        public List<UserViewModel> GetFilteredList(UserSearchModel model)
        {
            using var context = new HRproDatabase();

            if (model.CompanyId.HasValue && model.Role != null)
            {
                return context.Users
                .Where(x => x.CompanyId.Equals(model.CompanyId) && x.Role.Equals(model.Role))
                .Select(x => x.GetViewModel)
                .ToList();
            }
            else if (model.CompanyId.HasValue)
            {
                return context.Users
                .Where(x => x.CompanyId == model.CompanyId)
                .Select(x => x.GetViewModel)
                .ToList();
            }
            else if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return new();
            }           

            return context.Users
                .Include(x => x.Company)
                .Where(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password))
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public List<UserViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Users
                .Include(x => x.Company)
                .ToList()
                .Select(x => x.GetViewModel)
                .ToList();
        }

        public UserViewModel? Insert(UserBindingModel model)
        {
            var newUser = User.Create(model);
            if (newUser == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Users.Add(newUser);
            context.SaveChanges();
            return context.Users
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Id == newUser.Id)?
                .GetViewModel;
        }

        public UserViewModel? Update(UserBindingModel model)
        {
            using var context = new HRproDatabase();
            var user = context.Users
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Id == model.Id);

            if (user == null)
            {
                return null;
            }

            user.Update(model);
            context.SaveChanges();
            return user.GetViewModel;
        }
    }
}
