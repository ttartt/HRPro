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
    public class TaskLogic : ITaskLogic
    {
        private readonly ILogger _logger;
        private readonly ITaskStorage _taskStorage;
        public TaskLogic(ILogger<TaskLogic> logger, ITaskStorage taskStorage)
        {
            _logger = logger;
            _taskStorage = taskStorage;
        }
        public bool Create(TaskBindingModel model)
        {
            CheckModel(model);
            if (_taskStorage.Insert(model) == null)
            {
                _logger.LogWarning("Insert operation failed");
                return false;
            }
            return true;
        }

        public bool Delete(TaskBindingModel model)
        {
            CheckModel(model, false);
            _logger.LogInformation("Delete. Id: {Id}", model.Id);
            if (_taskStorage.Delete(model) == null)
            {
                _logger.LogWarning("Delete operation failed");
                return false;
            }
            return true;
        }

        public TaskViewModel? ReadElement(TaskSearchModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var element = _taskStorage.GetElement(model);
            if (element == null)
            {
                _logger.LogWarning("ReadElement element not found");
                return null;
            }
            _logger.LogInformation("ReadElement find. Id: {Id}", element.Id);
            return element;
        }

        public List<TaskViewModel>? ReadList(TaskSearchModel? model)
        {
            var list = model == null ? _taskStorage.GetFullList() : _taskStorage.GetFilteredList(model);
            if (list == null)
            {
                _logger.LogWarning("ReadList return null list");
                return null;
            }
            _logger.LogInformation("ReadList. Count: {Count}", list.Count);
            return list;
        }

        public bool Update(TaskBindingModel model)
        {
            CheckModel(model);
            if (_taskStorage.Update(model) == null)
            {
                _logger.LogWarning("Update operation failed");
                return false;
            }
            return true;
        }

        private void CheckModel(TaskBindingModel model, bool withParams = true)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!withParams)
            {
                return;
            }

            if (model.TestTaskId <= 0)
            {
                throw new ArgumentException("Нет идентификатора тестового задания", nameof(model.TestTaskId));
            }

            if (string.IsNullOrEmpty(model.Text))
            {
                throw new ArgumentNullException(nameof(model.Text), "Нет текста задачи");
            }

            var element = _taskStorage.GetElement(new TaskSearchModel
            {
                TestTaskId = model.TestTaskId,
                Text = model.Text
            });

            if (element != null && element.Id != model.Id)
            {
                throw new InvalidOperationException("Задача с такими данными уже существует");
            }
        }
    }
}
