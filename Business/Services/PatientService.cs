﻿using System;
using Contracts.Interfaces.Services;
using Contracts.RequestHandle;
using System.Threading.Tasks;
using Contracts.Entities;
using Contracts.Dto.Patient;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Contracts.Interfaces.Repositories;
using Contracts.Utils;
using Contracts.TransactionObjects.User;

namespace Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly IMapper _Mapper;
        private readonly IConfiguration _configuration;
        private readonly IPatientRepository _patientRepository;

        public PatientService(IMapper Mapper, IConfiguration configuration, IPatientRepository patientRepository)
        {
            _Mapper = Mapper;
            _configuration = configuration;
            _patientRepository = patientRepository;
        }

        public async Task<RequestResult<PatientMinDto>> CreatePatient(PatientDto patientDto)
        {
            try
            {
                var patientExists = await _patientRepository.CheckIfPatientExistsByEmail(patientDto.Email);
                if (patientExists)
                    return new RequestResult<PatientMinDto>(null, true, RequestAnswer.UserDuplicateCreateError.GetDescription());
                var model = _Mapper.Map<Patient>(patientDto);
                model.Active = true;
                var response = await _patientRepository.CreatePatient(model);
                if (response.Id == 0)
                    return new RequestResult<PatientMinDto>(null, true, RequestAnswer.UserCreateError.GetDescription());
                var dto = _Mapper.Map<PatientMinDto>(response);
                return new RequestResult<PatientMinDto>(dto);
            }
            catch (Exception ex)
            {
                return new RequestResult<PatientMinDto>(null, true, ex.Message);
            }
        }

        public async Task<RequestResult<PatientDto>> GetPatientById(int id)
        {
            try
            {
                var model = await _patientRepository.GetPatientById(id);

                if (model == null)
                    return new RequestResult<PatientDto>(null, true, RequestAnswer.PatientNotFound.GetDescription());

                var dto = _Mapper.Map<PatientDto>(model);
                var result = new RequestResult<PatientDto>(dto);

                return result;
            }
            catch (Exception ex)
            {
                return new RequestResult<PatientDto>(null, true, ex.Message);
            }
        }

        public async Task<FilterInfoDto[]> GetAllPatients()
        {
            Patient[] patients  = await _patientRepository.GetAllPatients();

            var array = _Mapper.Map<FilterInfoDto[]>(patients);

            return array;
        }

        public async Task<RequestResult<RequestAnswer>> UpdatePatient(PatientDto patientDto)
        {
            try
            {
                var patientCheck = await _patientRepository.CheckIfPatientExistsByEmail(patientDto.Email);

                if(!patientCheck)
                    return new RequestResult<RequestAnswer>(RequestAnswer.PatientNotFound);
                
                var model = _Mapper.Map<Patient>(patientDto);
                await _patientRepository.UpdatePatient(model);

                return new RequestResult<RequestAnswer>(RequestAnswer.PatientUpdateSuccess);
            }
            catch (Exception)
            {
                return new RequestResult<RequestAnswer>(RequestAnswer.PatientUpdateError, true);
            }
        }

        public async Task<RequestResult<RequestAnswer>> DeletePatient(int id)
        {
            try
            {
                await _patientRepository.DeletePatient(id);

                return new RequestResult<RequestAnswer>(RequestAnswer.PatientDeleteSuccess);
            }
            catch (Exception)
            {
                return new RequestResult<RequestAnswer>(RequestAnswer.PatientDeleteError, true);
            }
        }

    }
}
