using Microsoft.AspNetCore.Http.HttpResults;
using MVC.Models;

namespace MVC.Data
{
    public interface IRepository_mini
    {
        // Pour les posts
        Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post);
        Task<Results<Ok<List<PostReadDTO>>, NotFound>> GetAllPosts();
        Task<Results<Ok<PostReadDTO>, NotFound>> GetPostById(Guid id);
        Task<Results<Ok, NotFound>> IncrementPostLike(Guid id);
        Task<Results<Ok, NotFound>> IncrementPostDislike(Guid id);

        // Pour les commentaires
        Task<Results<Created<CommentReadDTO>, BadRequest, InternalServerError>> CreateAPIComment(Comment comment);
        Task<Results<Ok<List<CommentReadDTO>>, NotFound>> GetCommentsByPostId(Guid postId);
        Task<Results<Ok, NotFound>> IncrementCommentLike(Guid id);
        Task<Results<Ok, NotFound>> IncrementCommentDislike(Guid id);
    }
}
