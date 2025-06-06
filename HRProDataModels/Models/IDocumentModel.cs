﻿namespace HRProDataModels.Models
{
    public interface IDocumentModel : IId
    {
        int CreatorId { get; }
        int CompanyId { get; }
        string Name { get; }
        DateTime CreatedAt { get; }
        int? TemplateId { get; }
        string FilePath { get; }
    }
}
