using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using form_1a.Models;


namespace form_1a.Controllers
{
    public class HomeController : Controller
    {
        Form_1AEntities1 db = new Form_1AEntities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form_1a()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Form_1a(Form_1A model)
        {
            try
            {
                Form_1A form = new Form_1A
                {
                    page = model.page,
                    book = model.book,
                    entry = model.entry,
                    lcr_reg_no = model.lcr_reg_no,
                    date_of_reg = model.date_of_reg,
                    name_child = model.name_child,
                    sex = model.sex,
                    date_of_birth = model.date_of_birth,
                    place_of_birth = model.place_of_birth,
                    name_of_mother = model.name_of_mother,
                    citizenship_of_mother = model.citizenship_of_mother,
                    name_of_father = model.name_of_father,
                    citizenship_of_father = model.citizenship_of_father,
                    date_of_marriage_of_parents = model.date_of_marriage_of_parents,
                    place_of_marriage_of_parents = model.place_of_marriage_of_parents,
                    issued_to = model.issued_to,
                    officer_name = model.officer_name,
                    officer_title = model.officer_title,
                    verifier_name = model.verifier_name,
                    verifier_title = model.verifier_title,
                    payment = model.payment,
                    or_no = model.or_no,
                    date_paid = model.date_paid
                };

                db.Form_1A.Add(form);
                db.SaveChanges();

                int latestId = form.Id;
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        public ActionResult Form_1b()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_1b(Form_1B model)
        {
            try
            { 
                Form_1B form = new Form_1B
                {
                    entry = model.entry,
                    name = model.name,
                    date_of_birth = model.date_of_birth,
                    father = model.father,
                    mother = model.mother,
                    year = model.year,
                    issued_to = model.issued_to,
                    officer_name = model.officer_name,
                    officer_title = model.officer_title,
                    verifier_name = model.verifier_name,
                    verifier_title = model.verifier_title,
                    payment = model.payment,
                    or_no = model.or_no,
                    date_paid = model.date_paid
                };

                db.Form_1B.Add(form);
                db.SaveChanges();

                int latestId = form.Id;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        public ActionResult Form_1c()
        {
            return View();
        }
        public ActionResult Form_2a()
        {
            return View();
        }
        public ActionResult Form_2b()
        {
            return View();
        }
        public ActionResult Form_3a()
        {
            return View();
        }
        public ActionResult Form_3b()
        {
            return View();
        }
        public ActionResult Form_3c()
        {
            return View();
        }
        //
        //public ActionResult SaveEntry(string lcrno, string regdate, string childname,string sex, string dateofbirth, string placeofbirth, string mothername, string mothercship, string fathername, string fathercship, string dateofmrg, string placeofmrg, string issuedto, string officername, string officertitle, string verifiername, string verifiertitle, string payment, string orno, string datepaid)
        //{
        //    Form_1AEntities1 db = new Form_1AEntities1();

        //    DateTime today = DateTime.Now;
        //    string datenow = today.ToString("yyyy-M-dd hh:mm:ss");

        //    string q_exist = "Select * from Form_1A where lcr_reg_no = '" + lcrno + "'";
        //    var d_return = db.Database.SqlQuery<Form_1A>(q_exist).FirstOrDefault();

        //    if(d_return != null)
        //    {
        //        return Json("Exist", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        string q_insert = "Insert into Form_1A (date_entry,lcr_reg_no,date_of_reg,name_child,sex,date_of_birth,place_of_birth,name_of_mother,citizenship_of_mother,name_of_father,citizenship_of_father,date_of_marriage,place_of_marriage,issued_to,officer_name,officer_title,verifier_name,verifier_title,payment,or_no,date_paid) VALUES('" + datenow + "','" + lcrno + "','" + regdate + "','" + childname + "','" + sex + "','" + dateofbirth + "','" + placeofbirth + "','" + mothername + "','" + mothercship + "','" + fathername + "','" + fathercship + "','" + dateofmrg + "','" + placeofmrg + "','" + issuedto + "','" + officername + "','" + officertitle + "','" + verifiername + "','" + verifiertitle + "','" + payment + "','" + orno + "','" + datepaid + "')";
        //        var d_return2 = db.Database.ExecuteSqlCommand(q_insert);

        //        return Json("Success", JsonRequestBehavior.AllowGet);
        //    }


        //}

        //public ActionResult SaveRecord(Form_1A model)
        //{


        //}



    }
}