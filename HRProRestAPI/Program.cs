using HRProBusinessLogic.BusinessLogic;
using HRProBusinessLogic.MailWorker;
using HRProBusinessLogic.OfficePackage;
using HRProBusinessLogic.OfficePackage.Implements;
using HRProContracts.BindingModels;
using HRProContracts.BusinessLogicsContracts;
using HRProContracts.StoragesContracts;
using HRproDatabaseImplement.Implements;
using HRProDatabaseImplement.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddTransient<IUserStorage, UserStorage>();
builder.Services.AddTransient<ICompanyStorage, CompanyStorage>();
builder.Services.AddTransient<IVacancyStorage, VacancyStorage>();
builder.Services.AddTransient<IResumeStorage, ResumeStorage>();
builder.Services.AddTransient<IMeetingStorage, MeetingStorage>();
builder.Services.AddTransient<IMeetingParticipantStorage, MeetingParticipantStorage>();
builder.Services.AddTransient<IDocumentStorage, DocumentStorage>();
builder.Services.AddTransient<ITemplateStorage, TemplateStorage>();
builder.Services.AddTransient<ITagStorage, TagStorage>();
builder.Services.AddTransient<IDocumentTagStorage, DocumentTagStorage>();
builder.Services.AddTransient<IMessageInfoStorage, MessageInfoStorage>();

builder.Services.AddTransient<IUserLogic, UserLogic>();
builder.Services.AddTransient<ICompanyLogic, CompanyLogic>();
builder.Services.AddTransient<IVacancyLogic, VacancyLogic>();
builder.Services.AddTransient<IResumeLogic, ResumeLogic>();
builder.Services.AddTransient<IMeetingLogic, MeetingLogic>();
builder.Services.AddTransient<IMeetingParticipantLogic, MeetingParticipantLogic>();
builder.Services.AddTransient<ITemplateLogic, TemplateLogic>();
builder.Services.AddTransient<ITagLogic, TagLogic>();
builder.Services.AddTransient<IDocumentTagLogic, DocumentTagLogic>();
builder.Services.AddTransient<IDocumentLogic, DocumentLogic>();
builder.Services.AddTransient<IMessageInfoLogic, MessageInfoLogic>();

builder.Services.AddSingleton<AbstractMailWorker, MailKitWorker>();
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
/*builder.Services.AddHttpClient("AvitoClient")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        Proxy = new WebProxy(new Uri("http://proxy-service.com:8000")),
        UseProxy = true,
        AllowAutoRedirect = true,
    });*/
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HRProRestAPI", Version = "v1" });
});

var app = builder.Build();

var mailSender = app.Services.GetService<AbstractMailWorker>();

mailSender?.MailConfig(new MailConfigBindingModel
{
    MailLogin = builder.Configuration?.GetSection("MailLogin")?.Value?.ToString() ?? string.Empty,
    MailPassword = builder.Configuration?.GetSection("MailPassword")?.Value?.ToString() ?? string.Empty,
    SmtpClientHost = builder.Configuration?.GetSection("SmtpClientHost")?.Value?.ToString() ?? string.Empty,
    SmtpClientPort = Convert.ToInt32(builder.Configuration?.GetSection("SmtpClientPort")?.Value?.ToString()),
    PopHost = builder.Configuration?.GetSection("PopHost")?.Value?.ToString() ?? string.Empty,
    PopPort = Convert.ToInt32(builder.Configuration?.GetSection("PopPort")?.Value?.ToString())
});

using (var scope = app.Services.CreateScope())
{
    var userLogic = scope.ServiceProvider.GetRequiredService<IUserLogic>();
    var existingAdmin = userLogic.ReadElement(new HRProContracts.SearchModels.UserSearchModel
    {
        Email = "admin@admin.com"
    });

    if (existingAdmin == null)
    {
        userLogic.Create(new UserBindingModel
        {
            Surname = "Admin",
            Name = "Admin",
            Email = "admin@admin.com",
            Password = "Admin123!",
            Role = HRProDataModels.Enums.RoleEnum.Администратор,
            PhoneNumber = "9999999999",
            IsEmailConfirmed = true
        });
        Console.WriteLine("Администратор создан");
    }
    else
    {
        Console.WriteLine("Администратор уже существует.");
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
