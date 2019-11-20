using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PdfFormFiller.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        [HttpGet]
        public ActionResult Get()
        {
            //var reader = new PdfReader("ESDC-EMP5624.pdf");
            //var writer = new PdfWriter("ESDC-EMP5624_filled.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            //reader.SetUnethicalReading(true);

            //var pdf = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());

            //var form = PdfAcroForm.GetAcroForm(pdf, false);

            //var fields = form.GetFormFields();
            //var list = new List<string>();
            //int i = 0;
            //foreach (string field in fields.Keys)
            //{   
            //    PdfFormField formField = form.GetField(field);
            //    //fields.TryGetValue(field, out PdfFormField formField);
            //    if (formField.IsReadOnly()) continue;

            //    string[] states = formField.GetAppearanceStates();
            //    string statesStr = string.Empty;
            //    foreach (var state in states)
            //    {
            //        statesStr += $"{state} | ";
            //    }

            //    if (!string.IsNullOrEmpty(statesStr))
            //        list.Add($"[{formField.GetType().Name}] {field} - {statesStr}");

            //    if (formField is PdfTextFormField textFormField)
            //    {
            //        formField = textFormField.SetValue("test");
            //        //formField.SetValue("test");
            //    }
            //    else if (formField is PdfButtonFormField buttonFormField)
            //    {
            //        formField = buttonFormField.SetCheckType(PdfFormField.TYPE_CHECK);
            //        //formField.SetValue("btrue");
            //        if (buttonFormField.IsRadio())
            //            formField = buttonFormField.SetRadio(true);
            //        else
            //            formField = buttonFormField.SetValue("1");
            //    }
            //    //else if (formField is PdfChoiceFormField choiceFormField)
            //    //{
            //    //    //formField.SetValue("ctrue");
            //    //    //if (choiceFormField.IsCombo())
            //    //    //    choiceFormField.SetCombo(true);
            //    //    //if (choiceFormField.IsEdit())
            //    //    //    choiceFormField.SetEdit(true);
            //    //    //if (choiceFormField.IsMultiSelect())
            //    //    //    choiceFormField.SetMultiSelect(true);
            //    //    //if (choiceFormField.IsSort())
            //    //    //    choiceFormField.SetSort(true);
            //    //    if (choiceFormField.IsCombo())
            //    //        list.Add("[" + (i++) + "] Choice - Combo");
            //    //    else if (choiceFormField.IsEdit())
            //    //        list.Add("[" + (i++) + "] Choice - Edit");
            //    //    else if (choiceFormField.IsMultiSelect())
            //    //        list.Add("[" + (i++) + "] Choice - Multi");
            //    //    else if (choiceFormField.IsSort())
            //    //        list.Add("[" + (i++) + "] Choice - Sort");
            //    //    else
            //    //        list.Add("[" + (i++) + "] Choice - None");
            //    //}
            //    ////formField.SetValue("test");
            //}

            //pdf.Close();

            //return Ok(list);


            var obj = SaveMockJsonData("ESDC-EMP5624-filled.pdf", "formJsonData.json");
            LoadMockJsonData("formJsonData.json", "ESDC-EMP5624.pdf", "ESDC-EMP5624_result.pdf");
            return Ok(obj);



            //iTextSharp.text.pdf.PdfReader.unethicalreading = true;
            //var a = new iTextSharp.text.pdf.PdfReader("ESDC-EMP5624.pdf");
            ////if (a.HasUsageRights()) a.RemoveUsageRights();
            //var stream = new FileStream("ESDC-EMP5624_filled.pdf", FileMode.Create);
            //var b = new iTextSharp.text.pdf.PdfStamper(a, stream, '\0', true);
            ////b.GetOverContent(1);
            //foreach (string field in b.AcroFields.Fields.Keys)
            //{                
            //    var fieldType = b.AcroFields.GetFieldType(field);
            //    if (fieldType == iTextSharp.text.pdf.AcroFields.FIELD_TYPE_TEXT)
            //    {
            //        b.AcroFields.SetField(field, "test");
            //    }
            //    else if (fieldType == iTextSharp.text.pdf.AcroFields.FIELD_TYPE_CHECKBOX
            //        || fieldType == iTextSharp.text.pdf.AcroFields.FIELD_TYPE_RADIOBUTTON)
            //    {
            //        b.AcroFields.SetField(field, "true");
            //    }
            //}

            //b.Close();
            //b.Dispose();

            //a.Close();
            //a.Dispose();

            //var reader = new iTextSharp.text.pdf.PdfReader("ESDC-EMP5624.pdf");
            //var pdf = new PdfSharpCore.Pdf.PdfDocument("ESDC-EMP5624.pdf");
            //var formFields = pdf.AcroForm.Fields;
            //return new string[] { "value1", "value2" };
            //return Ok(new string[] { "value1", "value2" });
        }
        private dynamic SaveMockJsonData(string pdfFileName, string jsonFileName)
        {
            var reader = new PdfReader(pdfFileName);
            reader.SetUnethicalReading(true);

            var pdf = new PdfDocument(reader);

            var form = PdfAcroForm.GetAcroForm(pdf, false);

            var fields = form.GetFormFields();
            var obj = new ExpandoObject();
            foreach (string field in fields.Keys)
            {
                PdfFormField formField = form.GetField(field);
                if (formField.IsReadOnly()) continue;

                var pdfValue = formField.GetValue();
                var value = pdfValue?.ToString();
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    if (formField is PdfButtonFormField buttonFormField)
                        value = value.Replace("/", "");
                    
                    ((IDictionary<string, object>)obj).Add(field, value.ToString());
                }
                    
            }

            reader.Close();
            pdf.Close();

            SaveJsonFile(jsonFileName, obj);
            return obj;
        }
        private void SaveJsonFile(string jsonFileName, ExpandoObject obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            using (FileStream stream = new FileStream("formJsonData.json", FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(json);
                }
            }
        }

        private void LoadMockJsonData(string jsonFileName, string originalPdfFile, string destPdfFile)
        {
            dynamic obj = null;
            using (StreamReader r = new StreamReader(jsonFileName))
            {
                string json = r.ReadToEnd();
                obj = JsonConvert.DeserializeObject(json);
            }

            var reader = new PdfReader(originalPdfFile);
            var writer = new PdfWriter(destPdfFile);
            reader.SetUnethicalReading(true);

            var pdf = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            var form = PdfAcroForm.GetAcroForm(pdf, false);
            var fields = form.GetFormFields();
            foreach (string field in fields.Keys)
            {
                PdfFormField formField = form.GetField(field);
                if (formField == null || formField.IsReadOnly()) continue;

                //string[] states = formField.GetAppearanceStates();
                //string statesStr = string.Empty;
                //foreach (var state in states)
                //{
                //    statesStr += $"{state} | ";
                //}

                //if (!string.IsNullOrEmpty(statesStr))
                //    list.Add($"[{formField.GetType().Name}] {field} - {statesStr}");

                string value = null;
                try
                {
                    value = (string)obj[field];
                    //formField.SetValue((string)obj[field]);
                }
                catch { }
                if (!string.IsNullOrEmpty(value))
                {
                    if (formField is PdfButtonFormField buttonFormField)
                    {   
                        formField = buttonFormField.SetCheckType(PdfFormField.TYPE_CHECK);
                        if (buttonFormField.IsRadio())
                            formField = buttonFormField.SetRadio(true);
                    }
                    formField.SetValue(value);
                }

                //if (formField is PdfTextFormField textFormField)
                //{
                //    formField = textFormField.SetValue("test");
                //    //formField.SetValue("test");
                //}
                //else if (formField is PdfButtonFormField buttonFormField)
                //{
                //    formField = buttonFormField.SetCheckType(PdfFormField.TYPE_CHECK);
                //    //formField.SetValue("btrue");
                //    if (buttonFormField.IsRadio())
                //        formField = buttonFormField.SetRadio(true);
                //    else
                //        formField = buttonFormField.SetValue("1");
                //}
                //else if (formField is PdfChoiceFormField choiceFormField)
                //{
                //    //formField.SetValue("ctrue");
                //    //if (choiceFormField.IsCombo())
                //    //    choiceFormField.SetCombo(true);
                //    //if (choiceFormField.IsEdit())
                //    //    choiceFormField.SetEdit(true);
                //    //if (choiceFormField.IsMultiSelect())
                //    //    choiceFormField.SetMultiSelect(true);
                //    //if (choiceFormField.IsSort())
                //    //    choiceFormField.SetSort(true);
                //    if (choiceFormField.IsCombo())
                //        list.Add("[" + (i++) + "] Choice - Combo");
                //    else if (choiceFormField.IsEdit())
                //        list.Add("[" + (i++) + "] Choice - Edit");
                //    else if (choiceFormField.IsMultiSelect())
                //        list.Add("[" + (i++) + "] Choice - Multi");
                //    else if (choiceFormField.IsSort())
                //        list.Add("[" + (i++) + "] Choice - Sort");
                //    else
                //        list.Add("[" + (i++) + "] Choice - None");
                //}
                ////formField.SetValue("test");
            }

            pdf.Close();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
