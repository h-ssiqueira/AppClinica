﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Repository.Context;
using Contracts.Entities;
using Contracts.Interfaces.Repositories;
using System;

namespace Repository.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly TemplateDbContext _context;
        public ScheduleRepository(TemplateDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule> GetScheduleById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Schedule> CreateSchedule(Schedule schedule)
        {
            var result = await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task UpdateSchedule(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.Where(u => u.id == id).FirstOrDefaultAsync();
            schedule.active = false;

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }
}
