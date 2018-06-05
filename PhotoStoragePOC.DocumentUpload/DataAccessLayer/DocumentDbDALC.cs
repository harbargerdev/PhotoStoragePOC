using Amazon.SecurityToken.Model;
using PhotoStoragePOC.DocumentUpload.Entities;
using PhotoStoragePOC.DocumentUpload.STS;
using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace PhotoStoragePOC.DocumentUpload.DataAccessLayer
{
    public class DocumentDbDALC
    {
        private static string DEFAULT_TABLE_NAME = "photostoragepoc";

        private string AccessKey { get; set; }
        private string SecretKey { get; set; }
        private string SessionToken { get; set; }
        private string Region { get; set; }
        private AmazonDynamoDBClient DynamoClient { get; set; }

        public DocumentDbDALC(string key, string secret)
        {
            StsUtility stsUtility = new StsUtility();
            Credentials credentials = stsUtility.GetSessionToken(key, secret);

            AccessKey = credentials.AccessKeyId;
            SecretKey = credentials.SecretAccessKey;
            SessionToken = credentials.SessionToken;
        }

        public DocumentDbDALC(string key, string secret, string sessionToken)
        {
            AccessKey = key;
            SecretKey = secret;
            SessionToken = sessionToken;
        }

        #region Public Methods

        public DocumentEntity InsertDocumentRecord(DocumentEntity entity)
        {
            SetDynamoDBClient();

            DynamoDBContext context = new DynamoDBContext(DynamoClient);

            try
            {
                entity.ID = (entity.DocumentOwner + entity.FileName).GetHashCode();
                context.Save<DocumentEntity>(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            return entity;
        }

        public DocumentEntity UpdateDocumentRecord(DocumentEntity entity)
        {
            SetDynamoDBClient();

            DynamoDBContext context = new DynamoDBContext(DynamoClient);

            try
            {
                context.Save<DocumentEntity>(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
        }

        public DocumentEntity GetDocumentRecord(string username, string fileName)
        {
            DocumentEntity entity = null;

            SetDynamoDBClient();

            DynamoDBContext context = new DynamoDBContext(DynamoClient);

            try
            {
                Table table = Table.LoadTable(DynamoClient, DEFAULT_TABLE_NAME);

                int hashKey = (username + fileName).GetHashCode();
                QueryFilter filter = new QueryFilter("ID", QueryOperator.Equal, hashKey);

                QueryOperationConfig queryConfig = new QueryOperationConfig()
                {
                    Limit = 1,
                    Select = SelectValues.AllAttributes,
                    Filter = filter
                };

                Search search = table.Query(queryConfig);

                List<Document> documents = new List<Document>();
                List<DocumentEntity> entities = new List<DocumentEntity>();

                do
                {
                    documents = search.GetNextSet();
                    foreach(var document in documents)
                    {
                        string json = document.ToJson();
                        DocumentEntity newEntity = JsonConvert.DeserializeObject<DocumentEntity>(json);
                        entities.Add(newEntity);
                    }
                } while (!search.IsDone);

                if (entities.Count > 0)
                    entity = entities.FirstOrDefault<DocumentEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
        }

        public bool DeleteDocumentRecord(DocumentEntity document)
        {
            bool status = true;

            SetDynamoDBClient();

            DynamoDBContext context = new DynamoDBContext(DynamoClient);

            try
            {
                context.Delete<DocumentEntity>(document);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return status;
        }

        public List<DocumentEntity> GetAllUserDocuments(string username)
        {
            List<DocumentEntity> documents = null;

            SetDynamoDBClient();

            try
            {
                DynamoDBContext context = new DynamoDBContext(DynamoClient);

                //List<QueryFilter> query = new List<QueryFilter>() { new QueryFilter("", QueryOperator.Equal, username) };
                List<ScanCondition> conditions = new List<ScanCondition>() { new ScanCondition("DocumentOwner", ScanOperator.Equal, username) };


                DynamoDBOperationConfig operation = new DynamoDBOperationConfig()
                {
                    IndexName = "DocumentOwner-index",
                    QueryFilter = conditions
                };

                BatchGet<DocumentEntity> results = context.CreateBatchGet<DocumentEntity>(operation);
                results.Execute();

                if(results.Results != null && results.Results.Count > 0)
                {
                    foreach (DocumentEntity document in results.Results)
                        documents.Add(document);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return documents;
        }

        #endregion
        
        #region Private Methods

        private void SetDynamoDBClient()
        {
            if(DynamoClient == null)
            {
                AmazonDynamoDBConfig config = new AmazonDynamoDBConfig() { RegionEndpoint = RegionEndpoint.USEast1 };

                DynamoClient = new AmazonDynamoDBClient(AccessKey, SecretKey, SessionToken, config);
            }
        }
        
        #endregion
    }
}
