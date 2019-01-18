using BF.POC.FaceAPI.Domain.Contracts;
using BF.POC.FaceAPI.Domain.Contracts.Clients;
using BF.POC.FaceAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.POC.FaceAPI.Business
{
    public class PersonManager : BaseManager, IPersonManager
    {
        private readonly IGroupRepository groupRepository;
        private readonly IPersonRepository personRepository;
        private readonly IFaceAPIClient faceAPIClient;

        public PersonManager(IGroupRepository groupRepository, IPersonRepository personRepository, IFaceAPIClient faceAPIClient)
        {
            this.groupRepository = groupRepository;
            this.personRepository = personRepository;
            this.faceAPIClient = faceAPIClient;
        }

        public IList<Person> GetAll()
        {
            return personRepository.GetAll().ToList();
        }

        public IList<Person> GetAllByGroupId(int groupId)
        {
            return personRepository.GetAll().Where(person => person.GroupId == groupId).ToList();
        }

        public Person GetById(int id)
        {
            return personRepository.GetById(id);
        }

        public async Task AddAsync(Person person)
        {
            // ToDo: Get the group details from the groupRepository
            var group = groupRepository.GetById(person.GroupId);

            // ToDo: Call faceAPIClient to create a new person. Do not forget to save APIPersonId
            person.APIPersonId = await faceAPIClient.PersonCreateAsync(group.Code, person.Fullname);

            // ToDo: Call personRepository to save the new person
            await personRepository.AddAsync(person);
        }

        public async Task UpdateAsync(Person person)
        {
            // ToDo: Get the group details from the groupRepository
            var group = groupRepository.GetById(person.GroupId);

            // ToDo: Call faceAPIClient to update a person
            await faceAPIClient.PersonUpdateAsync(group.Code, person.APIPersonId, person.Fullname);

            // ToDo: Call personRepository to save the person
            await personRepository.UpdateAsync(person);
        }

    }
}
