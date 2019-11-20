using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PdfFormFiller.Api.Options;
using PdfFormFiller.Core.Enums;
using PdfFormFiller.Core.Exceptions;
using PdfFormFiller.Core.Interfaces;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Api.Controllers
{
    [Route("pdfform/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class FillController : Controller
    {
        private readonly PdfFilesOptions _pdfFilesOptions;
        private readonly IPdfFormMapRepository _pdfFormRepository;
        private readonly IDynamicRepository _dynamicRepository;

        public FillController(IConfiguration configuration,
            IPdfFormMapRepository pdfFormRepository,
            IDynamicRepository dynamicRepository)
        {
            _pdfFilesOptions = configuration.GetSection("PdfFiles").Get<PdfFilesOptions>();
            _pdfFormRepository = pdfFormRepository;
            _dynamicRepository = dynamicRepository;
        }

        [HttpGet("fill/{pdfCode}")]
        public ActionResult Get(string pdfCode, [FromQuery(Name = "ids")] List<string> formMapCollectionsIds)
        {
            PdfFormMap pdfFormMap;
            try
            {
                pdfFormMap = _pdfFormRepository.Get(pdfCode);
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"pdfCode={pdfCode}");
            }

            if (pdfFormMap == null)
            {
                return NotFound($"pdfCode={pdfCode}");
            }
            if (pdfFormMap.Collections?.Count != formMapCollectionsIds?.Count)
            {
                return BadRequest("ids not of expected size");
            }

            var collectionsValues = new Dictionary<string, dynamic>();
            if (pdfFormMap.Collections != null)
            {
                for (int i = 0; i < pdfFormMap.Collections.Count; i++)
                {
                    _dynamicRepository.SetContext(pdfFormMap.Collections[i]);
                    try
                    {
                        var value = _dynamicRepository.Get(formMapCollectionsIds[i]);
                        collectionsValues.Add(pdfFormMap.Collections[i], value);
                    }
                    catch (EntityNotFoundException)
                    {
                        return NotFound($"ids[{i}]={formMapCollectionsIds[i]}");
                    }
                }
            }

            var templateFilePath = $"{pdfFormMap.PdfCode}.pdf";
            if (!string.IsNullOrEmpty(_pdfFilesOptions.TemplatePath))
            {
                templateFilePath = $"{_pdfFilesOptions.TemplatePath}{Path.DirectorySeparatorChar}{templateFilePath}";
            }

            var generatedFileName = $"{pdfFormMap.PdfCode}-filled-{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}.pdf";
            var generatedFilePath = generatedFileName;
            if (!string.IsNullOrEmpty(_pdfFilesOptions.GeneratedPath))
            {
                generatedFilePath = $"{_pdfFilesOptions.GeneratedPath}{Path.DirectorySeparatorChar}{generatedFilePath}";
            }

            FillPdfForm(templateFilePath, generatedFilePath, pdfFormMap, collectionsValues);
            
            //var stream = new FileStream(@"path\to\file", FileMode.Open);
            //return File(stream, "application/pdf", "FileDownloadName.ext");
            var stream = new FileStream(generatedFilePath, FileMode.Open);
            return File(stream, "application/pdf", generatedFileName);
        }

        [HttpGet("check/{pdfCode}")]
        public ActionResult Get(string pdfCode, [FromQuery(Name = "page")] int pageNumber)
        {
            var templateFilePath = $"{pdfCode}-filled.pdf";
            if (!string.IsNullOrEmpty(_pdfFilesOptions.TemplatePath))
            {
                templateFilePath = $"{_pdfFilesOptions.TemplatePath}{Path.DirectorySeparatorChar}{templateFilePath}";
            }

            using PdfReader reader = new PdfReader(templateFilePath);
            reader.SetUnethicalReading(true);
            using PdfDocument pdf = new PdfDocument(reader);

            var form = PdfAcroForm.GetAcroForm(pdf, false);

            var fields = form.GetFormFields();
            var obj = new ExpandoObject();
            foreach (string field in fields.Keys)
            {
                PdfFormField formField = form.GetField(field);
                if (formField.IsReadOnly()) continue;

                var pdfValue = formField.GetValue();
                if (formField.GetWidgets() == null) continue;
                if (formField.GetWidgets().Count <= 0) continue;

                var page = formField.GetWidgets().First().GetPage();

                if (pdf.GetPageNumber(page) != pageNumber) continue;

                bool isButton = (formField is PdfButtonFormField);
                bool isText = (formField is PdfTextFormField);

                if (!isText && !isButton) continue;

                dynamic itemObj = new ExpandoObject();

                var value = pdfValue?.ToString();
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    if (isButton)
                    {
                        value = value.Replace("/", "");
                        itemObj.IsRadio = ((PdfButtonFormField)formField).IsRadio();
                        itemObj.States = formField.GetAppearanceStates();
                    }
                }

                itemObj.Type = formField.GetType().Name;
                itemObj.WidgetsCount = formField.GetWidgets().Count;
                itemObj.Value = value;
                ((IDictionary<string, object>)obj).Add(field, itemObj);
            }

            return Ok(obj);
        }

        private void FillPdfForm(string originalPdfFile, string destPdfFile, PdfFormMap formMap, Dictionary<string, dynamic> dbValues)
        {
            using PdfReader reader = new PdfReader(originalPdfFile);
            using PdfWriter writer = new PdfWriter(destPdfFile);
            reader.SetUnethicalReading(true);

            using PdfDocument pdf = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            var form = PdfAcroForm.GetAcroForm(pdf, false);

            foreach (var fieldMap in formMap.FieldMaps)
            {
                PdfFormField formField = form.GetField(fieldMap.Name);
                if (formField == null || formField.IsReadOnly()) continue;

                //string[] states = formField.GetAppearanceStates();
                //string statesStr = string.Empty;
                //foreach (var state in states)
                //{
                //    statesStr += $"{state} | ";
                //}

                //if (!string.IsNullOrEmpty(statesStr))
                //    list.Add($"[{formField.GetType().Name}] {field} - {statesStr}");

                if (!MatchCondition(fieldMap.Condition, dbValues))
                    continue;

                string value = (string)ResolveDynamicValue(fieldMap.Value, dbValues);

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
            }
        }

        private dynamic GetPropertyValue(dynamic obj, string key)
        {
            try
            {
                return obj[key];
            }
            catch { }

            return null;
        }

        private bool MatchCondition(PdfMapCondition condition, Dictionary<string, dynamic> dbValues)
        {
            if (condition == null) return true;

            var left = ResolveDynamicValue(condition.Left, dbValues);
            var right = ResolveDynamicValue(condition.Right, dbValues);

            switch (condition.Type)
            {
                case PdfMapConditionType.Equal:
                    return left == right;
                case PdfMapConditionType.NotEqual:
                    return left != right;
                case PdfMapConditionType.GreaterThan:
                    return left > right;
                case PdfMapConditionType.LessThan:
                    return left < right;
                case PdfMapConditionType.GreaterThanOrEqual:
                    return left >= right;
                case PdfMapConditionType.LessThanOrEqual:
                    return left <= right;
                case PdfMapConditionType.HasValue:
                    string leftJson = JsonConvert.SerializeObject(left);
                    var leftList = JsonConvert.DeserializeObject<IList<dynamic>>(leftJson);
                    return leftList.Any(v => v == right);
            }

            return false;
        }

        private dynamic ResolveDynamicValue(PdfMapDynamicValue dynamicValue, Dictionary<string, dynamic> dbValues)
        {
            switch (dynamicValue.Type)
            {
                case PdfMapDynamicValueType.Fixed:
                    return Convert.ToString(dynamicValue.Value);
                case PdfMapDynamicValueType.Database:
                    string databaseJson = JsonConvert.SerializeObject(dynamicValue.Value);
                    var databaseValue = JsonConvert.DeserializeObject<PdfMapDatabaseValue>(databaseJson);

                    if (!dbValues.ContainsKey(databaseValue.Collection))
                        return null;

                    dynamic obj = dbValues[databaseValue.Collection];
                    string[] nestedFields = databaseValue.DocumentField.Split(".");

                    foreach (string field in nestedFields)
                    {
                        obj = GetPropertyValue(obj, field);
                    }

                    return obj;
                case PdfMapDynamicValueType.Arithmetic:
                    string arithmeticJson = JsonConvert.SerializeObject(dynamicValue.Value);
                    var arithmeticValue = JsonConvert.DeserializeObject<PdfMapArithmetic>(arithmeticJson);
                    return ResolveArithmetic(arithmeticValue, dbValues);
            }

            return null;
        }

        private dynamic ResolveArithmetic(PdfMapArithmetic arithmetic, Dictionary<string, dynamic> dbValues)
        {
            var left = ResolveDynamicValue(arithmetic.Left, dbValues);
            var right = ResolveDynamicValue(arithmetic.Right, dbValues);

            switch (arithmetic.Type)
            {
                case PdfMapArithmeticType.Add:
                    return left + right;
                case PdfMapArithmeticType.Subtract:
                    return left - right;
                case PdfMapArithmeticType.Multiply:
                    return left * right;
                case PdfMapArithmeticType.Divide:
                    return left / right;
                case PdfMapArithmeticType.Modulus:
                    return left % right;
            }

            return null;
        }
    }
}