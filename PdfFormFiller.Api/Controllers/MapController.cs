using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PdfFormFiller.Api.Options;
using PdfFormFiller.Core.Exceptions;
using PdfFormFiller.Core.Interfaces;
using PdfFormFiller.Core.Models;

namespace PdfFormFiller.Api.Controllers
{
    [Route("pdfform/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class MapController : Controller
    {
        private readonly IPdfFormMapRepository _pdfFormRepository;

        public MapController(IPdfFormMapRepository pdfFormRepository)
        {
            _pdfFormRepository = pdfFormRepository;
        }

        [HttpGet("{pdfCode}")]
        public ActionResult<PdfFormMap> Get(string pdfCode)
        {
            try
            {
                var pdfFormMap = _pdfFormRepository.Get(pdfCode);
                return Ok(pdfFormMap);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(pdfCode);
            }
        }

        [HttpPost]
        public ActionResult<PdfFormMap> Post([FromBody] PdfFormMap map)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var pdfFormMap = _pdfFormRepository.Add(map);
                return Ok(pdfFormMap);
            } 
            catch (EntityAlreadyExistsException)
            {
                return Conflict();
            }
        }

        [HttpPut("{pdfCode}")]
        public ActionResult Put(string pdfCode, [FromBody] PdfFormMap updatedMap)
        {
            if (string.IsNullOrEmpty(pdfCode))
            {
                return NotFound(pdfCode);
            }
            if (updatedMap.Id != pdfCode)
            {
                return BadRequest(updatedMap.Id);
            }

            try
            {
                _pdfFormRepository.Update(updatedMap);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(pdfCode);
            }

        }

        [HttpDelete("{pdfCode}")]
        public ActionResult Delete(string pdfCode)
        {
            try
            {                
                var pdfFormMapToDelete = _pdfFormRepository.Get(pdfCode);
                if (pdfFormMapToDelete == null)
                {
                    return NotFound(pdfCode);
                }

                _pdfFormRepository.Delete(pdfFormMapToDelete);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(pdfCode);
            }
        }
    }
}