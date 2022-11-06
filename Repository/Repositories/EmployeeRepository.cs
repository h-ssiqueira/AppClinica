﻿using Contracts.Entities;
using Contracts.Enums.Auth;
using Contracts.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories {
    public class EmployeeRepository : IEmployeeRepository {

        private readonly TemplateDbContext _context;

        public EmployeeRepository(TemplateDbContext context) {
            _context = context;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _context.Employees.Where(u => u.Id == id && u.Active).FirstOrDefaultAsync();
        }

        public async Task<Employee> Register(Employee employee) {
            var result = await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public Task<Employee> GetEmployeeByEmailAndPassword(string email, string password)
        {
            throw new NotImplementedException();
        }
        
        public Task<Employee> GetEmployeeByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckIfEmployeeExistsById(int id) {
            var result = await _context.Employees.AnyAsync(u => u.Id == id && u.Active);
            return result;
        }

        public async Task<bool> CheckIfEmployeeExistsByEmail(string email) {
            var result = await _context.Employees.AnyAsync(u => u.Email == email && u.Active);
            return result;
        }

        public async Task UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await _context.Employees.Where(e => e.Id == id).FirstOrDefaultAsync();
            employee.Active = false;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
    }
}
