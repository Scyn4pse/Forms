//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace form_1a.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Form_2A
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> entry { get; set; }
        public string page { get; set; }
        public string book { get; set; }
        public string lcr_reg_no { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_of_reg { get; set; }
        public string death_name { get; set; }
        public string sex { get; set; }
        public string age { get; set; }
        public string civil_status { get; set; }
        public string citizenship { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_of_death { get; set; }
        public string place_of_death { get; set; }
        public string cause_of_death { get; set; }
        public string issued_to { get; set; }
        public string officer_name { get; set; }
        public string officer_title { get; set; }
        public string verifier_name { get; set; }
        public string verifier_title { get; set; }
        public string payment { get; set; }
        public string or_no { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_paid { get; set; }
    }
}