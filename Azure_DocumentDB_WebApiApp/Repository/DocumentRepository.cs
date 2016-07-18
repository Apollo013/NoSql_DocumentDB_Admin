﻿using Azure_DocumentDB_WebApiApp.Models.ViewModels;
using Azure_DocumentDB_WebApiApp.Repository.Abstract;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Azure_DocumentDB_WebApiApp.Repository
{
    /// <summary>
    /// Handles all operations in relation to Document's
    /// </summary>
    /// <typeparam name="T">The Type/schema of document we are dealing with</typeparam>
    public class DocumentRepository : RepositoryBase
    {
        #region CONSTRUCTORS
        public DocumentRepository(DocumentClient client) : base(client) { }
        #endregion

        #region DOCUMENT METHODS
        /// <summary>
        /// Creates a new Document
        /// </summary>
        /// <param name="dbid">database id</param>
        /// <param name="colid">collection id</param>
        /// <param name="document">The document to persist</param>
        /// <returns></returns>
        public async Task CreateDocumentAsync(string dbid, string colid, Document document)
        {
            Check(dbid, colid, document.Id);

            try
            {
                await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(dbid, colid, document.Id));
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbid, colid), document);
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Replaces a document
        /// </summary>
        /// <param name="dbid">database id</param>
        /// <param name="colid">collection id</param>
        /// <param name="document">The document to replace</param>
        /// <returns></returns>
        public async Task ReplaceDocumentAsync(string dbid, string colid, Document document)
        {
            Check(dbid, colid, document.Id);
            await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentCollectionUri(dbid, colid), document);
        }

        /// <summary>
        /// Deletes a document
        /// </summary>
        /// <param name="dbid">database id</param>
        /// <param name="colid">collection id</param>
        /// <param name="docid">The id of the document to delete</param>
        /// <returns></returns>
        public async Task DeleteDocumentAsync(string dbid, string colid, string docid)
        {
            Check(dbid, colid, docid);

            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(dbid, colid, docid));
            }
            catch (DocumentClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a document
        /// </summary>
        /// <param name="dbid">database id</param>
        /// <param name="colid">collection id</param>
        /// <param name="docid">The id of the document to get</param>
        /// <returns></returns>
        public async Task<Document> GetDocumentAsync(string dbid, string colid, string docid)
        {
            Check(dbid, colid, docid);
            try
            {
                return await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(dbid, colid, docid));
            }
            catch (DocumentClientException)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a list of documents (limited to ten docs)
        /// </summary>
        /// <param name="dbid">database id</param>
        /// <param name="colid">collection id</param>
        /// <returns></returns>
        public async Task<IEnumerable<ItemVM>> GetDocumentDetailsAsync(string dbid, string colid)
        {
            List<ItemVM> list = new List<ItemVM>();

            foreach (var doc in await Client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri(dbid, colid), new FeedOptions { MaxItemCount = 10 }))
            {
                list.Add(ModelFactory.Create(doc));
            }
            return list;
        }

        /// <summary>
        /// Check parameters prior to processing
        /// </summary>
        /// <param name="dbid"></param>
        /// <param name="colid"></param>
        /// <param name="docid"></param>
        private void Check(string dbid, string colid, string docid)
        {
            // Check parameters
            dbid.Check("No valid database id provided");
            colid.Check("No valid collection id provided");
            docid.Check("No valid document id provided");
        }




        /*
        /// <summary>
        /// Repalces a Document
        /// </summary>
        /// <param name="document">The document to replace</param>
        /// <returns></returns>
        public async Task ReplaceDocumentAsync(T document)
        {
                        // Check parameters
            dbid.Check("No valid database id provided");
            colid.Check("No valid collection id provided");

            try
            {
                await Client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(dbid, colid));
            }
            catch (DocumentClientException)
            {
                throw;
            }

        }

        public async Task DeleteDocumentAsync(DocumentDM documentDetails)
        {
            // Check that the document already exists
            var doc = Task.Factory.StartNew(() => { return GetDocument(documentDetails); });

            if (doc.Result != null)
            {
                await Client.DeleteDocumentAsync(doc.Result.SelfLink);
            }
        }

        /// <summary>
        /// Get a list of T, with an optional predicate
        /// </summary>
        /// <param name="predicate">The linq expression Where clause</param>
        /// <returns>An IEnumerable of T</returns>
        public async Task<IEnumerable<T>> GetDocumentAsync(DocumentDM documentDetails, Expression<Func<T, bool>> predicate = null)
        {
            // Make sure the collection exists (Will also ensure that the database exists)
            //Task.Run(() => CreateCollectionAsync(documentDetails.Collection)).Wait();

            IDocumentQuery<T> query;

            // Check if a predicate was provided to filter results
            if (predicate != null)
            {
                query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                    .Where(predicate)
                    .AsDocumentQuery();
            }
            else
            {
                query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                    .AsDocumentQuery();
            }

            // Process results
            List<T> results = new List<T>();

            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            // Return
            return results;
        }


        /// <summary>
        /// Gets a document
        /// </summary>
        /// <param name="documentDetails">Details necessary to retrieve the document</param>
        /// <returns>A Document object</returns>
        public Document GetDocument(DocumentDM documentDetails)
        {
            if (documentDetails == null)
            {
                throw new ArgumentNullException("Please specify valid document collection properties");
            }
            else if (String.IsNullOrEmpty(documentDetails.Id))
            {
                throw new ArgumentNullException("Please specify valid name for the document");
            }

            // Make sure the collection exists (Will also ensure that the database exists)
            //Task.Run(() => CreateCollectionAsync(documentDetails.Collection)).Wait();

            return Client.CreateDocumentQuery<Document>(
                Collection.SelfLink,
                $"SELECT * FROM c WHERE c.id = '{documentDetails.Id}'")
                .AsEnumerable()
                .FirstOrDefault();
        }
        */
        #endregion
    }
}