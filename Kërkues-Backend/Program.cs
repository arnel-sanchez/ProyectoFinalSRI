using Kërkues_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<ISearchEngine, SearchEngine>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(option =>
{
    option.AllowAnyOrigin();
    option.AllowAnyHeader();
    option.AllowAnyMethod();
});

app.MapControllers();

//Búsqueda en el conjunto de prueba Cranfield
//Corpus.CorpusLoad(app.Configuration["FileLocationTestCranfield"], true);

//Búsqueda en el conjunto de prueba Medline
//Corpus.CorpusLoad(app.Configuration["FileLocationTestMedline"], true);

//Búsqueda en el conjunto de prueba ADI
//Corpus.CorpusLoad(app.Configuration["FileLocationTestADI"], true);

//Búsqueda en el conjunto de prueba 20NewsGroup
//Corpus.CorpusLoad(app.Configuration["FileLocationTest20NewsGroup"]);

//Búsqueda en archivos reales
Corpus.CorpusLoad(app.Configuration["FileLocation"]);

app.Run();