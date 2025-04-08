using HRProBusinessLogic.BusinessLogic;
using HRProBusinessLogic.OfficePackage;
using HRProBusinessLogic.OfficePackage.Implements;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.StoragesContracts;
using HRproDatabaseImplement.Implements;
using HRProDatabaseImplement.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddTransient<IUserStorage, UserStorage>();
builder.Services.AddTransient<ICompanyStorage, CompanyStorage>();
builder.Services.AddTransient<IVacancyStorage, VacancyStorage>();
builder.Services.AddTransient<IResumeStorage, ResumeStorage>();
builder.Services.AddTransient<IMeetingStorage, MeetingStorage>();
builder.Services.AddTransient<IMeetingParticipantStorage, MeetingParticipantStorage>();
builder.Services.AddTransient<IRequirementStorage, RequirementStorage>();
builder.Services.AddTransient<IResponsibilityStorage, ResponsibilityStorage>();
builder.Services.AddTransient<IVacancyRequirementStorage, VacancyRequirementStorage>();
builder.Services.AddTransient<IVacancyResponsibilityStorage, VacancyResponsibilityStorage>();
builder.Services.AddTransient<IDocumentStorage, DocumentStorage>();
builder.Services.AddTransient<ITemplateStorage, TemplateStorage>();
builder.Services.AddTransient<ITagStorage, TagStorage>();
builder.Services.AddTransient<IDocumentTagStorage, DocumentTagStorage>();

builder.Services.AddTransient<IUserLogic, UserLogic>();
builder.Services.AddTransient<ICompanyLogic, CompanyLogic>();
builder.Services.AddTransient<IVacancyLogic, VacancyLogic>();
builder.Services.AddTransient<IResumeLogic, ResumeLogic>();
builder.Services.AddTransient<IMeetingLogic, MeetingLogic>();
builder.Services.AddTransient<IMeetingParticipantLogic, MeetingParticipantLogic>();
builder.Services.AddTransient<IRequirementLogic, RequirementLogic>();
builder.Services.AddTransient<IResponsibilityLogic, ResponsibilityLogic>();
builder.Services.AddTransient<IVacancyRequirementLogic, VacancyRequirementLogic>();
builder.Services.AddTransient<IVacancyResponsibilityLogic, VacancyResponsibilityLogic>();
builder.Services.AddTransient<IDocumentLogic, DocumentLogic>();
builder.Services.AddTransient<ITemplateLogic, TemplateLogic>();
builder.Services.AddTransient<ITagLogic, TagLogic>();
builder.Services.AddTransient<IDocumentTagLogic, DocumentTagLogic>();

builder.Services.AddTransient<AbstractSaveToPdf, SaveToPdf>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();


builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HRProRestAPI", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userLogic = scope.ServiceProvider.GetRequiredService<IUserLogic>();
    var existingAdmin = userLogic.ReadElement(new HRProContracts.SearchModels.UserSearchModel
    {
        Email = "admin@admin.com"
    });

    if (existingAdmin == null)
    {
        userLogic.Create(new HRProContracts.BindingModels.UserBindingModel
        {
            Surname = "Admin",
            Name = "Admin",
            Email = "admin@admin.com",
            Password = "Admin123!",
            Role = HRProDataModels.Enums.RoleEnum.�������������,
            PhoneNumber = "9999999999",
            City = "AdminCity"
        });
        Console.WriteLine("������������� ������");
    }
    else
    {
        Console.WriteLine("������������� ��� ����������.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HRProRestAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
