﻿
namespace Contracts.Dto.Student
{
    public class StudentMinDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ra { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Period { get; set; }
        public bool Active { get; set; }
    }
}