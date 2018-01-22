using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
namespace SampleBotTemplate
{
    public class DocumentDBOperations
    {
        private const string EndpointUrl = "https://mybotdocumentdb.documents.azure.com:443/";
        private const string AuthorizationKey =
           "oAsw53gcwlMVVIKyxtoGCOMTHpXCnoow0Ny9wa4kmju81KruBZfzWasmjvkRwsui2ngEGpbPw2dIOG81zZQbTQ==";
        private static Database database;
        private static DocumentCollection collection;
        public static JObject resultJson = new JObject();
        public static string databaseName = "CustomerDatabase";
        public static string collectionName = "CustomerCollection";
        public static string CreateDocumentClient(JObject customer)
        {
            try
            {
                resultJson = customer;

                // Create a new instance of the DocumentClient
                var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
                return CreateDatabase(client);
            }
            catch (Exception e)
            {

            }
            return "";
        }
        private static string CreateDatabase(DocumentClient client)
        {
            var databaseDefinition = new Database { Id = databaseName };
            
            Task<ResourceResponse<Database>> result = client.CreateDatabaseIfNotExistsAsync(databaseDefinition);
            database = result.Result.Resource;
            CreateCollection(client, collectionName);
            return CreateDocument(client, resultJson);
        }
        private static void CreateCollection(DocumentClient client, string collectionId, string offerType = "S1")
        {
            var collectionDefinition = new DocumentCollection { Id = collectionId };
            var options = new RequestOptions { OfferType = offerType };
            Task<ResourceResponse<DocumentCollection>> result = client.CreateDocumentCollectionIfNotExistsAsync(database.SelfLink,
               collectionDefinition, options);
            collection = result.Result.Resource;
        }
        private static string CreateDocument(DocumentClient client, object documentObject)
        {
            Task<ResourceResponse<Document>> insertresult = client.CreateDocumentAsync(collection.SelfLink, documentObject);
            Document Insertdocument = insertresult.Result.Resource;
            return Insertdocument.SelfLink;
        }
        public static void UpdateDocument(string documentSelfLink,string propertyname, string propertyvalue)
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            Task<ResourceResponse<Document>> readresult = client.ReadDocumentAsync(documentSelfLink);
            Document Readdocument = readresult.Result.Resource;
            Readdocument.SetPropertyValue(propertyname, propertyvalue);
            client.ReplaceDocumentAsync(Readdocument);
        }
        public static Document ReadDocument(string documentid)
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            Task<ResourceResponse<Document>> readresult = client.ReadDocumentAsync("dbs/" + databaseName +"/colls/" + collectionName + "/docs/" + documentid);
            Document Readdocument = readresult.Result.Resource;
            return Readdocument;
        }
        public static bool ValidatePropertyValue(string documentSelfLink, string propertyname)
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            Task<ResourceResponse<Document>> readresult = client.ReadDocumentAsync(documentSelfLink);
            Document Readdocument = readresult.Result.Resource;
        
            string propValue = Readdocument.GetPropertyValue<string>(propertyname);
            Regex regex = new Regex("^(\\b[A-Za-z]*\\b\\s+\\b[A-Za-z]*\\b+\\.[A-Za-z])$",RegexOptions.IgnoreCase| RegexOptions.CultureInvariant| RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


            if (propValue == null)
            {
                return false;
            }
            if (propValue.Trim() == string.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void DeleteDocument(string documentSelfLink)
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            Task<ResourceResponse<Document>> readresult = client.DeleteDocumentAsync(documentSelfLink);
        }
        public static void ReplaceDocument(string documentSelfLink, JObject result)
        {
            resultJson = result;
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var replaceResult=client.ReplaceDocumentAsync(documentSelfLink, resultJson).Result;
            //Task replaceDoc = client.ReplaceDocumentAsync(documentSelfLink, resultJson);
            //Task.WaitAll(replaceDoc);
        }

    }
}