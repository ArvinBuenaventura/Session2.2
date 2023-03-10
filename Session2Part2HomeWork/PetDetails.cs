using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Session2Part2HomeWork;
using RestSharp;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]

namespace Session2Part2HomeWork
{
    [TestClass]
    public class PetDetails
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostPetMethod()
        {

            #region create data

            // Create Json Object
            PetModel newPet = new PetModel()
            {
                Id = 987,
                Name = "Milkty",
                PhotoUrls = new string[] { "catUrl" },
                Category = new Category { Id = 987, Name = "Cat" },
                Tags = new Category[] { new Category { Id = 987, Name = "White" } },
                Status = "available"
            };

            // Send Post Request
            var temp = GetURI(PetEndpoint);
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);

            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");
            #endregion

            #region GetUser
            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{newPet.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(newPet.Name, restResponse.Data.Name, "Name did not match.");
            Assert.AreEqual(newPet.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "Photo Urls did not match.");
            Assert.AreEqual(newPet.Category.Id, restResponse.Data.Category.Id, "Category ID did not match.");
            Assert.AreEqual(newPet.Category.Name, restResponse.Data.Category.Name, "Category Name did not match.");
            Assert.AreEqual(newPet.Tags[0].Id, restResponse.Data.Tags[0].Id, "Tags ID did not match.");
            Assert.AreEqual(newPet.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tags Name did not match.");
            Assert.AreEqual(newPet.Status, restResponse.Data.Status, "Status did not match.");
            #endregion
 

            #region CleanUp
            cleanUpList.Add(newPet);
            #endregion
        }

    }
}