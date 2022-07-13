using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using form_1a.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace form_1a.Controllers
{
    public class HomeController : Controller
    {
        Form_1AEntities1 db = new Form_1AEntities1();

        public ActionResult Index()
        {
            return View();
        }

        //=========================================================== F O R M  1 A =================================================================
        public ActionResult Form_1a()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Form_1a(Form_1A model)
        {

            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from Form_1A order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if(res_filename.filename == null)
            {
                newfilename = "00001";
            }
            else
            {
              
                int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                newfilename = decValue.ToString("X").PadLeft(5,'0').ToUpper();
            }

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
                    date_paid = model.date_paid,
                    filename = newfilename
                };
            
                db.Form_1A.Add(form);
                db.SaveChanges();

                int latestId = form.Id;

                FillAcroFieldsForm1A(form.lcr_reg_no, newfilename);

            return View(model);
        }
        public string FillAcroFieldsForm1A(string lcr_no, string newFilename)
        {

            string entry = "";
            string datereg = "";
            string datebirth = "";
            string datemarr = "";
            string datepaid = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 1A.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_1A/" + newFilename + ".pdf");

            var q_string = "Select * from FORM_1A where lcr_reg_no='" + lcr_no + "' AND filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<Form_1A>(q_string).FirstOrDefault();
            DateTime date1 = (DateTime)r_data.entry;
            entry = date1.ToString("MM-dd-yyyy");
            try
            {
                DateTime date2 = (DateTime)r_data.date_of_reg;
                DateTime date3 = (DateTime)r_data.date_of_birth;
                DateTime date4 = (DateTime)r_data.date_of_marriage_of_parents;
                DateTime date5 = (DateTime)r_data.date_paid;

                datereg = date2.ToString("MM-dd-yyyy");
                datebirth = date3.ToString("MM-dd-yyyy");
                datemarr = date4.ToString("MM-dd-yyyy");
                datepaid = date5.ToString("MM-dd-yyyy");
            }
            catch
            {

            }



            if (r_data != null)
            {
                PdfReader pdfReader = new PdfReader(form_template);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


                AcroFields pdfFormFields = pdfStamper.AcroFields;

                pdfFormFields.SetField("txtDate", entry);
                pdfFormFields.SetField("txtPage", r_data.page);
                pdfFormFields.SetField("txtBookNo", r_data.book);
                pdfFormFields.SetField("txtLCRNo", r_data.lcr_reg_no);
                pdfFormFields.SetField("txtRegDate", datereg);
                pdfFormFields.SetField("txtChildName", r_data.name_child);
                pdfFormFields.SetField("txtSex", r_data.sex);
                pdfFormFields.SetField("txtDateOfBirth", datebirth);
                pdfFormFields.SetField("txtPlaceOfBirth", r_data.place_of_birth);
                pdfFormFields.SetField("txtMotherName", r_data.name_of_mother);
                pdfFormFields.SetField("txtMotherCship", r_data.citizenship_of_mother);
                pdfFormFields.SetField("txtFatherName", r_data.name_of_father);
                pdfFormFields.SetField("txtFatherCship", r_data.citizenship_of_father);
                pdfFormFields.SetField("txtDateOfMrg", datemarr);
                pdfFormFields.SetField("txtPlaceofMrg", r_data.place_of_marriage_of_parents);
                pdfFormFields.SetField("txtIssuedTo", r_data.issued_to);
                pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
                pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
                pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
                pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
                pdfFormFields.SetField("txtPayment", r_data.payment);
                pdfFormFields.SetField("txtORNo", r_data.or_no);
                pdfFormFields.SetField("txtDatePaid", datepaid);


                pdfStamper.FormFlattening = true;
                pdfStamper.Close();
                pdfReader.Close();

                return "Success";
            }
            else
            {
                return "Failed/Not Exist";
            }

        }


        public ActionResult SearchItem()
        {
            List<Form_1A> form_1a = db.Form_1A.ToList();

            return View(form_1a);
        }

        [HttpGet]
        public ActionResult SearchThis(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "lcrno":
                    q_item = "Select * from FORM_1A where lcr_reg_no like '%" + searchItem + "%'";
                    break;
                case "chname":
                    q_item = "Select * from FORM_1A where name_child like '%" + searchItem + "%'";
                    break;
                case "mothname":
                    q_item = "Select * from FORM_1A where name_of_mother like '%" + searchItem + "%'";

                    break;
                case "fathname":
                    q_item = "Select * from FORM_1A where name_of_father like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<Form_1A>(q_item);



            if (r_itemdata.Count() > 0)
            {
                List<SearchResult> SR = new List<SearchResult>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);

                    SR.Add(new SearchResult
                    {
                        lcr_reg_no = d.lcr_reg_no,
                        name_child = d.name_child,
                        name_of_mother = d.name_of_mother,
                        name_of_father = d.name_of_father,
                        entry = date_entry.ToString("MM-dd-yyyy"),
                        filename = d.filename
                    });
                }

                return Json(SR.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult EditItem(string lcr_no, string filename)
        {
            var q_item = "Select * from FORM_1A where lcr_reg_no = '" + lcr_no + "' and filename='" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_1A>(q_item).FirstOrDefault();
            
            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }


        [HttpGet]
        public ActionResult UpdateItem(string issued_to, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string page, string book, string lcr_reg_no, string date_of_reg, string name_child, string sex, string date_of_birth, string place_of_birth, string name_of_mother, string citizenship_of_mother, string name_of_father, string citizenship_of_father, string date_of_marriage_of_parents, string place_of_marriage_of_parents, string filename)
        {
            string q_update = "Update Form_1A Set issued_to='" + issued_to + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "', page='" + page + "',book='" + book + "', date_of_reg='" + date_of_reg + "',name_child='" + name_child + "', sex='" + sex + "',date_of_birth='" + date_of_birth + "', place_of_birth='" + place_of_birth + "',name_of_mother='" + name_of_mother + "', citizenship_of_mother='" + citizenship_of_mother + "',name_of_father='" + name_of_father + "', citizenship_of_father='" + citizenship_of_father + "',date_of_marriage_of_parents='" + date_of_marriage_of_parents + "', place_of_marriage_of_parents='" + place_of_marriage_of_parents + "' where lcr_reg_no='" + lcr_reg_no + "' and filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm1A(lcr_reg_no, filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyRecords(string lcr_no, string filename)
        {
            var q_item = "Select * from FORM_1A where lcr_reg_no = '" + lcr_no + "' and filename='" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_1A>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }



        //==========================================================  F O R M  1 B  ==========================================================
        public ActionResult Form_1b()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_1b(Form_1B model)
        {
            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from Form_1B order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if (res_filename.filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
            }

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
                date_paid = model.date_paid,
                filename = newfilename
            };

            db.Form_1B.Add(form);
            db.SaveChanges();

            int latestId = form.Id;

            FillAcroFieldsForm1B(newfilename);

            return View(model);
        }
        public string FillAcroFieldsForm1B(string newFilename)
        {
            string entry = "";
            string datebirth = "";
            string datepaid = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 1B.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_1B/" + newFilename + ".pdf");

            var q_string = "Select * from FORM_1B where filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<Form_1B>(q_string).FirstOrDefault();
            DateTime date1 = (DateTime)r_data.entry;
            entry = date1.ToString("MM-dd-yyyy");

            try
            {
                DateTime date3 = (DateTime)r_data.date_of_birth;
                DateTime date5 = (DateTime)r_data.date_paid;

                datebirth = date3.ToString("MM-dd-yyyy");
                datepaid = date5.ToString("MM-dd-yyyy");
            }
            catch
            {

            }

            if (r_data != null)
            {
                PdfReader pdfReader = new PdfReader(form_template);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


                AcroFields pdfFormFields = pdfStamper.AcroFields;

                pdfFormFields.SetField("txtDate", entry);
                pdfFormFields.SetField("txtName", r_data.name);
                pdfFormFields.SetField("txtDateofBirth", datebirth);
                pdfFormFields.SetField("txtFather", r_data.father);
                pdfFormFields.SetField("txtMother", r_data.mother);
                pdfFormFields.SetField("txtYear", r_data.year);
                pdfFormFields.SetField("txtIssueName", r_data.issued_to);
                pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
                pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
                pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
                pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
                pdfFormFields.SetField("txtPayment", r_data.payment);
                pdfFormFields.SetField("txtORno", r_data.or_no);
                pdfFormFields.SetField("txtDatePaid", datepaid);

                pdfStamper.FormFlattening = true;
                pdfStamper.Close();
                pdfReader.Close();

                return "Success";
            }
            else
            {
                return "Failed/Not Exist";
            }

        }

        public ActionResult SearchItemForm1B()
        {
            List<Form_1B> form_1b = db.Form_1B.ToList();

            return View(form_1b);
        }

        [HttpGet]
        public ActionResult SearchThisForm1B(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "name":
                    q_item = "Select * from FORM_1B where name like '%" + searchItem + "%'";
                    break;
                case "mothname":
                    q_item = "Select * from FORM_1B where mother like '%" + searchItem + "%'";

                    break;
                case "fathname":
                    q_item = "Select * from FORM_1B where father like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<Form_1B>(q_item);
            if (r_itemdata.Count() > 0)
            {
                List<SearchResultForm1B> SR = new List<SearchResultForm1B>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);

                    SR.Add(new SearchResultForm1B
                    {
                        name = d.name,
                        mother = d.mother,
                        father = d.father,
                        entry = date_entry.ToString("MM-dd-yyyy"),
                        filename = d.filename
                    });
                }

                return Json(SR.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult EditItemForm1B(string lcr_no, string filename)
        {
            var q_item = "Select * from FORM_1B where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_1B>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }


        [HttpGet]
        public ActionResult UpdateItemForm1B(string issued_to, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string name, string date_of_birth, string mother, string father, string year, string filename)
        {
            string q_update = "Update Form_1B Set issued_to='" + issued_to + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "',name='" + name + "',date_of_birth='" + date_of_birth + "',mother='" + mother + "',father='" + father + "', year ='" + year + "' where filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm1B(filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CopyRecordsForm1B(string filename)
        {
            var q_item = "Select * from FORM_1B where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_1B>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }

        //==========================================================  F O R M  1 C  ==========================================================

        //public ActionResult Form_1c()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Form_1c(form_1C model)
        //{

        //    try
        //    {
        //        form_1C form = new form_1C
        //        {
        //            Id = model.Id,
        //            entry = model.entry,
        //            year = model.year,
        //            month_year1 = model.month_year1/* + "_" + nd*/,
        //            month_year2 = model.month_year2,
        //            reason = model.reason,
        //            name_of_person = model.name_of_person,
        //            date_of_birth = model.date_of_birth,
        //            father_name = model.father_name,
        //            mother_name = model.mother_name,
        //            issued_name = model.issued_name,
        //            officer_name = model.officer_name,
        //            officer_title = model.officer_title,
        //            verifier_name = model.verifier_name,
        //            verifier_title = model.verifier_title,
        //            payment = model.payment,
        //            or_no = model.or_no,
        //            date_paid = model.date_paid,

        //        };

        //        //lcr_no = form.lcr_reg_no.ToString();

        //        db.form_1C.Add(form);
        //        db.SaveChanges();

        //        int latestId = form.Id;



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}
        //public ActionResult Form_2a()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Form_2a(Form_2A model)
        //{

        //    try
        //    {
        //        Form_2A form = new Form_2A
        //        {
        //            Id = model.Id,
        //            entry = model.entry,
        //            page = model.page,
        //            book = model.book/* + "_" + nd*/,
        //            lcr_reg_no = model.lcr_reg_no,
        //            date_of_reg = model.date_of_reg,
        //            death_name = model.death_name,
        //            sex = model.sex,
        //            age = model.age,
        //            civil_status = model.civil_status,
        //            citizenship = model.citizenship,
        //            date_of_death = model.date_of_death,
        //            place_of_death = model.place_of_death,
        //            cause_of_death = model.cause_of_death,
        //            issued_to = model.issued_to,
        //            officer_name = model.officer_name,
        //            officer_title = model.officer_title,
        //            verifier_name = model.verifier_name,
        //            verifier_title = model.verifier_title,
        //            payment = model.payment,
        //            or_no = model.or_no,
        //            date_paid = model.date_paid
        //        };



        //        db.Form_2A.Add(form);
        //        db.SaveChanges();

        //        int latestId = form.Id;



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}
        //public ActionResult Form_2b()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Form_2b(form_2B model)
        //{

        //    try
        //    {
        //        form_2B form = new form_2B
        //        {
        //            Id = model.Id,
        //            entry = model.entry,
        //            name = model.name,
        //            date_of_death = model.date_of_death/* + "_" + nd*/,
        //            year = model.year,
        //            issued_name = model.issued_name,
        //            officer_name = model.officer_name,
        //            officer_title = model.officer_title,
        //            verifier_name = model.verifier_name,
        //            verifier_title = model.verifier_title,
        //            payment = model.payment,
        //            or_no = model.or_no,
        //            date_paid = model.date_paid
        //        };



        //        db.form_2B.Add(form);
        //        db.SaveChanges();

        //        int latestId = form.Id;



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}

        //public ActionResult Form_3a()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Form_3a(Form_3A model)
        //{

        //    try
        //    {
        //        Form_3A form = new Form_3A
        //        {
        //            Id = model.Id,
        //            entry = model.entry,
        //            page = model.page,
        //            book = model.book/* + "_" + nd*/,
        //            husband_name = model.husband_name,
        //            wife_name = model.wife_name,
        //            husband_age = model.husband_age,
        //            wife_age = model.wife_age,
        //            husband_citizenship = model.husband_citizenship,
        //            wife_citizenship = model.wife_citizenship,
        //            husband_civil_status = model.husband_civil_status,
        //            wife_civil_status = model.wife_civil_status,
        //            husband_father = model.husband_father,
        //            wife_father = model.wife_father,
        //            husband_mother = model.husband_mother,
        //            wife_mother = model.wife_mother,
        //            reg_no = model.reg_no,
        //            date_of_reg = model.date_of_reg,
        //            date_of_marriage = model.date_of_marriage,
        //            place_of_marriage = model.place_of_marriage,
        //            issued_to = model.issued_to,
        //            officer_name = model.officer_name,
        //            officer_title = model.officer_title,
        //            verifier_name = model.verifier_name,
        //            verifier_title = model.verifier_title,
        //            payment = model.payment,
        //            or_no = model.or_no,
        //            date_paid = model.date_paid
        //        };



        //        db.Form_3A.Add(form);
        //        db.SaveChanges();

        //        int latestId = form.Id;


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}

        //public ActionResult Form_3b()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Form_3b(form_3B model)
        //{

        //    try
        //    {
        //        form_3B form = new form_3B
        //        {
        //            Id = model.Id,
        //            entry = model.entry,
        //            husband_name = model.husband_name,
        //            wife_name = model.wife_name/* + "_" + nd*/,
        //            date_of_marriage = model.date_of_marriage,
        //            year = model.year,
        //            issued_name = model.issued_name,
        //            officer_name = model.officer_name,
        //            officer_title = model.officer_title,
        //            verifier_name = model.verifier_name,
        //            verifier_title = model.verifier_title,
        //            payment = model.payment,
        //            or_no = model.or_no,
        //            date_paid = model.date_paid
        //        };



        //        db.form_3B.Add(form);
        //        db.SaveChanges();

        //        int latestId = form.Id;



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}

        //public ActionResult Form_3c()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Form_3c(form_3C model)
        //{

        //    try
        //    {
        //        form_3C form = new form_3C
        //        {
        //            Id = model.Id,
        //            entry = model.entry,
        //            year = model.year,
        //            month_year1 = model.month_year1/* + "_" + nd*/,
        //            month_year2 = model.month_year2,
        //            reason = model.reason,
        //            husband_name = model.husband_name,
        //            wife_name = model.wife_name,
        //            place_of_marriage = model.place_of_marriage,
        //            issued_name = model.issued_name,
        //            officer_name = model.officer_name,
        //            officer_title = model.officer_title,
        //            verifier_name = model.verifier_name,
        //            verifier_title = model.verifier_title,
        //            payment = model.payment,
        //            or_no = model.or_no,
        //            date_paid = model.date_paid
        //        };



        //        db.form_3C.Add(form);
        //        db.SaveChanges();

        //        int latestId = form.Id;



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(model);
        //}







        ////public string FillAcroFieldsForm1C(string lcr_no)
        ////{
        ////    DateTime newdate = DateTime.Now;
        ////    string nd = newdate.ToString("MM-dd-yyyy");

        ////    string filename = lcr_no + "_" + nd;

        ////    var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 1C.pdf");
        ////    var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_1C/" + filename + ".pdf");

        ////    var q_string = "Select * from FORM_1C where lcr_reg_no='" + lcr_no + "'";
        ////    var r_data = db.Database.SqlQuery<form_1C>(q_string).FirstOrDefault();

        ////    if (r_data != null)
        ////    {
        ////        PdfReader pdfReader = new PdfReader(form_template);
        ////        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


        ////        AcroFields pdfFormFields = pdfStamper.AcroFields;

        ////        pdfFormFields.SetField("txtDate", r_data.entry.ToString());
        ////        pdfFormFields.SetField("txtYear", r_data.year);
        ////        pdfFormFields.SetField("txtMonthYear1", r_data.month_year1);
        ////        pdfFormFields.SetField("txtMonthYear2", r_data.month_year2);
        ////        pdfFormFields.SetField("txtReason", r_data.reason);
        ////        pdfFormFields.SetField("txtNameofPerson", r_data.name_of_person);
        ////        pdfFormFields.SetField("txtDateofBirth", r_data.date_of_birth.ToString());
        ////        pdfFormFields.SetField("txtFather", r_data.father_name);
        ////        pdfFormFields.SetField("txtMother", r_data.mother_name);
        ////        pdfFormFields.SetField("txtIssuedTo", r_data.issued_name);
        ////        pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
        ////        pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
        ////        pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
        ////        pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
        ////        pdfFormFields.SetField("txtPayment", r_data.payment);
        ////        pdfFormFields.SetField("txtORNo", r_data.or_no);
        ////        pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());

        ////        pdfStamper.FormFlattening = true;
        ////        pdfStamper.Close();
        ////        pdfReader.Close();

        ////        return "Success";
        ////    }
        ////    else
        ////    {
        ////        return "Failed/Not Exist";
        ////    }

        ////}
        ////SAME PROBLEM HERE
        //public string FillAcroFieldsForm2A(string lcr_no)
        //{
        //    DateTime newdate = DateTime.Now;
        //    string nd = newdate.ToString("MM-dd-yyyy");

        //    string filename = lcr_no + "_" + nd;

        //    var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 2A.pdf");
        //    var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_2A/" + filename + ".pdf");

        //    var q_string = "Select * from FORM_2A where lcr_reg_no='" + lcr_no + "'";
        //    var r_data = db.Database.SqlQuery<Form_2A>(q_string).FirstOrDefault();

        //    if (r_data != null)
        //    {
        //        PdfReader pdfReader = new PdfReader(form_template);
        //        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


        //        AcroFields pdfFormFields = pdfStamper.AcroFields;

        //        pdfFormFields.SetField("txtDate", r_data.entry.ToString());
        //        pdfFormFields.SetField("txtPage", r_data.page);
        //        pdfFormFields.SetField("txtBookNo", r_data.book);
        //        pdfFormFields.SetField("txtLCRNo", r_data.lcr_reg_no);
        //        pdfFormFields.SetField("txtRegDate", r_data.date_of_reg.ToString());
        //        pdfFormFields.SetField("txtDeathName", r_data.death_name);
        //        pdfFormFields.SetField("txtSex", r_data.sex);
        //        pdfFormFields.SetField("txtAge", r_data.age);
        //        pdfFormFields.SetField("txtCivStatus", r_data.civil_status);
        //        pdfFormFields.SetField("txtCship", r_data.citizenship);
        //        pdfFormFields.SetField("txtDateOfDeath", r_data.date_of_death.ToString());
        //        pdfFormFields.SetField("txtPlaceOfDeath", r_data.place_of_death);
        //        pdfFormFields.SetField("txtCauseOfDeath", r_data.cause_of_death);
        //        pdfFormFields.SetField("txtIssuedTo", r_data.issued_to);
        //        pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
        //        pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
        //        pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
        //        pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
        //        pdfFormFields.SetField("txtPayment", r_data.payment);
        //        pdfFormFields.SetField("txtORNo", r_data.or_no);
        //        pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());

        //        pdfStamper.FormFlattening = true;
        //        pdfStamper.Close();
        //        pdfReader.Close();

        //        return "Success";
        //    }
        //    else
        //    {
        //        return "Failed/Not Exist";
        //    }

        //}

        ////public string FillAcroFieldsForm2B(string lcr_no)
        ////{
        ////    DateTime newdate = DateTime.Now;
        ////    string nd = newdate.ToString("MM-dd-yyyy");

        ////    string filename = lcr_no + "_" + nd;

        ////    var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 2B.pdf");
        ////    var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_2B/" + filename + ".pdf");

        ////    var q_string = "Select * from FORM_2B where lcr_reg_no='" + lcr_no + "'";
        ////    var r_data = db.Database.SqlQuery<form_2B>(q_string).FirstOrDefault();

        ////    if (r_data != null)
        ////    {
        ////        PdfReader pdfReader = new PdfReader(form_template);
        ////        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


        ////        AcroFields pdfFormFields = pdfStamper.AcroFields;

        ////        pdfFormFields.SetField("txtDate", r_data.entry.ToString());
        ////        pdfFormFields.SetField("txtName", r_data.name);
        ////        pdfFormFields.SetField("txtDateofDeath", r_data.date_of_death.ToString());
        ////        pdfFormFields.SetField("txtYear", r_data.year);
        ////        pdfFormFields.SetField("txtIssuedTo", r_data.issued_name);
        ////        pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
        ////        pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
        ////        pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
        ////        pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
        ////        pdfFormFields.SetField("txtPayment", r_data.payment);
        ////        pdfFormFields.SetField("txtORNo", r_data.or_no);
        ////        pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());

        ////        pdfStamper.FormFlattening = true;
        ////        pdfStamper.Close();
        ////        pdfReader.Close();

        ////        return "Success";
        ////    }
        ////    else
        ////    {
        ////        return "Failed/Not Exist";
        ////    }

        ////}

        ////public string FillAcroFieldsForm3A(string lcr_no)
        ////{
        ////    DateTime newdate = DateTime.Now;
        ////    string nd = newdate.ToString("MM-dd-yyyy");

        ////    string filename = lcr_no + "_" + nd;

        ////    var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 3A.pdf");
        ////    var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_3A/" + filename + ".pdf");

        ////    var q_string = "Select * from FORM_3A where lcr_reg_no='" + lcr_no + "'";
        ////    var r_data = db.Database.SqlQuery<Form_3A>(q_string).FirstOrDefault();

        ////    if (r_data != null)
        ////    {
        ////        PdfReader pdfReader = new PdfReader(form_template);
        ////        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


        ////        AcroFields pdfFormFields = pdfStamper.AcroFields;

        ////        pdfFormFields.SetField("txtDate", r_data.entry.ToString());
        ////        pdfFormFields.SetField("txtPage", r_data.page);
        ////        pdfFormFields.SetField("txtBookNo", r_data.book);
        ////        pdfFormFields.SetField("txtHusName", r_data.husband_name);
        ////        pdfFormFields.SetField("txtWifeName", r_data.wife_name);
        ////        pdfFormFields.SetField("txtHusAge", r_data.husband_age);
        ////        pdfFormFields.SetField("txtWifeAge", r_data.wife_age);
        ////        pdfFormFields.SetField("txtHusCship", r_data.husband_citizenship);
        ////        pdfFormFields.SetField("txtWifeCship", r_data.wife_citizenship);
        ////        pdfFormFields.SetField("txtHusCivStatus", r_data.husband_civil_status);
        ////        pdfFormFields.SetField("txtWifeCivStatus", r_data.wife_civil_status);
        ////        pdfFormFields.SetField("txtHusFather", r_data.husband_father);
        ////        pdfFormFields.SetField("txtWifeFather", r_data.wife_father);
        ////        pdfFormFields.SetField("txtHusMother", r_data.husband_mother);
        ////        pdfFormFields.SetField("txtWifeMother", r_data.wife_mother);
        ////        pdfFormFields.SetField("txtRegNo", r_data.reg_no);
        ////        pdfFormFields.SetField("txtDateOfReg", r_data.date_of_reg.ToString());
        ////        pdfFormFields.SetField("txtDateOfMrg", r_data.date_of_marriage.ToString());
        ////        pdfFormFields.SetField("txtPlaceOfMrg", r_data.place_of_marriage);
        ////        pdfFormFields.SetField("txtIssuedTo", r_data.issued_to);
        ////        pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
        ////        pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
        ////        pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
        ////        pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
        ////        pdfFormFields.SetField("txtPayment", r_data.payment);
        ////        pdfFormFields.SetField("txtORNo", r_data.or_no);
        ////        pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());

        ////        pdfStamper.FormFlattening = true;
        ////        pdfStamper.Close();
        ////        pdfReader.Close();

        ////        return "Success";
        ////    }
        ////    else
        ////    {
        ////        return "Failed/Not Exist";
        ////    }

        ////}
        ////public string FillAcroFieldsForm3B(string lcr_no)
        ////{
        ////    DateTime newdate = DateTime.Now;
        ////    string nd = newdate.ToString("MM-dd-yyyy");

        ////    string filename = lcr_no + "_" + nd;

        ////    var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 3B.pdf");
        ////    var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_3B/" + filename + ".pdf");

        ////    var q_string = "Select * from FORM_3B where lcr_reg_no='" + lcr_no + "'";
        ////    var r_data = db.Database.SqlQuery<form_3B>(q_string).FirstOrDefault();

        ////    if (r_data != null)
        ////    {
        ////        PdfReader pdfReader = new PdfReader(form_template);
        ////        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


        ////        AcroFields pdfFormFields = pdfStamper.AcroFields;

        ////        pdfFormFields.SetField("txtDate", r_data.entry.ToString());
        ////        pdfFormFields.SetField("txtHusband", r_data.husband_name);
        ////        pdfFormFields.SetField("txtWife", r_data.wife_name);
        ////        pdfFormFields.SetField("txtDateofMarriage", r_data.date_of_marriage.ToString());
        ////        pdfFormFields.SetField("txtYear", r_data.year);
        ////        pdfFormFields.SetField("txtIssueName", r_data.issued_name);
        ////        pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
        ////        pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
        ////        pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
        ////        pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
        ////        pdfFormFields.SetField("txtPayment", r_data.payment);
        ////        pdfFormFields.SetField("txtORno", r_data.or_no);
        ////        pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());

        ////        pdfStamper.FormFlattening = true;
        ////        pdfStamper.Close();
        ////        pdfReader.Close();

        ////        return "Success";
        ////    }
        ////    else
        ////    {
        ////        return "Failed/Not Exist";
        ////    }

        ////}

        ////public string FillAcroFieldsForm3C(string lcr_no)
        ////{
        ////    DateTime newdate = DateTime.Now;
        ////    string nd = newdate.ToString("MM-dd-yyyy");

        ////    string filename = lcr_no + "_" + nd;

        ////    var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 3C.pdf");
        ////    var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_3C/" + filename + ".pdf");

        ////    var q_string = "Select * from FORM_3C where lcr_reg_no='" + lcr_no + "'";
        ////    var r_data = db.Database.SqlQuery<form_3C>(q_string).FirstOrDefault();

        ////    if (r_data != null)
        ////    {
        ////        PdfReader pdfReader = new PdfReader(form_template);
        ////        PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


        ////        AcroFields pdfFormFields = pdfStamper.AcroFields;

        ////        pdfFormFields.SetField("txtDate", r_data.entry.ToString());
        ////        pdfFormFields.SetField("txtYear", r_data.year);
        ////        pdfFormFields.SetField("txtMonthYear1", r_data.month_year1);
        ////        pdfFormFields.SetField("txtMonthYear2", r_data.month_year2);
        ////        pdfFormFields.SetField("txtReason", r_data.reason);
        ////        pdfFormFields.SetField("txtHusband", r_data.husband_name);
        ////        pdfFormFields.SetField("txtWife", r_data.wife_name);
        ////        pdfFormFields.SetField("txtPlaceofMarriage", r_data.place_of_marriage);
        ////        pdfFormFields.SetField("txtIssuedTo", r_data.issued_name);
        ////        pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
        ////        pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
        ////        pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
        ////        pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
        ////        pdfFormFields.SetField("txtPayment", r_data.payment);
        ////        pdfFormFields.SetField("txtORNo", r_data.or_no);
        ////        pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());

        ////        pdfStamper.FormFlattening = true;
        ////        pdfStamper.Close();
        ////        pdfReader.Close();

        ////        return "Success";
        ////    }
        ////    else
        ////    {
        ////        return "Failed/Not Exist";
        ////    }

        ////}

        [HttpGet]
        public ActionResult GetLastPDFFilename()
        {
            var q_lastfilename = "select TOP 1 id, filename from Form_1A order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetLastPDFFilenameForm1B()
        {
            var q_lastfilename = "select TOP 1 id, filename from Form_1B order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);
        }
    }
}


public class FilenameInfo{
    public int id { get; set; }
    public string filename { get; set; }
}


public class SearchResult
{
    public string lcr_reg_no { get; set; }
    public string name_child { get; set; }
    public string name_of_mother { get; set; }
    public string name_of_father { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }


}

public class SearchResultForm1B
{
    public string name { get; set; }
    public string mother { get; set; }
    public string father { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }

}