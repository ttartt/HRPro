using HRProBusinessLogic.BusinessLogic;
using HRProBusinessLogic.OfficePackage;
using HRProBusinessLogic.OfficePackage.Implements;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.StoragesContracts;
using HRproDatabaseImplement.Implements;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddLog4Net("log4net.config");

// Add services to the container.
builder.Services.AddTransient<IUserStorage, UserStorage>();
builder.Services.AddTransient<ICompanyStorage, CompanyStorage>();
builder.Services.AddTransient<IVacancyStorage, VacancyStorage>();
builder.Services.AddTransient<IResumeStorage, ResumeStorage>();

builder.Services.AddTransient<IUserLogic, UserLogic>();
builder.Services.AddTransient<ICompanyLogic, CompanyLogic>();
builder.Services.AddTransient<IVacancyLogic, VacancyLogic>();
builder.Services.AddTransient<IResumeLogic, ResumeLogic>();
builder.Services.AddTransient<IReportLogic, ReportLogic>();

builder.Services.AddTransient<AbstractSaveToPdf, SaveToPdf>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HRProRestAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRProRestAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
