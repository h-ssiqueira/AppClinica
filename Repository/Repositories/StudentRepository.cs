﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Repository.Context;
using Contracts.Entities;
using Contracts.Interfaces.Repositories;

namespace Repository.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly TemplateDbContext _context;

        public StudentRepository(TemplateDbContext context)
        {
            _context = context;
        }

        public async Task<Student> CreateStudent(Student student)
        {
            var result = await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return result.Entity;
        }
        
        public async Task UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudent(int id)
        {
            var student = await _context.Students.Where(u => u.id == id).FirstOrDefaultAsync();
            student.active = false;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task<Student> GetStudentById(int id)
        {
            return await _context.Students.Where(u => u.id == id && u.active).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfStudentExistsById(int id)
        {
            var result = await _context.Students.AnyAsync(u => u.id == id && u.active);
            return result;
        }

        public async Task<bool> CheckIfStudentExistsByEmail(string email)
        {
            var result = await _context.Students.AnyAsync(u => u.email == email && u.active);
            return result;
        }

        public async Task<bool> CheckIfStudentExistsByRa(string ra)
        {
            var result = await _context.Students.AnyAsync(u => u.ra == ra && u.active);
            return result;
        }

        public async Task<string> GetStudentPasswordByEmail(string email)
        {
            var result = await _context.Students.Where(u => u.email == email && u.active).Select(p => p.password).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Student> GetStudentByEmailAndPassword(string email, string password)
        {
            return await _context.Students.Where(x => x.email == email && x.password == password && x.active).FirstOrDefaultAsync();
        }

        public async Task<Student> GetStudentByEmail(string email)
        {
            var result = await _context.Students.Where(u => u.email == email && u.active).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Student> GetStudentByRa(string ra)
        {
            var result = await _context.Students.Where(u => u.ra == ra && u.active).FirstOrDefaultAsync();
            return result;
        }
    }
}
