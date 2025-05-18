namespace HRProContracts.BindingModels
{
    public class ExternalVacancyBindingModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Salary { get; set; }
        public string? Url { get; set; }
        public string? City { get; set; }
        public int CompanyId { get; set; }
    }
}
