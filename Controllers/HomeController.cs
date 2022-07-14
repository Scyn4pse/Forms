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

        //==========================================================  F O R M  1 A  ==========================================================
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

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
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
                        entry = date_entry.ToString("MM/dd/yyyy"),
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

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
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
                        entry = date_entry.ToString("MM/dd/yyyy"),
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
        public ActionResult EditItemForm1B(string filename)
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

        public ActionResult Form_1c()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_1c(form_1C model)
        {
            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from form_1C order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
            }
            form_1C form = new form_1C
                {
                    entry = model.entry,
                    year = model.year,
                    month_year1 = model.month_year1,
                    month_year2 = model.month_year2,
                    reason = model.reason,
                    name_of_person = model.name_of_person,
                    date_of_birth = model.date_of_birth,
                    father_name = model.father_name,
                    mother_name = model.mother_name,
                    issued_name = model.issued_name,
                    officer_name = model.officer_name,
                    officer_title = model.officer_title,
                    verifier_name = model.verifier_name,
                    verifier_title = model.verifier_title,
                    payment = model.payment,
                    or_no = model.or_no,
                    date_paid = model.date_paid,
                    filename = newfilename,
                };

                db.form_1C.Add(form);
                db.SaveChanges();

                int latestId = form.Id;

            FillAcroFieldsForm1C(newfilename);

            return View(model);
        }

        public string FillAcroFieldsForm1C(string newFilename)
        {
            string entry = "";
            string datebirth = "";
            string datepaid = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 1C.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_1C/" + newFilename + ".pdf");

            var q_string = "Select * from form_1C where filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<form_1C>(q_string).FirstOrDefault();
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
                pdfFormFields.SetField("txtYear", r_data.year);
                pdfFormFields.SetField("txtMonthYear1", r_data.month_year1);
                pdfFormFields.SetField("txtMonthYear2", r_data.month_year2);
                pdfFormFields.SetField("txtReason", r_data.reason);
                pdfFormFields.SetField("txtNameofPerson", r_data.name_of_person);
                pdfFormFields.SetField("txtDateofBirth", datebirth);
                pdfFormFields.SetField("txtFather", r_data.father_name);
                pdfFormFields.SetField("txtMother", r_data.mother_name);
                pdfFormFields.SetField("txtIssueName", r_data.issued_name);
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

        public ActionResult SearchItemForm1C()
        {
            List<form_1C> form_1C = db.form_1C.ToList();

            return View(form_1C);
        }

        [HttpGet]
        public ActionResult SearchThisForm1C(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "name":
                    q_item = "Select * from form_1C where name_of_person like '%" + searchItem + "%'";
                    break;
                case "mothname":
                    q_item = "Select * from form_1C where mother_name like '%" + searchItem + "%'";

                    break;
                case "fathname":
                    q_item = "Select * from form_1C where father_name like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<form_1C>(q_item);
            if (r_itemdata.Count() > 0)
            {
                List<SearchResultForm1C> SR = new List<SearchResultForm1C>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);

                    SR.Add(new SearchResultForm1C
                    {
                        name_of_person = d.name_of_person,
                        mother_name = d.mother_name,
                        father_name = d.father_name,
                        entry = date_entry.ToString("MM/dd/yyyy"),
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
        public ActionResult EditItemForm1C(string filename)
        {
            var q_item = "Select * from form_1C where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<form_1C>(q_item).FirstOrDefault();

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
        public ActionResult UpdateItemForm1C(string issued_name, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string year, string name_of_person, string father_name, string mother_name, string month_year1, string month_year2, string reason, string date_of_birth, string filename)
        {
            string q_update = "Update form_1C Set issued_name='" + issued_name + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "',year='" + year + "',name_of_person='" + name_of_person + "',father_name='" + father_name + "',mother_name='" + mother_name + "',month_year1='" + month_year1 + "',month_year2='" + month_year2 + "',reason='" + reason + "',date_of_birth='" + date_of_birth + "' where filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm1C(filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyRecordsForm1C(string filename)
        {
            var q_item = "Select * from form_1C where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<form_1C>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }

        //==========================================================  F O R M  2 A  ==========================================================
        public ActionResult Form_2a()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_2a(Form_2A model)
        {
            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from Form_2A order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
            }
            Form_2A form = new Form_2A
                {
                    entry = model.entry,
                    page = model.page,
                    book = model.book,
                    lcr_reg_no = model.lcr_reg_no,
                    date_of_reg = model.date_of_reg,
                    death_name = model.death_name,
                    sex = model.sex,
                    age = model.age,
                    civil_status = model.civil_status,
                    citizenship = model.citizenship,
                    date_of_death = model.date_of_death,
                    place_of_death = model.place_of_death,
                    cause_of_death = model.cause_of_death,
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

                db.Form_2A.Add(form);
                db.SaveChanges();

                int latestId = form.Id;

            FillAcroFieldsForm2A(newfilename);
            return View(model);
        }

        public string FillAcroFieldsForm2A(string newFilename)
        {
            string entry = "";
            string datepaid = "";
            string datereg = "";
            string datedeath = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 2A.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_2A/" + newFilename + ".pdf");

            var q_string = "Select * from Form_2A where filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<Form_2A>(q_string).FirstOrDefault();
            DateTime date1 = (DateTime)r_data.entry;
            entry = date1.ToString("MM-dd-yyyy");

            try
            {
                DateTime date3 = (DateTime)r_data.date_of_death;
                DateTime date5 = (DateTime)r_data.date_paid;
                DateTime date2 = (DateTime)r_data.date_of_reg;
                datereg = date2.ToString("MM-dd-yyyy");
                datepaid = date5.ToString("MM-dd-yyyy");
                datedeath = date3.ToString("MM-dd-yyyy");
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
                pdfFormFields.SetField("txtDeathName", r_data.death_name);
                pdfFormFields.SetField("txtSex", r_data.sex);
                pdfFormFields.SetField("txtAge", r_data.age);
                pdfFormFields.SetField("txtCivStatus", r_data.civil_status);
                pdfFormFields.SetField("txtCship", r_data.citizenship);
                pdfFormFields.SetField("txtDateOfDeath", datedeath);
                pdfFormFields.SetField("txtPlaceOfDeath", r_data.place_of_death);
                pdfFormFields.SetField("txtCauseOfDeath", r_data.cause_of_death);
                pdfFormFields.SetField("txtIssuedTo", r_data.issued_to);
                pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
                pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
                pdfFormFields.SetField("txtVerifier", r_data.verifier_name);
                pdfFormFields.SetField("txtVerifTitle", r_data.verifier_title);
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

        public ActionResult SearchItemForm2A()
        {
            List<Form_2A> form_2a = db.Form_2A.ToList();

            return View(form_2a);
        }

        [HttpGet]
        public ActionResult SearchThisForm2A(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "lcrno":
                    q_item = "Select * from Form_2A where lcr_reg_no like '%" + searchItem + "%'";
                    break;
                case "name":
                    q_item = "Select * from Form_2A where death_name like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<Form_2A>(q_item);
            if (r_itemdata.Count() > 0)
            {
                List<SearchResultForm2A> SR = new List<SearchResultForm2A>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);

                    SR.Add(new SearchResultForm2A
                    {
                        lcr_reg_no = d.lcr_reg_no,
                        death_name = d.death_name,
                        entry = date_entry.ToString("MM/dd/yyyy"),
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
        public ActionResult EditItemForm2A(string filename)
        {
            var q_item = "Select * from Form_2A where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_2A>(q_item).FirstOrDefault();

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
        public ActionResult UpdateItemForm2A(string issued_to, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string lcr_reg_no, string date_of_reg, string death_name, string sex, string age, string page, string book, string civil_status, string citizenship, string date_of_death, string place_of_death, string cause_of_death, string filename)
        {
            string q_update = "Update Form_2A Set issued_to='" + issued_to + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "',lcr_reg_no='" + lcr_reg_no + "',date_of_reg='" + date_of_reg + "',death_name='" + death_name + "',sex='" + sex + "',age='" + age + "',page='" + page + "',book='" + book + "',civil_status='" + civil_status + "',citizenship='" + citizenship + "',date_of_death='" + date_of_death + "',place_of_death='" + place_of_death + "',cause_of_death='" + cause_of_death + "' where filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm2A(filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyRecordsForm2A(string filename)
        {
            var q_item = "Select * from Form_2A where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_2A>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }

        //==========================================================  F O R M  2 B  ==========================================================

        public ActionResult Form_2b()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_2b(form_2B model)
        {
            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from form_2B order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
            }
            form_2B form = new form_2B
                {
                    entry = model.entry,
                    name = model.name,
                    date_of_death = model.date_of_death,
                    year = model.year,
                    issued_name = model.issued_name,
                    officer_name = model.officer_name,
                    officer_title = model.officer_title,
                    verifier_name = model.verifier_name,
                    verifier_title = model.verifier_title,
                    payment = model.payment,
                    or_no = model.or_no,
                    date_paid = model.date_paid,
                    filename = newfilename
            };

                db.form_2B.Add(form);
                db.SaveChanges();

                int latestId = form.Id;
            FillAcroFieldsForm2B(newfilename);
            return View(model);
        }

        public string FillAcroFieldsForm2B(string newFilename)
        {
            string entry = "";
            string datepaid = "";
            string datedeath = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 2B.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_2B/" + newFilename + ".pdf");

            var q_string = "Select * from form_2B where filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<form_2B>(q_string).FirstOrDefault();
            DateTime date1 = (DateTime)r_data.entry;
            entry = date1.ToString("MM-dd-yyyy");

            try
            {
                DateTime date3 = (DateTime)r_data.date_of_death;
                DateTime date5 = (DateTime)r_data.date_paid;
                datepaid = date5.ToString("MM-dd-yyyy");
                datedeath = date3.ToString("MM-dd-yyyy");
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
                pdfFormFields.SetField("txtDateofDeath", datedeath);
                pdfFormFields.SetField("txtYear", r_data.year);
                pdfFormFields.SetField("txtIssueName", r_data.issued_name);
                pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
                pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
                pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
                pdfFormFields.SetField("txtVerifTitle", r_data.verifier_title);
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

        public ActionResult SearchItemForm2B()
        {
            List<form_2B> form_2b = db.form_2B.ToList();

            return View(form_2b);
        }

        [HttpGet]
        public ActionResult SearchThisForm2B(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "name":
                    q_item = "Select * from form_2B where name like '%" + searchItem + "%'";
                    break;
                case "year":
                    q_item = "Select * from form_2B where year like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<form_2B>(q_item);
            if (r_itemdata.Count() > 0)
            {
                List<SearchResultForm2B> SR = new List<SearchResultForm2B>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);
                    DateTime date_death = Convert.ToDateTime(d.date_of_death);

                    SR.Add(new SearchResultForm2B
                    {
                        name = d.name,
                        year = d.year,
                        date_of_death = date_death.ToString("MM/dd/yyyy"),
                        entry = date_entry.ToString("MM/dd/yyyy"),
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
        public ActionResult EditItemForm2B(string filename)
        {
            var q_item = "Select * from form_2B where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<form_2B>(q_item).FirstOrDefault();

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
        public ActionResult UpdateItemForm2B(string issued_name, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string name, string date_of_death, string year, string filename)
        {
            string q_update = "Update form_2B Set issued_name='" + issued_name + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "',name='" + name + "',date_of_death='" + date_of_death + "',year='" + year + "' where filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm2B(filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyRecordsForm2B(string filename)
        {
            var q_item = "Select * from form_2B where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<form_2B>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }

        //==========================================================  F O R M  3 A  ==========================================================

        public ActionResult Form_3a()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_3a(Form_3A model)
        {
            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from Form_3A order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
            }
            Form_3A form = new Form_3A
                {
                    entry = model.entry,
                    page = model.page,
                    book = model.book,
                    husband_name = model.husband_name,
                    wife_name = model.wife_name,
                    husband_age = model.husband_age,
                    wife_age = model.wife_age,
                    husband_citizenship = model.husband_citizenship,
                    wife_citizenship = model.wife_citizenship,
                    husband_civil_status = model.husband_civil_status,
                    wife_civil_status = model.wife_civil_status,
                    husband_father = model.husband_father,
                    wife_father = model.wife_father,
                    husband_mother = model.husband_mother,
                    wife_mother = model.wife_mother,
                    reg_no = model.reg_no,
                    date_of_reg = model.date_of_reg,
                    date_of_marriage = model.date_of_marriage,
                    place_of_marriage = model.place_of_marriage,
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

                db.Form_3A.Add(form);
                db.SaveChanges();

                int latestId = form.Id;
                FillAcroFieldsForm3A(newfilename);

            return View(model);
        }

        public string FillAcroFieldsForm3A(string newFilename)
        {
            string entry = "";
            string datepaid = "";
            string datereg = "";
            string datemarr = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 3A.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_3A/" + newFilename + ".pdf");

            var q_string = "Select * from Form_3A where filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<Form_3A>(q_string).FirstOrDefault();
            DateTime date1 = (DateTime)r_data.entry;
            entry = date1.ToString("MM-dd-yyyy");

            try
            {
                DateTime date2 = (DateTime)r_data.date_of_reg;
                DateTime date3 = (DateTime)r_data.date_of_marriage;
                DateTime date5 = (DateTime)r_data.date_paid;
                datereg = date2.ToString("MM-dd-yyyy");
                datemarr = date3.ToString("MM-dd-yyyy");
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
                pdfFormFields.SetField("txtHusName", r_data.husband_name);
                pdfFormFields.SetField("txtWifeName", r_data.wife_name);
                pdfFormFields.SetField("txtHusAge", r_data.husband_age);
                pdfFormFields.SetField("txtWifeAge", r_data.wife_age);
                pdfFormFields.SetField("txtHusCship", r_data.husband_citizenship);
                pdfFormFields.SetField("txtWifeCship", r_data.wife_citizenship);
                pdfFormFields.SetField("txtHusCivStatus", r_data.husband_civil_status);
                pdfFormFields.SetField("txtWifeCivStatus", r_data.wife_civil_status);
                pdfFormFields.SetField("txtHusFather", r_data.husband_father);
                pdfFormFields.SetField("txtWifeFather", r_data.wife_father);
                pdfFormFields.SetField("txtHusMother", r_data.husband_mother);
                pdfFormFields.SetField("txtWifeMother", r_data.wife_mother);
                pdfFormFields.SetField("txtRegNo", r_data.reg_no);
                pdfFormFields.SetField("txtDateOfReg", datereg);
                pdfFormFields.SetField("txtDateOfMrg", datemarr);
                pdfFormFields.SetField("txtPlaceOfMrg", r_data.place_of_marriage);
                pdfFormFields.SetField("txtIssuedTo", r_data.issued_to);
                pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
                pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
                pdfFormFields.SetField("txtVerifier", r_data.verifier_name);
                pdfFormFields.SetField("txtVerifTitle", r_data.verifier_title);
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

        public ActionResult SearchItemForm3A()
        {
            List<Form_3A> form_3a = db.Form_3A.ToList();

            return View(form_3a);
        }

        [HttpGet]
        public ActionResult SearchThisForm3A(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "regno":
                    q_item = "Select * from Form_3A where reg_no like '%" + searchItem + "%'";
                    break;
                case "namehus":
                    q_item = "Select * from Form_3A where husband_name like '%" + searchItem + "%'";
                    break;
                case "namewife":
                    q_item = "Select * from Form_3A where wife_name like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<Form_3A>(q_item);
            if (r_itemdata.Count() > 0)
            {
                List<SearchResultForm3A> SR = new List<SearchResultForm3A>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);

                    SR.Add(new SearchResultForm3A
                    {
                        reg_no = d.reg_no,
                        husband_name = d.husband_name,
                        wife_name = d.wife_name,
                        entry = date_entry.ToString("MM/dd/yyyy"),
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
        public ActionResult EditItemForm3A(string filename)
        {
            var q_item = "Select * from Form_3A where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_3A>(q_item).FirstOrDefault();

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
        public ActionResult UpdateItemForm3A(string issued_to, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string page, string book, string husband_name, string wife_name, string husband_age,string wife_age, string husband_citizenship, string wife_citizenship, string husband_civil_status, string wife_civil_status, string husband_father, string wife_father, string husband_mother, string wife_mother, string reg_no, string date_of_reg, string date_of_marriage, string place_of_marriage, string filename)
        {
            string q_update = "Update Form_3A Set issued_to='" + issued_to + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "',page='" + page + "',book='" + book + "',husband_name='" + husband_name + "',wife_name='" + wife_name + "',husband_age='" + husband_age + "',wife_age='" + wife_age + "',husband_citizenship='" + husband_citizenship + "',wife_citizenship='" + wife_citizenship + "',husband_civil_status='" + husband_civil_status + "',wife_civil_status='" + wife_civil_status + "',husband_father='" + husband_father + "',wife_father='" + wife_father + "',husband_mother='" + husband_mother + "',wife_mother='" + wife_mother + "',reg_no='" + reg_no + "',date_of_reg='" + date_of_reg + "',date_of_marriage='" + date_of_marriage + "',place_of_marriage='" + place_of_marriage + "' where filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm3A(filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyRecordsForm3A(string filename)
        {
            var q_item = "Select * from Form_3A where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<Form_3A>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }
        //==========================================================  F O R M  3 B  ==========================================================

        public ActionResult Form_3b()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form_3b(form_3B model)
        {
            string newfilename = "";
            var q_lastfilename = "select  TOP 1 id, filename from form_3B order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            if (res_filename == null)
            {
                newfilename = "00001";
            }
            else
            {
                if (res_filename.filename == null)
                {
                    newfilename = "00001";
                }
                else
                {
                    int decValue = Convert.ToInt32(res_filename.filename, 16) + 1;
                    newfilename = decValue.ToString("X").PadLeft(5, '0').ToUpper();
                }
            }
            form_3B form = new form_3B
                {
                    entry = model.entry,
                    husband_name = model.husband_name,
                    wife_name = model.wife_name,
                    date_of_marriage = model.date_of_marriage,
                    year = model.year,
                    issued_name = model.issued_name,
                    officer_name = model.officer_name,
                    officer_title = model.officer_title,
                    verifier_name = model.verifier_name,
                    verifier_title = model.verifier_title,
                    payment = model.payment,
                    or_no = model.or_no,
                    date_paid = model.date_paid,
                    filename = newfilename
            };

                db.form_3B.Add(form);
                db.SaveChanges();

                int latestId = form.Id;
            FillAcroFieldsForm3B(newfilename);
            return View(model);
        }

        public string FillAcroFieldsForm3B(string newFilename)
        {
            string entry = "";
            string datepaid = "";
            string datemarr = "";

            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 3B.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_3B/" + newFilename + ".pdf");

            var q_string = "Select * from form_3B where filename='" + newFilename + "'";
            var r_data = db.Database.SqlQuery<form_3B>(q_string).FirstOrDefault();
            DateTime date1 = (DateTime)r_data.entry;
            entry = date1.ToString("MM-dd-yyyy");

            try
            {
                DateTime date3 = (DateTime)r_data.date_of_marriage;
                DateTime date5 = (DateTime)r_data.date_paid;
                datepaid = date5.ToString("MM-dd-yyyy");
                datemarr = date3.ToString("MM-dd-yyyy");
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
                pdfFormFields.SetField("txtHusband", r_data.husband_name);
                pdfFormFields.SetField("txtWife", r_data.wife_name);
                pdfFormFields.SetField("txtDateofMarriage", datemarr);
                pdfFormFields.SetField("txtYear", r_data.year);
                pdfFormFields.SetField("txtIssueName", r_data.issued_name);
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

        public ActionResult SearchItemForm3B()
        {
            List<form_3B> form_3b = db.form_3B.ToList();

            return View(form_3b);
        }

        [HttpGet]
        public ActionResult SearchThisForm3B(string selType, string searchItem)
        {
            var q_item = "";

            switch (selType)
            {
                case "husname":
                    q_item = "Select * from form_3B where husband_name like '%" + searchItem + "%'";
                    break;
                case "wifename":
                    q_item = "Select * from form_3B where wife_name like '%" + searchItem + "%'";
                    break;
                case "year":
                    q_item = "Select * from form_3B where year like '%" + searchItem + "%'";
                    break;
                default:
                    break;

            }

            var r_itemdata = db.Database.SqlQuery<form_3B>(q_item);
            if (r_itemdata.Count() > 0)
            {
                List<SearchResultForm3B> SR = new List<SearchResultForm3B>();

                foreach (var d in r_itemdata)
                {
                    DateTime date_entry = Convert.ToDateTime(d.entry);
                    DateTime date_marr = Convert.ToDateTime(d.date_of_marriage);

                    SR.Add(new SearchResultForm3B
                    {
                        husband_name = d.husband_name,
                        wife_name = d.wife_name,
                        year = d.year,
                        date_of_marriage = date_marr.ToString("MM/dd/yyyy"),
                        entry = date_entry.ToString("MM/dd/yyyy"),
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
        public ActionResult EditItemForm3B(string filename)
        {
            var q_item = "Select * from form_3B where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<form_3B>(q_item).FirstOrDefault();

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
        public ActionResult UpdateItemForm3B(string issued_name, string officer_name, string officer_title, string verifier_name, string verifier_title, string payment, string or_no, string date_paid, string husband_name, string wife_name, string date_of_marriage, string year, string filename)
        {
            string q_update = "Update form_3B Set issued_name='" + issued_name + "', officer_name='" + officer_name + "',officer_title='" + officer_title + "', verifier_name='" + verifier_name + "', verifier_title='" + verifier_title + "',payment='" + payment + "', or_no='" + or_no + "',date_paid='" + date_paid + "',husband_name='" + husband_name + "',wife_name='" + wife_name + "',date_of_marriage='" + date_of_marriage + "',year='" + year + "' where filename='" + filename + "'";
            db.Database.ExecuteSqlCommand(q_update);

            FillAcroFieldsForm3B(filename);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CopyRecordsForm3B(string filename)
        {
            var q_item = "Select * from form_3B where filename = '" + filename + "'";
            var r_data = db.Database.SqlQuery<form_3B>(q_item).FirstOrDefault();

            if (r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }

        //==========================================================  F O R M  3 C  ==========================================================




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
        //==========================================================  G E T L A S T P D F F I L E N A M E  ==========================================================
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
        [HttpGet]
        public ActionResult GetLastPDFFilenameForm1C()
        {
            var q_lastfilename = "select TOP 1 id, filename from form_1C order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetLastPDFFilenameForm2A()
        {
            var q_lastfilename = "select TOP 1 id, filename from Form_2A order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetLastPDFFilenameForm2B()
        {
            var q_lastfilename = "select TOP 1 id, filename from form_2B order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetLastPDFFilenameForm3A()
        {
            var q_lastfilename = "select TOP 1 id, filename from Form_3A order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetLastPDFFilenameForm3B()
        {
            var q_lastfilename = "select TOP 1 id, filename from form_3B order by id desc";
            var res_filename = db.Database.SqlQuery<FilenameInfo>(q_lastfilename).FirstOrDefault();

            return Json(res_filename.filename, JsonRequestBehavior.AllowGet);

        }
    }
}

//=========================================================================  C L A S S   =========================================================================
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

public class SearchResultForm1C
{
    public string name_of_person { get; set; }
    public string mother_name { get; set; }
    public string father_name { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }

}
public class SearchResultForm2A
{
    public string lcr_reg_no { get; set; }
    public string death_name { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }

}
public class SearchResultForm2B
{
    public string name { get; set; }
    public string date_of_death { get; set; }
    public string year { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }

}
public class SearchResultForm3A
{
    public string reg_no { get; set; }
    public string husband_name { get; set; }
    public string wife_name { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }

}
public class SearchResultForm3B
{
    public string husband_name { get; set; }
    public string wife_name { get; set; }
    public string date_of_marriage { get; set; }
    public string year { get; set; }
    public string entry { get; set; }
    public string filename { get; set; }

}