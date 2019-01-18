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
            var subscriptionKey = ConfigurationManager.AppSettings["FaceApiSubscriptionKey"];
            var endpoint = ConfigurationManager.AppSettings["FaceApiEndpoint"];

            faceServiceClient = new FaceServiceClient(subscriptionKey, endpoint);
        }

        #region - Group Managment -

        public async Task<bool> GroupExistsAsync(string code)
        {
            // ToDo: Create a try-catch block
            try
            {
                // ToDo: In the try section, invoke faceServiceClient and get the group by code and return a boolean value if exists
                return await faceServiceClient.GetPersonGroupAsync(code) != null;
            }
            catch (FaceAPIException ex)
            {
                // ToDo: In the catch section, capture FaceAPIException and if the error code is "PersonGroupNotFound", returns false.
                if (ex.ErrorCode == "PersonGroupNotFound")
                {
                    return false;
                }
                throw;
            }
        }

        public async Task GroupCreateAsync(string code, string name)
        {
            // ToDo: Invoke the API method to create a new person group
            await faceServiceClient.CreatePersonGroupAsync(code, name);
        }

        public async Task GroupUpdateAsync(string code, string name)
        {
            // ToDo: Invoke the API method to update an existing person group
            await faceServiceClient.UpdatePersonGroupAsync(code, name);
        }

        public async Task GroupTrainAsync(string code)
        {
            // ToDo: Invoke the API method to train an existing person group
            await faceServiceClient.TrainPersonGroupAsync(code);
        }

        #endregion - Group Managment -

        #region - Person Management -

        public async Task<Guid> PersonCreateAsync(string groupCode, string personName)
        {
            // ToDo: Invoke the API method to create a new person in an existing person group
            return (await faceServiceClient.CreatePersonInPersonGroupAsync(groupCode, personName)).PersonId;
        }

        public async Task PersonUpdateAsync(string groupCode, Guid personId, string personName)
        {
            // ToDo: Invoke the API method to update an existing person in an existing person group
            await faceServiceClient.UpdatePersonInPersonGroupAsync(groupCode, personId, personName);
        }

        #endregion - Person Management -

        #region - Face Management -

        public async Task<Guid> FaceAddAsync(string groupCode, Guid personId, byte[] image)
        {
            // ToDo: Invoke the API method to add a new face to an existing person
            return (await faceServiceClient.AddPersonFaceAsync(groupCode, personId, new MemoryStream(image))).PersistedFaceId;
        }

        public async Task<Face[]> FaceDetectAsync(byte[] image)
        {
            // ToDo: Invoke the API method to detect faces in an image
            return await faceServiceClient.DetectAsync(new MemoryStream(image), true, false, faceAttributes);
        }

        public async Task<Face[]> FaceCountFacesAsync(byte[] image)
        {
            // ToDo: Invoke the API method to count faces in an image
            return await faceServiceClient.DetectAsync(new MemoryStream(image), true, false, new FaceAttributeType[] { });
        }

        public async Task<IdentifyResult[]> FaceIdentifyFacesAsync(string groupCode, Guid[] faceIDs)
        {
            // ToDo: Invoke the API method to identify a face in an image
            return await faceServiceClient.IdentifyAsync(faceIDs, groupCode, null, 0.65F, 1);
        }

        #endregion - Face Management -
    }
}
