﻿using System;
using Contracts.Interfaces.Services;
using Contracts.RequestHandle;
using System.Threading.Tasks;
using Contracts.Entities;
using Contracts.Interfaces.Repositories;
using Contracts.Dto.Professor;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Contracts.Utils;

namespace Business.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly IMapper _Mapper;
        private readonly IConfiguration _configuration;
        private readonly IProfessorRepository _professorRepository;

        public ProfessorService(IMapper Mapper, IConfiguration configuration, IProfessorRepository professorRepository)
        {
            _Mapper = Mapper;
            _configuration = configuration;
            _professorRepository = professorRepository;
        }

        public async Task<RequestResult<ProfessorDto>> CreateProfessor(ProfessorDto professorDto)
        {
            try
            {
                var patientExists = await _professorRepository.CheckIfProfessorExistsByEmail(professorDto.Email);
                if (patientExists)
                    return new RequestResult<ProfessorDto>(null, true, RequestAnswer.UserDuplicateCreateError.GetDescription());
                var model = _Mapper.Map<Professor>(professorDto);
                model.active = true;
                var response = await _professorRepository.CreateProfessor(model);
                if (response.id == 0)
                    return new RequestResult<ProfessorDto>(null, true, RequestAnswer.UserCreateError.GetDescription());
                var dto = _Mapper.Map<ProfessorDto>(response);
                return new RequestResult<ProfessorDto>(dto);
            }
            catch (Exception ex)
            {
                return new RequestResult<ProfessorDto>(null, true, ex.Message);
            }
        }

        public async Task<RequestResult<ProfessorDto>> GetProfessorById(int id)
        {
            try
            {
                var model = await _professorRepository.GetProfessorById(id);

                if (model == null)
                    return new RequestResult<ProfessorDto>(null, true, RequestAnswer.ProfessorNotFound.GetDescription());

                var dto = _Mapper.Map<ProfessorDto>(model);
                var result = new RequestResult<ProfessorDto>(dto);

                return result;
            }
            catch (Exception ex)
            {
                return new RequestResult<ProfessorDto>(null, true, ex.Message);
            }
        }

        public async Task<RequestResult<RequestAnswer>> UpdateProfessor(ProfessorDto professorDto)
        {
            try
            {
                var professorCheck = await _professorRepository.CheckIfProfessorExistsById(professorDto.Id);

                if (!professorCheck)
                    return new RequestResult<RequestAnswer>(RequestAnswer.ProfessorNotFound);

                var model = _Mapper.Map<Professor>(professorDto);
                await _professorRepository.UpdateProfessor(model);

                return new RequestResult<RequestAnswer>(RequestAnswer.ProfessorUpdateSuccess);
            }
            catch (Exception)
            {
                return new RequestResult<RequestAnswer>(RequestAnswer.ProfessorUpdateError, true);
            }
        }
        
        public async Task<RequestResult<RequestAnswer>> DeleteProfessor(int id)
        {
            try
            {
                await _professorRepository.DeleteProfessor(id);

                return new RequestResult<RequestAnswer>(RequestAnswer.ProfessorDeleteSuccess);
            }
            catch (Exception)
            {
                return new RequestResult<RequestAnswer>(RequestAnswer.ProfessorDeleteError, true);
            }
        }
    }
}
