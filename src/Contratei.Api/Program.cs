using Contratei.CrossCutting;
using Contratei.Service.Professionals;
using Contratei.Service.Appointments;
using Contratei.Service.Availability;
using Contratei.Service.Contractors;
//using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddContrateiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//if (app.Environment.IsDevelopment())
//{
//    app.MapGet("/dev/test-supabase", async (IConfiguration configuration) =>
//    {
//        var connectionString =
//            configuration.GetConnectionString("Supabase");
//
//        if (string.IsNullOrWhiteSpace(connectionString))
//        {
//            return Results.Problem(
//                "A connection string Supabase não foi configurada.");
//        }
//
//        try
//        {
//            await using var connection =
//                new NpgsqlConnection(connectionString);
//
//            await connection.OpenAsync();
//
//            await using var command =
//                new NpgsqlCommand("SELECT 1", connection);
//
//            var result = await command.ExecuteScalarAsync();
//
//            return Equals(result, 1)
//                ? Results.Ok(new
//                {
//                    connected = true,
//                    message = "Conexão com o Supabase realizada com sucesso."
//                })
//                : Results.Problem(
//                    "A conexão foi aberta, mas o teste não retornou o resultado esperado.");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(
//                $"Erro no teste de conexão com o Supabase: {ex}");
//
//            return Results.Problem(
//                title: "Falha ao conectar ao Supabase",
//                detail: ex is NpgsqlException
//                    ? $"Erro do PostgreSQL: {ex.Message}"
//                    : $"Erro inesperado: {ex.GetType().Name}",
//                statusCode: 500);
//        }
//    });
//}








app.MapPost(
    "/contractors",
    async (
        CreateContractorRequest request,
        ContractorService contractorService,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var id = await contractorService.CreateAsync(
                request, cancellationToken);

            return Results.Created(
                $"/contractors/{id}",
                new { id });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }
);

app.MapGet(
    "/contractors",
    async (
        ContractorService contractorService,
        CancellationToken cancellationToken) =>
    {
        var contractors = await contractorService.GetAllAsync(
            cancellationToken);

        return Results.Ok(contractors);
    }
);

app.MapGet(
    "/contractors/{id:guid}",
    async (
        Guid id,
        ContractorService contractorService,
        CancellationToken cancellationToken) =>
    {
        var contractor = await contractorService.GetByIdAsync(
            id, cancellationToken);

        return contractor is null
            ? Results.NotFound(new {
                message = "Contratante não encontrado."
              })
            : Results.Ok(contractor);
    }
);


app.MapPost(
    "/professionals",
    async (
        CreateProfessionalRequest request,
        ProfessionalService professionalService) =>
    {
        try
        {
            var id = await professionalService.CreateAsync(request);

            return Results.Created(
                $"/professionals/{id}",
                new { id });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new
            {
                message = ex.Message
            });
        }
    }
);


app.MapGet(
    "/professionals",
    async (ProfessionalService professionalService) =>
    {
        var professionals = await professionalService.GetAllAsync();

        return Results.Ok(professionals);
    }
);


app.MapGet(
    "/professionals/{id:guid}",
    async (
        Guid id,
        ProfessionalService professionalService) =>
    {
        var professional = await professionalService.GetByIdAsync(id);

        if (professional is null)
        {
            return Results.NotFound(new
            {
                message = "Profissional não encontrado."
            });
        }

        return Results.Ok(professional);
    }
);


app.MapGet(
    "/professionals/search",
    async (
        double? latitude,
        double? longitude,
        double? radiusKm,
        string? areaAtuacao,
        ProfessionalService professionalService) =>
    {
        try
        {
            var results = await professionalService.SearchAsync(
                latitude,
                longitude,
                radiusKm,
                areaAtuacao);

            return Results.Ok(results.Select(result => new
            {
                id = result.Professional.Id,
                name = result.Professional.Name,
                description = result.Professional.Description,
                areaAtuacao = result.Professional.AreaAtuacao,
                city = result.Professional.Address.City,
                state = result.Professional.Address.State,
                latitude = result.Professional.Address.Latitude,
                longitude = result.Professional.Address.Longitude,
                distanceKm = result.DistanceKm.HasValue
                    ? Math.Round(result.DistanceKm.Value, 2)
                    : (double?)null
            }));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new
            {
                message = ex.Message
            });
        }
    }
);



app.MapPost(
    "/professionals/{professionalId:guid}/services",
    async (
        Guid professionalId,
        CreateProfessionalServiceRequest request,
        ProfessionalCatalogService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var created = await service.CreateAsync(
                professionalId,
                request,
                cancellationToken);

            return Results.Created(
                $"/professionals/{professionalId}/services/{created.Id}",
                created);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
);

app.MapGet(
    "/professionals/{professionalId:guid}/services",
    async (
        Guid professionalId,
        ProfessionalCatalogService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var services = await service.GetByProfessionalIdAsync(
                professionalId,
                cancellationToken);

            return Results.Ok(services);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
);




app.MapPost(
    "/professionals/{professionalId:guid}/availability",
    async (
        Guid professionalId,
        ConfigureAvailabilityRequest request,
        ProfessionalAvailabilityService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var availability = await service.ConfigureAsync(
                professionalId,
                request,
                cancellationToken);

            return Results.Created(
                $"/professionals/{professionalId}/availability",
                availability);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }
);


app.MapGet(
    "/professionals/{professionalId:guid}/availability",
    async (
        Guid professionalId,
        ProfessionalAvailabilityService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var availability = await service.GetWeeklyAvailabilityAsync(
                professionalId,
                cancellationToken);

            return Results.Ok(availability);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
);


app.MapGet(
    "/professionals/{professionalId:guid}/availability/slots",
    async (
        Guid professionalId,
        DateOnly date,
        int durationMinutes,
        ProfessionalAvailabilityService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var slots = await service.GetAvailableSlotsAsync(
                professionalId,
                date,
                durationMinutes,
                cancellationToken);

            return Results.Ok(slots);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }
);


app.MapDelete(
    "/professionals/{professionalId:guid}/availability/{availabilityId:guid}",
    async (
        Guid professionalId,
        Guid availabilityId,
        ProfessionalAvailabilityService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var availabilities = await service.GetWeeklyAvailabilityAsync(
                professionalId,
                cancellationToken);

            if (!availabilities.Any(a => a.Id == availabilityId))
            {
                return Results.NotFound(new
                {
                    message = "Disponibilidade não encontrada para este profissional."
                });
            }

            await service.DeactivateAsync(
                availabilityId,
                cancellationToken);

            return Results.Ok(new
            {
                message = "Disponibilidade desativada."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
);


app.MapPost(
    "/appointments",
    async (
        CreateAppointmentRequest request,
        AppointmentService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var appointment = await service.CreateAsync(
                request,
                cancellationToken);

            return Results.Created(
                $"/appointments/{appointment.Id}",
                appointment);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }
);


app.MapGet(
    "/appointments/{appointmentId:guid}",
    async (
        Guid appointmentId,
        AppointmentService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var appointment = await service.GetByIdAsync(
                appointmentId,
                cancellationToken);

            return Results.Ok(appointment);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }
);


app.MapPatch(
    "/appointments/{appointmentId:guid}/cancel",
    async (
        Guid appointmentId,
        AppointmentService service,
        CancellationToken cancellationToken) =>
    {
        try
        {
            await service.CancelAsync(
                appointmentId,
                cancellationToken);

            return Results.Ok(new
            {
                message = "Agendamento cancelado."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }
);


app.MapGet(
    "/contractors/{contractorId:guid}/appointments",
    async (
        Guid contractorId,
        AppointmentService service,
        CancellationToken cancellationToken) =>
    {
        var appointments = await service.GetByContractorIdAsync(
            contractorId,
            cancellationToken);

        return Results.Ok(appointments);
    }
);


app.MapGet(
    "/professionals/{professionalId:guid}/appointments",
    async (
        Guid professionalId,
        AppointmentService service,
        CancellationToken cancellationToken) =>
    {
        var appointments = await service.GetByProfessionalIdAsync(
            professionalId,
            cancellationToken);

        return Results.Ok(appointments);
    }
);



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
