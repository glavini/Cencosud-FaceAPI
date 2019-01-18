using BF.POC.FaceAPI.Domain.Contracts;
using BF.POC.FaceAPI.Domain.Contracts.Clients;
using BF.POC.FaceAPI.Domain.Entities;
using BF.POC.FaceAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BF.POC.FaceAPI.Business
{
    public class FaceManager : BaseManager, IFaceManager
    {
        private readonly IGroupRepository groupRepository;
        private readonly IPersonRepository personRepository;
        private readonly IFaceRepository faceRepository;
        private readonly IFaceAPIClient faceAPIClient;

        public FaceManager(IGroupRepository groupRepository, IPersonRepository personRepository, IFaceRepository faceRepository, IFaceAPIClient faceAPIClient)
        {
            this.groupRepository = groupRepository;
            this.personRepository = personRepository;
            this.faceRepository = faceRepository;
            this.faceAPIClient = faceAPIClient;
        }

        public IList<Face> GetAll()
        {
            return faceRepository.GetAll().ToList();
        }

        public IList<Face> GetAllByPersonId(int personId)
        {
            return faceRepository.GetAll().Where(face => face.PersonId == personId).ToList();
        }

        public IList<Face> GetAllByGroupId(int groupId)
        {
            return faceRepository.GetAll().Where(face => face.Person.GroupId == groupId).ToList();
        }

        public Face GetById(int id)
        {
            return faceRepository.GetById(id);
        }

        public async Task AddAsync(Face face)
        {
            // ToDo: Get the person details from the personRepository
            Person person = personRepository.GetById(face.PersonId);

            // ToDo: Get the group details from the groupRepository
            Group group = groupRepository.GetById(person.GroupId);

            // ToDo: Call faceAPIClient and count how many faces are in current image.
            //        - If there is no faces in the image, then throw a new BusinessException with the message "No faces found in the selected image"
            //        - If there is more than one face in the image, then throw a new BusinessException with the message "To many faces found in the selected image"
            int count = (await faceAPIClient.FaceCountFacesAsync(face.Photo)).Length;
            if (count == 0)
            {
                throw new BusinessException("No faces found in the selected image");
            }
            else if (count > 1)
            {
                throw new BusinessException("To many faces found in the selected image");
            }

            // ToDo: Call faceAPIClient to add a new face, the get FaceId and assign it to APIFaceId property...
            face.APIFaceId = await faceAPIClient.FaceAddAsync(group.Code, person.APIPersonId, face.Photo);

            // ToDo: Call faceRepository to save the new person
            await faceRepository.AddAsync(face);

            // ToDo: Call faceAPIClient to train the Network again
            await faceAPIClient.GroupTrainAsync(group.Code);
        }

    }
}
