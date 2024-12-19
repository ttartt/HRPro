using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.SearchModels;
using HRProContracts.StoragesContracts;
using HRProContracts.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProBusinessLogic.BusinessLogic
{
    public class TestTaskLogic : ITestTaskLogic
    {
        private readonly ILogger _logger;
        private readonly ITestTaskStorage _testTaskStorage;
        public TestTaskLogic(ILogger<TestTaskLogic> logger, ITestTaskStorage testTaskStorage)
        {
            _logger = logger;
            _testTaskStorage = testTaskStorage;
        }
        public bool Create(TestTaskBindingModel model)
        {
            CheckModel(model);
            if (_testTaskStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(TestTaskBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_testTaskStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public TestTaskViewModel? ReadElement(TestTaskSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _testTaskStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<TestTaskViewModel>? ReadList(TestTaskSearchModel? model)
        {
            var list = model == null ? _testTaskStorage.GetFullList() : _testTaskStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(TestTaskBindingModel model)
        {
            CheckModel(model);
            if (_testTaskStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(TestTaskBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.CreatorId <= 0)
            {
                throw new ArgumentException("Нет создателя тестового задания", nameof(model.CreatorId));
            }

            if (string.IsNullOrEmpty(model.Topic))
            {
                throw new ArgumentNullException(nameof(model.Topic), "Нет темы тестового задания");
            }

            if (string.IsNullOrEmpty(model.Status.ToString()))
            {
                throw new ArgumentNullException(nameof(model.Status), "Нет статуса тестового задания");
            }

            var element = _testTaskStorage.GetElement(new TestTaskSearchModel
            {
                CreatorId = model.CreatorId,
                Topic = model.Topic
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Тестовое задание с такими данными уже существует");
            }
        }

    }
}
