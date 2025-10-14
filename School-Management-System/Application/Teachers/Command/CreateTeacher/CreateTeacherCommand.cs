using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain;
using MediatR;

namespace Application.Teachers.Command.CreateTeacher
{
    public class CreateTeacherCommand:IRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
    }
    public class CreateTeacherCommandhandler : IRequestHandler<CreateTeacherCommand>
    {
        private readonly IApplicationDbContext _context;
        public CreateTeacherCommandhandler(IApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = new Teacher
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                ContactNumber = request.ContactNumber,
                Gender = request.Gender,
                Age = request.Age,
                Email = request.Email,
                FatherName = request.FatherName,
                MotherName = request.MotherName,
            };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
