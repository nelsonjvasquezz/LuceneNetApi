using LuceneNetApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDocumentoGestionadoService>(
    new DocumentoGestionadoService(
        Path.Combine(builder.Environment.ContentRootPath, "DocumentosGestionados", "lucene_index")));
builder.Services.AddSingleton<IIndexService>(
    new ExpedienteService(
        Path.Combine(builder.Environment.ContentRootPath, "Expedientes", "lucene_index")));
builder.Services.AddSingleton<IIndexService>(
    new CandidatoService(
        Path.Combine(builder.Environment.ContentRootPath, "Candidatos", "lucene_index")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
