using BF.POC.FaceAPI.Domain.Contracts;
using BF.POC.FaceAPI.Domain.Contracts.Clients;
using BF.POC.FaceAPI.Domain.DTOs;
using BF.POC.FaceAPI.Domain.Entities;
using BF.POC.FaceAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.POC.FaceAPI.Business
{
    public class GroupManager : BaseManager, IGroupManager
    {
        private readonly IGroupRepository groupRepository;
        private readonly IPersonRepository personRepository;
        private readonly IFaceAPIClient faceAPIClient;

        public GroupManager(IGroupRepository groupRepository, IPersonRepository personManager, IFaceAPIClient faceAPIClient)
        {
            this.groupRepository = groupRepository;
            this.personRepository = personManager;
            this.faceAPIClient = faceAPIClient;
        }

        public IList<Group> GetAll()
        {
            return groupRepository.GetAll().ToList();
        }

        public Group GetById(int id)
        {
            return groupRepository.GetById(id);
        }

        public async Task AddAsync(Group group)
        {
            // ToDo: Check if the group exists using GroupExistsAsync from faceAPIClient
            throw new NotImplementedException();

            // ToDo: If the group do not exists, call faceAPIClient to create a group
            throw new NotImplementedException();

            // ToDo: If the group do not exists, call groupRepository to save the group details
            throw new NotImplementedException();

            // ToDo: If the group already exists, throw a new BusinessException with the message "Person group '[CODE]' already exists."
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Group group)
        {
            // ToDo: Call faceAPIClient to update a group
            throw new NotImplementedException();

            // ToDo: Call groupRepository to save the group details
            throw new NotImplementedException();
        }

        public async Task<List<Candidate>> SearchCandidatesAsync(int id, byte[] image)
        {
            // ToDo: Get the group details from the groupRepository
            throw new NotImplementedException();

            // ToDo: Call FaceDetectAsync in faceAPIClient and assing its result into a new 'faces' var from type 'Microsoft.ProjectOxford.Face.Contract.Face[]'
            throw new NotImplementedException();

            // ToDo: If no faces were detected, then return an empty list
            throw new NotImplementedException();

            // ToDo: Create a list of candidates from 'faces' list using our 'Candidate' entity
            throw new NotImplementedException();

            // ToDo: Call FaceIdentifyFacesAsync in faceAPIClient and assing its result into a new 'identifyResult' var from type 'Microsoft.ProjectOxford.Face.Contract.IdentifyResult[]'
            throw new NotImplementedException();

            // ToDo: For each item in 'identifyResult':
            //        - Get the first candidate item from it (result.Candidates[0]) and search in our DB for a Person that matching with the same 'PersonId'
            //        - If there is a Person registered in our DB, associate it with the candidate in 'candidates' list and set Confidence to it too
            throw new NotImplementedException();
        }

    }
}
