using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public class EFRepository_mini<TContext> : IRepository_mini where TContext : DbContext
    {
        protected readonly TContext _context;

        protected EFRepository_mini(TContext context)
        {
            this._context = context;
        }

        // Ajouter un Post
        public virtual async Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post)
        {
            try
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return TypedResults.Created($"/Posts/{post.Id}", new PostReadDTO(post));
            }
            catch (Exception ex) when (ex is DbUpdateException)
            {
                return TypedResults.BadRequest();
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        }

        // Recuperer tous les Posts
        public async Task<Results<Ok<List<PostReadDTO>>, NotFound>> GetAllPosts()
        {
            var posts = await _context.Set<Post>().ToListAsync();
            if (!posts.Any()) return TypedResults.NotFound();
            return TypedResults.Ok(posts.Select(p => new PostReadDTO(p)).ToList());
        }

        // Recuperer un Post par ID
        public async Task<Results<Ok<PostReadDTO>, NotFound>> GetPostById(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            return post is not null ? TypedResults.Ok(new PostReadDTO(post)) : TypedResults.NotFound();
        }

        // Incrementer les likes sur un Post
        public async Task<Results<Ok, NotFound>> IncrementPostLike(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            if (post is null) return TypedResults.NotFound();

            post.IncrementLike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }

        // Incrementer les dislikes sur un Post
        public async Task<Results<Ok, NotFound>> IncrementPostDislike(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            if (post is null) return TypedResults.NotFound();

            post.IncrementDislike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }

        // Ajouter un Commentaire
        public async Task<Results<Created<CommentReadDTO>, BadRequest, InternalServerError>> CreateAPIComment(Comment comment)
        {
            try
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return TypedResults.Created($"/Comments/{comment.Id}", new CommentReadDTO(comment));
            }
            catch (Exception ex) when (ex is DbUpdateException)
            {
                return TypedResults.BadRequest();
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        }

        // Recuperer les commentaires d’un Post
        public async Task<Results<Ok<List<CommentReadDTO>>, NotFound>> GetCommentsByPostId(Guid postId)
        {
            var comments = await _context.Set<Comment>().Where(c => c.PostId == postId).ToListAsync();
            return comments.Any() ? TypedResults.Ok(comments.Select(c => new CommentReadDTO(c)).ToList()) : TypedResults.NotFound();
        }

        // Incrementer les likes sur un Commentaire
        public async Task<Results<Ok, NotFound>> IncrementCommentLike(Guid id)
        {
            var comment = await _context.Set<Comment>().FindAsync(id);
            if (comment is null) return TypedResults.NotFound();

            comment.IncrementLike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }

        // Incrementer les dislikes sur un Commentaire
        public async Task<Results<Ok, NotFound>> IncrementCommentDislike(Guid id)
        {
            var comment = await _context.Set<Comment>().FindAsync(id);
            if (comment is null) return TypedResults.NotFound();

            comment.IncrementDislike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }
    }
}
