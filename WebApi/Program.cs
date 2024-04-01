using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hellang.Middleware.ProblemDetails;
using Infrastructure;
using Application;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
#region  Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console(LogEventLevel.Debug, builder.Configuration["Logging:OutputTemplate"], theme: SystemConsoleTheme.Colored)
    .WriteTo.File(Path.Combine(AppContext.BaseDirectory, builder.Configuration["Logging:Dir"]),
    rollingInterval: RollingInterval.Day, fileSizeLimitBytes: null).CreateLogger();

builder.Host.UseSerilog();
#endregion

#region Controllers
builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
#endregion

#region Authentication and Authorization


#endregion

#region Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);
#endregion

#region Application
builder.Services.AddApplication();
#endregion

#region Swagger
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(
    options =>
    {
        var securitySchema = new OpenApiSecurityScheme
        {
            Description =
            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        options.AddSecurityDefinition("Bearer", securitySchema);

        var securityRequirement = new OpenApiSecurityRequirement
        {
             { securitySchema, new[] { "Bearer" } }
        };
        options.AddSecurityRequirement(securityRequirement);


        options.OperationFilter<SwaggerDefaultValues>();

        var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        // integrate xml comments
        //options.IncludeXmlComments(filePath);
    });

#endregion

#region Versioning

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(
                    options =>
                    {
                        // reporting api versions will return the headers
                        // "api-supported-versions" and "api-deprecated-versions"
                        options.ReportApiVersions = true;
                        options.ApiVersionReader = new UrlSegmentApiVersionReader();
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.Policies.Sunset(0.9)
                                        .Effective(DateTimeOffset.Now.AddDays(60))
                                        .Link("policy.html")
                                            .Title("Versioning Policy")
                                            .Type("text/html");
                    })
                .AddMvc()
                .AddApiExplorer(
                    options =>
                    {
                        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                        // note: the specified format code will format the version as "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";
                        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                        // can also be used to control the format of the API version in route templates
                        options.SubstituteApiVersionInUrl = true;
                    });
#endregion

#region Routing

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

#endregion

#region Redirection & HSTS

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

#endregion

#region HTTPContext
builder.Services.AddHttpContextAccessor();
#endregion

#region Compression

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

#endregion

#region CORS

if (isDevelopment)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

#endregion CORS

var app = builder.Build();
var apiVersionDescriptionDescriber = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
            options.SwaggerEndpoint($"swagger/{description.GroupName}/swagger.json",
                $"{builder.Configuration["Swagger:Name"]} V{description.ApiVersion}");

        options.DocumentTitle = builder.Configuration["Swagger:DocumentTitle"];
        options.RoutePrefix = builder.Configuration["Swagger:RoutePrefix"];
    });

    app.UseCors("AllowAll");
}

if (app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseHsts(options => options.MaxAge(365).IncludeSubdomains());
    app.UseXContentTypeOptions();
    app.UseReferrerPolicy(opts => opts.NoReferrer());
    app.UseXXssProtection(options => options.EnabledWithBlockMode());
    app.UseXfo(options => options.Deny());
    app.UseCsp(opts => opts
        .BlockAllMixedContent()
        .StyleSources(s => s.Self())
        .StyleSources(s => s.UnsafeInline())
        .FontSources(s => s.Self())
        .FormActions(s => s.Self())
        .FrameAncestors(s => s.Self())
        .ImageSources(s => s.Self())
        .ScriptSources(s => s.Self())
    );

    app.Use(async (ctx, next) =>
    {
        ctx.Response.Headers.Add("Permissions-Policy",
            "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");
        await next();
    });
}

app.UseProblemDetails();

app.UseHttpsRedirection();

app.UseStaticFiles();

//app.UseSpaStaticFiles();

app.UseAuthentication();

app.UseResponseCompression();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseSpa(spa => { spa.Options.SourcePath = "qrMiddlewareUi"; });

app.Run();
