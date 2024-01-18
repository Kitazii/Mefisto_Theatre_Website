using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K_Burns_Assessment_2.Models
{
    public class Member : User
    {
        public MemberType MemberType { get; set; }

        //Navigational Properties
    }

    public enum MemberType
    {
        Premium,
        Standard
    }
}