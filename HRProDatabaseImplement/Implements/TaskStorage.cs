using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProDatabaseImplement.Implements
{
    public class TaskStorage : ITaskStorage
    {
        public List<TaskViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.Tasks
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<TaskViewModel> GetFilteredList(TaskSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Text))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.Tasks
                .Where(x => x.Text.Contains(model.Text))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public TaskViewModel? GetElement(TaskSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Text) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.Tasks
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Text) && x.Text == model.Text) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public TaskViewModel? Insert(TaskBindingModel model)
        {
            var newTask = HRProDatabaseImplement.Models.Task.Create(model);
            if (newTask == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.Tasks.Add(newTask);
            context.SaveChanges();
            return newTask.GetViewModel;
        }
        public TaskViewModel? Update(TaskBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.Tasks.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public TaskViewModel? Delete(TaskBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.Tasks.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.Tasks.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
