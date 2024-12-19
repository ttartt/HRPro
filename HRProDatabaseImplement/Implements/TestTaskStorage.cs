using HRProContracts.BindingModels;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using HRproDatabaseImplement;
using HRProDatabaseImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProDatabaseImplement.Implements
{
    public class TestTaskStorage : ITestTaskStorage
    {
        public List<TestTaskViewModel> GetFullList()
        {
            using var context = new HRproDatabase();
            return context.TestTasks
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public List<TestTaskViewModel> GetFilteredList(TestTaskSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Topic))
            {
                return new();
            }
            using var context = new HRproDatabase();
            return context.TestTasks
                .Where(x => x.Topic.Contains(model.Topic))
                .Select(x => x.GetViewModel)
                .ToList();
        }
        public TestTaskViewModel? GetElement(TestTaskSearchModel model)
        {
            if (string.IsNullOrEmpty(model.Topic) && !model.Id.HasValue)
            {
                return null;
            }
            using var context = new HRproDatabase();
            return context.TestTasks
                .FirstOrDefault(x => (!string.IsNullOrEmpty(model.Topic) && x.Topic == model.Topic) || (model.Id.HasValue && x.Id == model.Id))
                ?.GetViewModel;
        }
        public TestTaskViewModel? Insert(TestTaskBindingModel model)
        {
            var newTestTask = TestTask.Create(model);
            if (newTestTask == null)
            {
                return null;
            }
            using var context = new HRproDatabase();
            context.TestTasks.Add(newTestTask);
            context.SaveChanges();
            return newTestTask.GetViewModel;
        }
        public TestTaskViewModel? Update(TestTaskBindingModel model)
        {
            using var context = new HRproDatabase();
            var criterion = context.TestTasks.FirstOrDefault(x => x.Id == model.Id);
            if (criterion == null)
            {
                return null;
            }
            criterion.Update(model);
            context.SaveChanges();
            return criterion.GetViewModel;
        }
        public TestTaskViewModel? Delete(TestTaskBindingModel model)
        {
            using var context = new HRproDatabase();
            var element = context.TestTasks.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                context.TestTasks.Remove(element);
                context.SaveChanges();
                return element.GetViewModel;
            }
            return null;
        }
    }
}
