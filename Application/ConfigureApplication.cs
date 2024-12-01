using System.Reflection;
using Application.Common.Behaviours;
using Application.Security;
using Application.Security.JwtHelper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureApplication
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddSingleton(typeof(IPasswordHasher), typeof(PasswordHasher));
        services.AddSingleton(typeof(IJwtService), typeof(JwtService));
    }
}