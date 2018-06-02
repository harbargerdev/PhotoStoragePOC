﻿using Amazon.SecurityToken.Model;
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
                string hashKey = (username + fileName).GetHashCode().ToString();

                List<DocumentEntity> entities = context.Query<DocumentEntity>(hashKey).ToList<DocumentEntity>();

                if (entities.Count > 0)
                    entity = entities.FirstOrDefault<DocumentEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
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