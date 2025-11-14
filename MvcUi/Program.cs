using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.FileUploadServices;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.InputCleaner;
using AyrshireCollege.Biis.InputProcessingLibrary.InputProcessing.ValidationAttributes;
using AyrshireCollege.Biis.UserIdentityLibrary.IdentityServices;
using AyrshireCollege.Biis.UserIdentityLibrary.Interfaces;
using DataAccessLibrary.DataAccessObjects;
using DataAccessLibrary.DataServices;
using EmailLibrary.Configuration;
using EmailLibrary.Extensions;
using FluentEmail.Smtp;
using MvcUi.Middleware;
using MvcUi.Models;
using MvcUi.Services;
using MvcUi.Services.ApiClients;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using MvcUi.Middleware;
using MvcUi.Services;
using MvcUi.Services.ApiClients;
using SharedServicesLibrary.AdminStaffServices;
using SharedServicesLibrary.AppSettings;
using SharedServicesLibrary.EmailServices;
using SharedServicesLibrary.ExcelExportServices;
using SharedServicesLibrary.FormHandlingServices;
using SharedServicesLibrary.FormIconServices;
using SharedServicesLibrary.FormPreparationModels;
using SharedServicesLibrary.FormPreparationServices;
using SharedServicesLibrary.Identity;
using SharedServicesLibrary.SharedServices.UserRoleServices;
using SharedServicesLibrary.StudentExperienceFormServices;
using System.Security.Claims;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

// Ensure log folder exists
var logPath = builder.Configuration["Serilog:WriteTo:0:Args:path"];
if (!string.IsNullOrEmpty(logPath))
{
    if (!Path.IsPathRooted(logPath))
    {
        logPath = Path.Combine(AppContext.BaseDirectory, logPath);
    }

    var logFolder = Path.GetDirectoryName(logPath);
    if (!string.IsNullOrEmpty(logFolder))
    {
        Directory.CreateDirectory(logFolder);
    }
}


builder.Host.UseSerilog();








// Register the custom authorization filter if needed
// Only do if you plan to inject or use the filter programmatically, like in global filters
// builder.Services.AddScoped<AllowRolesAuthorizationFilter>(); (this would be automatically handled through AllowRolesAttribute)

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddEmailServices(builder.Configuration);

// Register file upload helper and related settings
// Enable client-side validation
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions>(options =>
{
    options.ClientValidationEnabled = true;
});

// Configure Form Options to allow larger file uploads (e.g., 100MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});


// Bind GlobalSettings from appsettings.json
builder.Services.Configure<GlobalSettings>(builder.Configuration.GetSection("GlobalSettings"));

// Bind EmailSettings from appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Bind StatusSettings from appsettings.json
builder.Services.Configure<StatusSettings>(builder.Configuration.GetSection("StatusSettings"));



// Access the Configuration object
var configuration = builder.Configuration;


// Register an HttpClient service for the StudentExperienceFormApiClient class
builder.Services.AddHttpClient<StudentExperienceFormApiClient>(client =>
{
    // Retrieve the base URL for the Student Experience Forms API from configuration (appsettings.json)
    var baseUrl = builder.Configuration["ApiUrls:StudentExperienceForms"];

    // Retrieve the API key to be sent in requests to the API, also from configuration
    var apiKey = builder.Configuration["ApiKeys:StudentExperienceForms"];

    // Ensure the base URL ends with a trailing slash '/'
    // This prevents incorrect URL concatenation when making relative requests
    if (!baseUrl.EndsWith('/'))
        baseUrl += "/";

    // Set the base address for all HTTP requests sent by this client
    client.BaseAddress = new Uri(baseUrl);

    // Add the API key header "X-Api-Key" with the configured key to every request sent by this HttpClient
    // This header will be used by the API server to authenticate the client
    client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
})
// Configure the underlying HTTP message handler
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        // Use the Windows credentials of the currently logged-in user
        // Required for authenticating with APIs secured by Windows Authentication (NTLM/Kerberos)
        UseDefaultCredentials = true
    });




// Retrieve UploadFolder path from appsettings.json e.g. uploads
var uploadFolder = configuration["UploadSettings:UploadFolder"];

// Combine with the wwwroot directory to get the absolute path e.g. C:\YourProject\wwwroot\uploads
var uploadsAbsoluteDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder);

// Register IFileUploadHelper with DI container
builder.Services.AddScoped<IFileUploadHelper>(provider =>
    new FileUploadHelper(
        uploadsAbsoluteDirectoryPath,
        uploadFolder // relative path to use for database
    ));

// Register other services
builder.Services.AddSingleton(
     new ConnectionStringData
     {
         SqlConnectionName = "Default"
     }
);
builder.Services.AddSingleton<UserTrackerService>();
builder.Services.AddSingleton<IDataAccess, SqlDataAccess>();
builder.Services.AddScoped<IMainFormService, MainFormService>();
builder.Services.AddScoped<IMainFormAdminService, MainFormAdminService>();

builder.Services.AddScoped<IImpactedPersonTypeService, ImpactedPersonTypeService>();
builder.Services.AddScoped<IIncidentBehaviourTypeService, IncidentBehaviourTypeService>();
builder.Services.AddScoped<IIncidentMotivationTypeService, IncidentMotivationTypeService>();
builder.Services.AddScoped<ISampleCheckboxService, SampleCheckboxService>();

builder.Services.AddScoped<IMainFormImpactedPersonTypeService, MainFormImpactedPersonTypeService>();
builder.Services.AddScoped<IMainFormIncidentBehaviourTypeService, MainFormIncidentBehaviourTypeService>();
builder.Services.AddScoped<IMainFormIncidentMotivationTypeService, MainFormIncidentMotivationTypeService>();
builder.Services.AddScoped<IMainFormSampleCheckboxService, MainFormSampleCheckboxService>();


builder.Services.AddScoped<ISampleCheckboxAdminService, SampleCheckboxAdminService>();
builder.Services.AddScoped<IMainFormSampleCheckboxAdminService, MainFormSampleCheckboxAdminService>();


builder.Services.AddScoped<IIncidentHappenedToService, IncidentHappenedToService>();
builder.Services.AddScoped<INumberOfPeopleImpactedService, NumberOfPeopleImpactedService>();
builder.Services.AddScoped<INumberOfPeopleCausedIncidentService, NumberOfPeopleCausedIncidentService>();
builder.Services.AddScoped<IIncidentLocationService, IncidentLocationService>();
builder.Services.AddScoped<IHasSimilarIncidentHappenedBeforeService, HasSimilarIncidentHappenedBeforeService>();
builder.Services.AddScoped<ISampleRadioService, SampleRadioService>();
builder.Services.AddScoped<ISampleRadioAdminService, SampleRadioAdminService>();

builder.Services.AddScoped<ISampleDropdownService, SampleDropdownService>();
builder.Services.AddScoped<ISampleDropdownAdminService, SampleDropdownAdminService>();


builder.Services.AddScoped<IBiisAdminStaffService, BiisAdminStaffService>();
builder.Services.AddScoped<IAdminStaffService, AdminStaffService>();
builder.Services.AddScoped<IPrepareFormSubmissionEmailNotificationService, PrepareFormSubmissionEmailNotificationService>();


// Register IHttpContextAccessor to allow access to HttpContext in services (e.g., to get current user info)
builder.Services.AddHttpContextAccessor();

// Register the UserIdentityService (NuGet UserIdentityLibraryApp) for resolving user identity information from the current HttpContext
builder.Services.AddScoped<IUserIdentityService, UserIdentityService>();

// Register the Data Access Library service for for database operations related to identity overrides
builder.Services.AddScoped<IIdentityOverrideDataService, IdentityOverrideDataService>();

// Register the SharedServicesLibrary/Identity mapper service that maps between IdentityOverride entity models and View Models
builder.Services.AddScoped<IIdentityOverrideMapper, IdentityOverrideMapper>();

// Register the main IdentityService (NuGet UserIdentityLibraryApp) that encapsulates business logic related to user identity and overrides
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Register the SharedIdentityOverrideService as the implementation of IIdentityOverrideService,
// so that when IIdentityOverrideService is requested via dependency injection,
// the app will use SharedIdentityOverrideService, which provides effective user identity resolution including override logic.
builder.Services.AddScoped<AyrshireCollege.Biis.UserIdentityLibrary.Interfaces.IIdentityOverrideService, SharedServicesLibrary.Identity.SharedIdentityOverrideService>();



builder.Services.AddScoped<IUserRoleService, UserRoleService>();

builder.Services.AddScoped<IMainFormViewModelPreparationService, MainFormViewModelPreparationService>();
builder.Services.AddScoped<IMainFormAdminViewModelPreparationService, MainFormAdminViewModelPreparationService>();

builder.Services.AddScoped<IFormHandlingService, FormHandlingService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IViewModelContextBuilder, ViewModelContextBuilder>();
builder.Services.AddScoped<IAdminViewModelContextBuilder, AdminViewModelContextBuilder>();

builder.Services.AddScoped<IAdminStaffMapper, AdminStaffMapper>();
builder.Services.AddScoped<IAdminStaffManagementService, AdminStaffManagementService>();

builder.Services.AddScoped<IStudentExperienceFormMapper, StudentExperienceFormMapper>();
builder.Services.AddScoped<IStudentExperienceFormService, StudentExperienceFormService>();
builder.Services.AddScoped<IStudentExperienceFormManagementService, StudentExperienceFormManagementService>();

builder.Services.AddScoped<IFormIconService, FormIconService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IExcelDataPreparationService, ExcelDataPreparationService>();
builder.Services.AddScoped<IExcelDownloadService, ExcelDownloadService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();






// Register StatusService with injected status title only
builder.Services.AddScoped<IStatusService>(sp =>
{
    // Retrieve the StatusSettings configuration values from the DI container
    var config = sp.GetRequiredService<IOptions<StatusSettings>>().Value;

    // Resolve IDataAccess dependency from the DI container
    var dataAccess = sp.GetRequiredService<IDataAccess>();

    // Resolve ConnectionStringData dependency from the DI container
    var connectionStringData = sp.GetRequiredService<ConnectionStringData>();

    // Create and return a new instance of StatusService with all required dependencies
    // including the submitted status title from configuration
    return new StatusService(dataAccess, connectionStringData, config.SubmittedTitle);
});


builder.Services.AddSingleton<IWhitespaceTrimmer, WhitespaceTrimmer>();
builder.Services.AddSingleton<ISpecialCharacterStripper, SpecialCharacterStripper>();
builder.Services.AddSingleton<IXssPrevention, XssPrevention>();
builder.Services.AddSingleton<IInputSanitizer, InputSanitizer>();




var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.Use(async (context, next) =>
    {
        string currentUser = Environment.UserName.ToLower();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, $"COLLEGE\\{currentUser}")
        };

        var identity = new ClaimsIdentity(claims, "Development"); // Authentication type
        context.User = new ClaimsPrincipal(identity);

        await next();
    });
}
else
{
    // Enable this temporarily to see actual errors in production
    app.UseDeveloperExceptionPage(); // <-- TEMPORARY

    // Global exception handler for unhandled server-side errors (e.g., null references, runtime exceptions).
    // This middleware catches exceptions and routes the request to the /Error/500 endpoint.
    // Typically used to display a custom 500 Internal Server Error page.
//    app.UseExceptionHandler("/Error/500");

    // Middleware for handling common HTTP status codes (e.g., 404 Not Found, 403 Forbidden).
    // It rewrites the request path to /Error/{statusCode}, allowing you to serve user-friendly error views
    // without changing the URL in the browser.
//    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Inject custom middleware into the request pipeline to track active users.
// This middleware uses the HttpContext to identify the current user (e.g., via Windows auth or ClaimsPrincipal)
// and notifies the UserTrackerService to register their presence. This can be used to show how many users
// are currently active on the site.
//
// Important: Place this AFTER app.UseRouting() so route data is available,
// and BEFORE app.UseAuthorization() so it runs for all users, including unauthenticated ones if desired.
app.UseMiddleware<UserTrackingMiddleware>();


// Enables custom error handling for non-success status codes (e.g. 404, 403)
// Instead of showing a blank page or default browser message, this middleware
// rewrites the request path to /Error/{statusCode}, which can be handled by your ErrorController
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

