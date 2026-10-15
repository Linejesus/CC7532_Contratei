using Contratei.Data.Repositories;
using Contratei.Domain.Interfaces.Repositories;
using Contratei.Service.Professionals;
using Microsoft.Extensions.DependencyInjection;
using Contratei.Service.Appointments;
using Contratei.Service.Availability;
using Contratei.Service.Contractors;

namespace Contratei.CrossCutting;

public static class DependencyInjection
{
    public static IServiceCollection AddContrateiServices(
        this IServiceCollection services)
    {
        services.AddSingleton<
            IProfessionalRepository,
            ProfessionalRepository>();

        services.AddSingleton<
            IAppointmentRepository, 
            AppointmentRepository>();

        services.AddSingleton<
            IProfessionalAvailabilityRepository,
            ProfessionalAvailabilityRepository>();
        
        services.AddSingleton<
            IContractorRepository, 
            ContractorRepository>();

        services.AddScoped<ProfessionalService>();

        services.AddScoped<AppointmentService>();
        
        services.AddScoped<ProfessionalAvailabilityService>();

        services.AddScoped<ContractorService>();

        services.AddScoped<ProfessionalCatalogService>();

        return services;
    }
}