

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Business;
using MVC.Data;
using MVC.Models;

var builder = WebApplication.CreateBuilder(args);

//services necessaires a l application
builder.Services.AddOpenApi();

// Configuration de la bd en memoire
builder.Services.AddDbContext<ApplicationDbContextInMemory>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddScoped<IRepository_mini, EFRepository_mini_InMemory>();

// Configuration de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<BlobController>(); //BlobController
builder.Services.AddSwaggerGen(c =>
    c.OperationFilter<FileUploadOperationFilter>()
);

var app = builder.Build();

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Définition des routes API
// Route pour ajouter un Post avec upload d image
app.MapPost("/Posts/Add",
    [Consumes("multipart/form-data")]
async Task<IResult> (
        HttpRequest request,
        [FromServices] IRepository_mini repo,
        [FromServices] BlobController blobController,
        [FromForm] PostCreateDTO postDto
    ) =>
    {
        try
        {
            var form = await request.ReadFormAsync();

            // Extraction et validation des donnees requises
            string title = form["title"];
            string user = form["user"];
            string categoryString = form["category"];
            var image = form.Files["image"];

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(categoryString) || image is null)
                return Results.BadRequest("Tous les champs (titre, utilisateur, categorie, image) sont obligatoires.");

            // Validation de la categorie
            if (!Enum.TryParse(categoryString, out Category category))
                return Results.BadRequest("Categorie invalide.");

            Guid imageGuid = Guid.NewGuid();
            string imageUrl = await blobController.PushImageToBlob(image, imageGuid);

            // Creation d un nouvel objet Post
            var newPost = new Post
            {
                Title = title,
                Category = category,
                User = user,
                BlobImage = imageGuid,
                Url = imageUrl
            };

            return await repo.CreateAPIPost(newPost);
        }
        catch (ExceptionFilesize)
        {
            return Results.BadRequest("L'image dépasse la taille autorisée.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout du post : {ex.Message}");
            return Results.Problem("Erreur interne lors du traitement de la requête.");
        }
    }).DisableAntiforgery();

// Routes pour les Posts
//Recuperer tous les Posts
app.MapGet("/Posts", async (IRepository_mini repo) => await repo.GetAllPosts());

//Recuperer un Post par son ID
app.MapGet("/Posts/{id}", async (Guid id, IRepository_mini repo) => await repo.GetPostById(id));

//Ajouter un like a un Post
app.MapPost("/Posts/{id}/like", async (Guid id, IRepository_mini repo) => await repo.IncrementPostLike(id));

//Ajouter un dislike a un Post
app.MapPost("/Posts/{id}/dislike", async (Guid id, IRepository_mini repo) => await repo.IncrementPostDislike(id));


// Routes pour les Commentaires
//Ajouter un Commentaire
app.MapPost("/Comments/Add", async ([FromBody] CommentCreateDTO dto, IRepository_mini repo) =>
{
    var comment = CommentCreateDTO.GetComment(dto);
    return await repo.CreateAPIComment(comment);
});
//Recuperer les commentaires d’un Post
app.MapGet("/Comments/{postId}", async (Guid postId, IRepository_mini repo) => await repo.GetCommentsByPostId(postId));

//Ajouter un like à un Commentaire
app.MapPost("/Comments/{id}/like", async (Guid id, IRepository_mini repo) => await repo.IncrementCommentLike(id));

//Ajouter un dislike à un Commentaire
app.MapPost("/Comments/{id}/dislike", async (Guid id, IRepository_mini repo) => await repo.IncrementCommentDislike(id));

app.Run();
