using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K_Burns_Assessment_2.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public string CommentText { get; set; }

        [Display(Name = "Date Posted")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")] //Format as ShortDateTime
        public DateTime DatePosted { get; set; }

        //Navigational Properties - COMMENT & BLOGPOST
        [ForeignKey("BlogPost")]
        public int BlogPostID { get; set; }
        public BlogPost BlogPost { get; set; }//ONE

        //Navigational Properties - COMMENT & USER
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }//ONE
    }
}