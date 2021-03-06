﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cnf.Finance.Api.Models;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformTermsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public PerformTermsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/PerformTerms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerformTerms>>> GetPerformTerms(int? performId)
        {
            var query = from t in _context.PerformTerms
                        where performId == null || performId.Value == t.PerformId
                        select t;

            return await query.ToListAsync();
        }

        // GET: api/PerformTerms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PerformTerms>> GetPerformTerms(int id)
        {
            var performTerms = await _context.PerformTerms.FindAsync(id);

            if (performTerms == null)
            {
                return NotFound();
            }

            return performTerms;
        }

        // GET: api/PerformTerms/GetTasks?orgId=1&year=2020&month=2
        // 注意： 返回一个PerformTerms类型的数组，其中，每个元素的Perform, Terms, Terms.Project是有效的对象。
        [HttpGet("GetTasks")]
        public async Task<ActionResult<IEnumerable<PerformTerms>>> GetPerformTerms(int orgId, int year, int month)
        {
            var performTermsQuery = from t in _context.PerformTerms
                                                .Include(t => t.Perform)
                                                .Include(t => t.Terms)
                                                    .ThenInclude(t => t.Project)
                                 where t.Terms.Project.OrganizationId == orgId
                                     && t.Perform.Year == year
                                     && t.Perform.Month == month
                                 select t;

            var performTerms = await performTermsQuery.ToListAsync();

            foreach (var t in performTerms)
            {
                t.Perform.PerformTerms = null;
                t.Perform.Project = null;
                t.Terms.PerformTerms = null;
                t.Terms.PlanTerms = null;
                t.Terms.Project.Plan = null;
                t.Terms.Project.AnnualBalance = null;
                t.Terms.Project.Organization = null;
                t.Terms.Project.Perform = null;
                t.Terms.Project.Terms = null;
            }

            return performTerms;
        }


        // PUT: api/PerformTerms/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerformTerms(int id, PerformTerms performTerms)
        {
            if (id != performTerms.Id)
            {
                return BadRequest();
            }

            _context.Entry(performTerms).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformTermsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PerformTerms
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PerformTerms>> PostPerformTerms(PerformTerms performTerms)
        {
            _context.PerformTerms.Add(performTerms);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerformTerms", new { id = performTerms.Id }, performTerms);
        }

        // DELETE: api/PerformTerms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PerformTerms>> DeletePerformTerms(int id)
        {
            var performTerms = await _context.PerformTerms.FindAsync(id);
            if (performTerms == null)
            {
                return NotFound();
            }

            _context.PerformTerms.Remove(performTerms);
            await _context.SaveChangesAsync();

            return performTerms;
        }

        private bool PerformTermsExists(int id)
        {
            return _context.PerformTerms.Any(e => e.Id == id);
        }
    }
}
