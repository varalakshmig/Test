using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SampleBotTemplate.DataAccess
{
    public class CosmosDBConnectionManager
    {
        private static DocumentClient client;

        //Assign a name for your database & collection 
        private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private static readonly string collectionId = ConfigurationManager.AppSettings["CollectionId"];
        //Read the DocumentDB endpointUrl and authorisationKeys from config
        //These values are available from the Azure Management Portal on the DocumentDB Account Blade under "Keys"
        //NB > Keep these values in a safe & secure location. Together they provide Administrative access to your DocDB account
        private static readonly string endpointUrl = ConfigurationManager.AppSettings["Endpoint"];
        private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthKey"];

        public static void InitializeCosmosClient()
        {
            client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
        }
        private static async Task LoadDataFromSQLIntoCosmos(string colSelfLink)
        {
            foreach (CosmosDocument cmp in ConnectionManager.GetCosmosData())
            {
                //Use this query to search acrynom in db
                dynamic filter = client.CreateDocumentQuery<Document>(colSelfLink).Where(d => d.Id == cmp.id).AsEnumerable().FirstOrDefault();
                if (filter != null)
                {
                    cmp.homogens.Add(new Homogen
                    {
                        keyword = cmp.keyword,
                        approvedind = cmp.approvedind,
                        category = cmp.category,
                        definition = cmp.definition
                    ,
                        userid = cmp.userid
                    });
                    CosmosDocument update = cmp;
                    //Use this query to update existing document in db
                    Document updatedDoc = await client.ReplaceDocumentAsync(filter.SelfLink, update);
                }
                else
                {
                    //Use this query to add a new keyword
                    Document created = await client.CreateDocumentAsync(colSelfLink, cmp);
                }
            }
        }

        /// <summary>
        /// Get a DocumentCollection by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="dbLink">The Database SelfLink property where this DocumentCollection exists / will be created</param>
        /// <param name="id">The id of the DocumentCollection to search for, or create.</param>
        /// <returns>The matched, or created, DocumentCollection object</returns>
        private static async Task<DocumentCollection> GetOrCreateCollectionAsync(string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }
            return collection;
        }

        /// <summary>
        /// Get a Database by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="id">The id of the Database to search for, or create.</param>
        /// <returns>The matched, or created, Database object</returns>
        private static async Task<Database> GetOrCreateDatabaseAsync(string id)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = id });
            }
            return database;
        }
        /// <summary>
        /// This function is used to either add a new keyword document to cosmos DB or appending homogen to an already existing keyword document
        /// </summary>
        /// <param name="keyword">user input keyword</param>
        /// <param name="definition">user input definition</param>
        /// <param name="empid">user input employee id</param>
        /// <returns></returns>
        public static async Task<bool> AddNewKeyword(string keyword, string definition, string empid)
        {
            try
            {
                CosmosDocument comp = new CosmosDocument();
                comp.id = comp.keyword = keyword;
                comp.definition = definition;
                comp.userid = empid;
                comp.isbidirectional = "No";
                comp.homogens = new List<Homogen>();
                comp.approvedind = "await";
                comp.category = "Delta Keyword";

                //Get Client Initialized
                InitializeCosmosClient();

                //Get, or Create, the Database
                var database = await GetOrCreateDatabaseAsync(databaseId);

                //Get, or Create, the Document Collection
                var collection = await GetOrCreateCollectionAsync(database.SelfLink, collectionId);

                //check if the document exist in cosmos DB
                Document filter = client.CreateDocumentQuery<Document>(collection.SelfLink).Where(d => d.Id.ToLower() == comp.id.ToLower()).AsEnumerable().FirstOrDefault();
                JObject outputJson = new JObject();
                List<Homogen> Allchilds = new List<Homogen>();
                //Document doesn't Exist
                if (filter == null)
                {
                    //Add the document to cosmos DB
                    ResourceResponse<Document> createdDoc = client.CreateDocumentAsync(collection.SelfLink, comp).Result;
                    if (createdDoc != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //Document already exist
                else
                {
                    outputJson = JObject.Parse(filter.ToString());
                    List<Homogen> homolist = outputJson["homogens"].ToObject<List<Homogen>>();
                    //Loop through the already existing Homogens available with the keyword and get into a container
                    if (homolist.Count != 0)
                    {
                        foreach (Homogen chld in homolist)
                        {
                            if (Allchilds.Contains(chld)) { }
                            else Allchilds.Add(chld);
                        }
                    }
                    //Add the newly requested keyword to the container
                    Allchilds.Add(new Homogen { approvedind = comp.approvedind, category = comp.category, definition = comp.definition, keyword = comp.keyword, userid = comp.userid });

                    //Update the document's homogens with the whole container data
                    outputJson["homogens"] = JArray.FromObject(Allchilds);
                    //Replace the document in the cosmos DB
                    ResourceResponse<Document> updatedDoc = client.ReplaceDocumentAsync(filter.SelfLink, outputJson).Result;
                    if (updatedDoc != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            { }
            return false;
        }
        /// <summary>
        /// This function is used to search for acronym's in cosmos DB. Initially it does a normal search along with homogens. If no such document found, then
        /// it does a bi-directional query search and displays the result.
        /// </summary>
        /// <param name="searchKeyword">user's search keyword</param>
        /// <returns>one or more definitions / one keyword in case of bi-directional result</returns>
        public static async Task<List<string>> SearchAcronym(string searchKeyword)
        {
            List<string> res = new List<string>();
            try
            {
                //Get Client Initialized
                InitializeCosmosClient();

                //Get, or Create, the Database
                var database = await GetOrCreateDatabaseAsync(databaseId);

                //Get, or Create, the Document Collection
                var collection = await GetOrCreateCollectionAsync(database.SelfLink, collectionId);

                Document filter = client.CreateDocumentQuery<Document>(collection.SelfLink).Where(d => d.Id.ToLower() == searchKeyword.ToLower()).AsEnumerable().FirstOrDefault();

                //if document exist in DB with the same keyword searched by user
                if (filter != null)
                {
                    //Read the document keyword's definition
                    JObject outputJSON = JObject.Parse(filter.ToString());
                    res.Add(outputJSON["definition"].ToString());

                    //Loop through the Homogens to display a list of definitions
                    JObject outputHomoJSON = JObject.Parse(filter.ToString());
                    
                    List<Homogen> homolist = outputJSON["homogens"].ToObject<List<Homogen>>();
                    if (homolist.Count != 0)
                    {
                        foreach (Homogen chld in homolist)
                        {
                            if (res.Contains(chld.definition)) { }
                            else res.Add(chld.definition);
                        }
                    }
                }
                //When no such document found in DB, do a bi-directional search
                else
                {
                    FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
                    IQueryable<CosmosDocument> compQuery = client.CreateDocumentQuery<CosmosDocument>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), "select * from c where c.isbidirectional = 'Yes' AND c.definition = '" + searchKeyword + "'", queryOptions);
                    CosmosDocument result = compQuery.AsEnumerable().FirstOrDefault();
                    //When bi-directional exact result found, return the keyword
                    if (result != null)
                    {
                        res.Add(result.keyword);
                    }
                    //When exact result not found, do a best match found using Levenshtein's Algorithm
                    else
                    {
                        int minValue = 30;
                        string interMediate = string.Empty;
                        IQueryable<CosmosDocument> filtercompQuery = client.CreateDocumentQuery<CosmosDocument>(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), "select * from c where c.isbidirectional = 'Yes'", queryOptions);
                        IEnumerable<CosmosDocument> filterresult = filtercompQuery.AsEnumerable();
                        List<CosmosDocument> final = filterresult.ToList();
                        
                        CosmosDocument bestmatch = final.Where(f => f.definition.ToLower().Contains(searchKeyword.ToLower())).AsEnumerable().FirstOrDefault();
                        if (bestmatch != null)
                        {
                            res.Add(bestmatch.category == "Holiday" ? bestmatch.keyword + "-2018" : bestmatch.keyword);
                        }
                        else
                        {
                            foreach (CosmosDocument cmp in final)
                            {
                                int dis = StringDistance.LevenshteinDistance(searchKeyword, cmp.definition);
                                if (dis < minValue)
                                {
                                    minValue = dis;
                                    interMediate = cmp.category == "Holiday" ? cmp.keyword + "-2018" : cmp.keyword;
                                }
                            }
                            if(minValue < 7) res.Add(interMediate);
                        }

                    }
                }
            }
            catch (Exception e)
            { }

            return res;
        }
        /// <summary>
        /// This function is used to Update a document's one property value at a time
        /// </summary>
        /// <param name="proptoUpdate">property value to be updated</param>
        /// <param name="valuetoUpdate">value to update</param>
        /// <param name="keyword">keyword whose property has to be updated</param>
        /// <returns>boolean value indicating the success of update</returns>
        public static async Task<bool> UpdateDocument(string proptoUpdate, string valuetoUpdate, string keyword)
        {
            //Get, or Create, the Database
            var database = await GetOrCreateDatabaseAsync(databaseId);

            //Get, or Create, the Document Collection
            var collection = await GetOrCreateCollectionAsync(database.SelfLink, collectionId);

            //Find the document in the DB
            Document doc = client.CreateDocumentQuery<Document>(collection.SelfLink).Where(d => d.Id.ToLower() == keyword.ToLower()).AsEnumerable().FirstOrDefault();
            JObject outputJson = new JObject();
            if (doc != null)
            {
                //Update the corresponding property with the user's value
                outputJson = JObject.Parse(doc.ToString());
                outputJson[proptoUpdate] = valuetoUpdate;
            }
            try
            {
                Document updatedDoc = await client.ReplaceDocumentAsync(doc.SelfLink, outputJson);
                if (updatedDoc.Id != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static async Task<string> GetHolidayList()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //Get Client Initialized
                InitializeCosmosClient();

                //Get, or Create, the Database
                var database = await GetOrCreateDatabaseAsync(databaseId);

                //Get, or Create, the Document Collection
                var collection = await GetOrCreateCollectionAsync(database.SelfLink, collectionId);
                FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
                IQueryable<CosmosDocument> filtercompQuery = client.CreateDocumentQuery<CosmosDocument>(
                            UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), "select * from c where c.category = 'Holiday'", queryOptions);
                IEnumerable<CosmosDocument> filterresult = filtercompQuery.AsEnumerable();
                List<CosmosDocument> final = filterresult.ToList();
                foreach (CosmosDocument cd in final)
                {
                    sb.Append(cd.definition + "/" + cd.keyword + "-2018\n\n");
                }
            }
            catch (Exception e)
            {

            }
            return sb.ToString();
        }
    }
    public class CosmosDocument
    {
        public string keyword { get; set; }
        public string definition { get; set; }
        public string category { get; set; }
        public string approvedind { get; set; }
        public string userid { get; set; }
        public string id { get; set; }
        public List<Homogen> homogens { get; set; }
        public string isbidirectional { get; set; }
    }
    public class Homogen
    {
        public string keyword { get; set; }
        public string definition { get; set; }
        public string category { get; set; }
        public string approvedind { get; set; }
        public string userid { get; set; }
    }
}