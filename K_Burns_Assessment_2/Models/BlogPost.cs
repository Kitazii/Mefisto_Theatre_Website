using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace K_Burns_Assessment_2.Models
{
    public class BlogPost
    {
        [Key]
        public int BlogPostID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "Blog Posted")]
        public bool BlogPosted { get; set; }

        [Display(Name = "Date Posted")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")] //Format as ShortDateTime
        public DateTime DatePosted { get; set; }

        [Display(Name = "Date Edited")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")] //Format as ShortDateTime
        public DateTime DateEdited { get; set; }


        //Navigational Properties - BLOGPOST & CATEGORY
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }//ONE


        //Navigational Properties - BLOGPOST & COMMENT
        public List<Comment> Comments { get; set; }//MANY
        public string NewCommentText { get; set; }


        //Navigational Properties - BLOGPOST & USER
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }//ONE

        // Method to truncate a string to a maximum number of words
        public string TruncateWords(string input, int wordLimit)
        {
            //ensures to return if empty or has empty spacing
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            //splits the input string into an array of words using a space as the delimiter.
            //if there are any empty back to back spaces we remove it before storing the word
            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //if the words length is less than the wordLimit then we return the string as it is
            if (words.Length <= wordLimit)
            {
                return input;
            }

            //otherwise we join the array, truncate the words and add a "..." to the end of the string
            return string.Join(" ", words.Take(wordLimit)) + "...";
        }
    }
}