using BF.POC.FaceAPI.Domain.Contracts.Clients;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace BF.POC.FaceAPI.Business.Clients
{
    public class FaceAPIClient : IFaceAPIClient
    {
        protected readonly IFaceServiceClient faceServiceClient;

        protected readonly IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[]
        {
            FaceAttributeType.Gender,
            FaceAttributeType.Age,
            FaceAttributeType.HeadPose,
            FaceAttributeType.Smile,
            FaceAttributeType.FacialHair,
            FaceAttributeType.Glasses,
            FaceAttributeType.Emotion,
            FaceAttributeType.Hair,
            FaceAttributeType.Makeup,
            FaceAttributeType.Occlusion,
            FaceAttributeType.Accessories,
            FaceAttributeType.Blur,
            FaceAttributeType.Exposure,
            FaceAttributeType.Noise
        };

        public FaceAPIClient()
        {
            // ToDo: initialize faceServiceClient object here...
        }

        #region - Group Managment -

        public async Task<bool> GroupExistsAsync(string code)
        {
            // ToDo: Create a try-catch block
            throw new NotImplementedException();

            // ToDo: In the try section, invoke faceServiceClient and get the group by code and return a boolean value if exists
            throw new NotImplementedException();

            // ToDo: In the catch section, capture FaceAPIException and if the error code is "PersonGroupNotFound", returns false.
            throw new NotImplementedException();
        }

        public async Task GroupCreateAsync(string code, string name)
        {
            // ToDo: Invoke the API method to create a new person group
            throw new NotImplementedException();
        }

        public async Task GroupUpdateAsync(string code, string name)
        {
            // ToDo: Invoke the API method to update an existing person group
            throw new NotImplementedException();
        }

        public async Task GroupTrainAsync(string code)
        {
            // ToDo: Invoke the API method to train an existing person group
            throw new NotImplementedException();
        }

        #endregion - Group Managment -

        #region - Person Management -

        public async Task<Guid> PersonCreateAsync(string groupCode, string personName)
        {
            // ToDo: Invoke the API method to create a new person in an existing person group
            throw new NotImplementedException();
        }

        public async Task PersonUpdateAsync(string groupCode, Guid personId, string personName)
        {
            // ToDo: Invoke the API method to update an existing person in an existing person group
            throw new NotImplementedException();
        }

        #endregion - Person Management -

        #region - Face Management -

        public async Task<Guid> FaceAddAsync(string groupCode, Guid personId, byte[] image)
        {
            // ToDo: Invoke the API method to add a new face to an existing person
            throw new NotImplementedException();
        }

        public async Task<Face[]> FaceDetectAsync(byte[] image)
        {
            // ToDo: Invoke the API method to detect faces in an image
            throw new NotImplementedException();
        }

        public async Task<Face[]> FaceCountFacesAsync(byte[] image)
        {
            // ToDo: Invoke the API method to count faces in an image
            throw new NotImplementedException();
        }

        public async Task<IdentifyResult[]> FaceIdentifyFacesAsync(string groupCode, Guid[] faceIDs)
        {
            // ToDo: Invoke the API method to identify a face in an image
            throw new NotImplementedException();
        }

        #endregion - Face Management -
    }
}
