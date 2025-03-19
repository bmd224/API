using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{

    // 
    // Pendant notre cours nous allons créer une application qui s'apparente a Facebook/Instagram, l'application va nous permettre d'envoyé des photos, d'ajouter des commentaires et des likes.
    // Les structures suivantes démontres en partie les schema que nous allons utiliser
    //

    public class Comment
    {
        // Référence pour les Accessor ( Get, Set, Init )
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-properties
        //

        [Key]
        // Ajouter pour NoSQL
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // Utilisation du Guid pour etre mieux aligner avec NoSQL
        public Guid Id { get; init; }

        [Required(ErrorMessage = "SVP entrer votre commentaire")]
        [MaxLength(128)]
        [Display(Name = "Commentaire")]
        public required string Commentaire { get; init; }

        [Required(ErrorMessage = "SVP entrer un nom d'utilisateur")]
        [MaxLength(128)]
        [Display(Name = "Nom de l'utilisateur")]
        public required string User { get; init; }

        [Display(Name = "Like")]
        public int Like { get; private set; } = 0;

        [Display(Name = "Dislike")]
        public int Dislike { get; private set; } = 0;

        [Display(Name = "Date de création")]
        public DateTime Created { get; init; } = DateTime.Now;

        [Display(Name = "Contenue revisé ?")]
        public bool IsApproved { get; private set; } = false;

        public bool IsDeleted { get; private set; } = false;


        public override string ToString()
        {
            return $"===============" + Environment.NewLine +
                $"Comment : {Commentaire}" + Environment.NewLine +
                $"User : {User}" + Environment.NewLine +
                $"Like : {Like}" + Environment.NewLine +
                $"Dislike : {Dislike}" + Environment.NewLine +
                $"Created : {Created}" + Environment.NewLine +
                $"===============";
        }

        public void IncrementLike()
        {
            Like++;
        }

        public void IncrementDislike()
        {
            Dislike++;
        }

        public void Approve()
        {
            IsApproved = true;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

        // référence pour ForeignKey ...
        public Post Post { get; set; } = null!;

        public Guid PostId { get; set; }

        //public int? ParentCommentId { get; set; }

        //public Comment? ParentComment { get; set; }

        //public ICollection<Comment> SubComments { get; private set; } = new List<Comment>();
    }
}
