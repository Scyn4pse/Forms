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

        public ActionResult Form_1a()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Form_1a(Form_1A model)
        {
            //string lcr_no = "";
            //DateTime newdate = DateTime.Now;
            //string nd = newdate.ToString("MM-dd-yyyy");

            try
            {
                Form_1A form = new Form_1A
                {
                    page = model.page,
                    book = model.book,
                    entry = model.entry,
                    lcr_reg_no = model.lcr_reg_no/* + "_" + nd*/,
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

                //lcr_no = form.lcr_reg_no.ToString();

                db.Form_1A.Add(form);
                db.SaveChanges();

                int latestId = form.Id;

                FillAcroFieldsForm1A(form.lcr_reg_no);

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
        
        public string FillAcroFieldsForm1A(string lcr_no)
        {
            DateTime newdate = DateTime.Now;
            string nd = newdate.ToString("MM-dd-yyyy");

            string filename = lcr_no + "_" + nd;
          
            var form_template = Server.MapPath("/FORM_TEMPLATE/FORM NO. 1A.pdf");
            var output_pdf = Server.MapPath("/OUTPUT_PDF/FORM_1A/" + filename + ".pdf");

            var q_string = "Select * from FORM_1A where lcr_reg_no='" + lcr_no + "'";
            var r_data = db.Database.SqlQuery<Form_1A>(q_string).FirstOrDefault();

            if (r_data != null)
            {
                PdfReader pdfReader = new PdfReader(form_template);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(output_pdf, FileMode.Create));


                AcroFields pdfFormFields = pdfStamper.AcroFields;

                pdfFormFields.SetField("txtDate", r_data.entry.ToString());
                pdfFormFields.SetField("txtPage", r_data.page);
                pdfFormFields.SetField("txtBookNo", r_data.book);
                pdfFormFields.SetField("txtLCRNo", r_data.lcr_reg_no);
                pdfFormFields.SetField("txtRegDate", r_data.date_of_reg.ToString());
                pdfFormFields.SetField("txtChildName", r_data.name_child);
                pdfFormFields.SetField("txtSex", r_data.sex);
                pdfFormFields.SetField("txtDateOfBirth", r_data.date_of_birth.ToString());
                pdfFormFields.SetField("txtPlaceOfBirth", r_data.place_of_birth);
                pdfFormFields.SetField("txtMotherName", r_data.name_of_mother);
                pdfFormFields.SetField("txtMotherCship", r_data.citizenship_of_mother);
                pdfFormFields.SetField("txtFatherName", r_data.name_of_father);
                pdfFormFields.SetField("txtFatherCship", r_data.citizenship_of_father);
                pdfFormFields.SetField("txtDateOfMrg", r_data.date_of_marriage_of_parents.ToString());
                pdfFormFields.SetField("txtPlaceofMrg", r_data.place_of_marriage_of_parents);
                pdfFormFields.SetField("txtIssuedTo", r_data.issued_to);
                pdfFormFields.SetField("txtOfficerName", r_data.officer_name);
                pdfFormFields.SetField("txtOfficerTitle", r_data.officer_title);
                pdfFormFields.SetField("txtVerifierName", r_data.verifier_name);
                pdfFormFields.SetField("txtVerifierTitle", r_data.verifier_title);
                pdfFormFields.SetField("txtPayment", r_data.payment);
                pdfFormFields.SetField("txtORNo", r_data.or_no);
                pdfFormFields.SetField("txtDatePaid", r_data.date_paid.ToString());


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

            if(r_itemdata.Count() > 0)
            {
                return Json(r_itemdata.ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

            
        }

        [HttpGet]
        public ActionResult EditItem(string lcr_no)
        {
            var q_item = "Select * from FORM_1A where lcr_reg_no = '" + lcr_no + "'";
            var r_data = db.Database.SqlQuery<Form_1A>(q_item).FirstOrDefault();

            if(r_data != null)
            {
                return View(r_data);
            }
            else
            {
                return View();
            }
        }

        public ActionResult EditItemForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UpdateItem(string lcr_reg_no, string name_child, string book)
        {
            string q_update = "Update Form_1A Set name_child='" + name_child + "', book='" + book + "' where lcr_reg_no='" + lcr_reg_no + "'";
            db.Database.ExecuteSqlCommand(q_update);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


    }
}