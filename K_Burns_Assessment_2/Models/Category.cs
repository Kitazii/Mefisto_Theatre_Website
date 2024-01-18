using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace K_Burns_Assessment_2.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Display(Name = "Category Name")]
        public string Name { get; set; }

        //Navigational Properties - CATEGORY & BLOGPOST
        public List<BlogPost> BlogPosts { get; set; }//MANY
    }
}