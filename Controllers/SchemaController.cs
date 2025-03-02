﻿using firnal.dashboard.services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace firnal.dashboard.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchemaController : ControllerBase
    {
        private readonly ISchemaService _schemaService;

        public SchemaController(ISchemaService schemaService)
        {
            _schemaService = schemaService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _schemaService.GetAll();

            return Ok(all);
        }
    }
}
