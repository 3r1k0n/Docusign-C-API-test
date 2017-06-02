using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.Net;

namespace TestProj
{
    class CoreRecipes
    {
        // Integrator Key (aka API key) is needed to authenticate your API calls.  This is an application-wide key
        private string INTEGRATOR_KEY = "a2dced5d-bebe-4b70-803e-bbbb70ecf7cb";

        //////////////////////////////////////////////////////////
        // Main()
        //////////////////////////////////////////////////////////
        static void Main(string[] args)
        {
            CoreRecipes recipes = new CoreRecipes();

            //*** TEST 0 - Trying out the REST API
            //recipes.testRestApi();

            //*** TEST 1 - Request Signature (on local document)            
            //EnvelopeSummary envSummary = recipes.requestSignatureOnDocumentTest();

            //*** TEST 2 - Request Signature (from Template)
            EnvelopeSummary envSummary = recipes.requestSignatureFromTemplateTest();

            //*** TEST 3 - Get Envelope Information           
            //Envelope env = recipes.getEnvelopeInformationTest();

            //*** TEST 4 - Get Recipient Information
            //Recipients recips = recipes.listRecipientsTest();

            //*** TEST 5 - List Envelopes           
            //EnvelopesInformation envelopes = recipes.listEnvelopesTest();

            //*** TEST 6 - Download Envelope Documents
            //recipes.listDocumentsAndDownloadTest();

            //*** TEST 7 - Embedded Sending            
            //ViewUrl senderView = recipes.createEmbeddedSendingViewTest();

            //*** TEST 8 - Embedded Signing 
            //ViewUrl recipientView = recipes.createEmbeddedSigningViewTest();

            //*** TEST 9 - Embedded DocuSign Console          
            //ViewUrl consoleView = recipes.createEmbeddedConsoleViewTest();

            Console.Read();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void testRestApi()
        {
            throw new NotImplementedException();
        }
        public EnvelopeSummary requestSignatureOnDocumentTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "epercic@ripcordtech.ca";
            string password = "epercic";

            // the document (file) we want signed
            const string SignTest1File = @"C:\Users\Erik Perčić\Documents\testDocusignTemplate.pdf";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Signature Request (AKA create & send Envelope)
            //===========================================================

            // Read a file from disk to use as a document.
            byte[] fileBytes = File.ReadAllBytes(SignTest1File);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc - From file";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = "TestFile.pdf";
            doc.DocumentId = "3";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the document
            Signer signer1 = new Signer();
            signer1.Email = "3r1k0n@gmail.com";
            signer1.Name = "Recipient Uno";
            signer1.RecipientId = "1";

            // Add a recipient to sign the document
            Signer signer2 = new Signer();
            signer2.Email = "erik.percic@gmail.com";
            signer2.Name = "Recipient Due";
            signer2.RecipientId = "2";

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer1.Tabs = new Tabs();
            signer1.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere = new SignHere();
            signHere.DocumentId = "3";
            signHere.TabLabel = "Uno";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "100";
            signHere.YPosition = "100";
            signer1.Tabs.SignHereTabs.Add(signHere);

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer2.Tabs = new Tabs();
            signer2.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere2 = new SignHere();
            signHere2.DocumentId = "3";
            signHere2.TabLabel = "Due";
            signHere2.Name = "Dva";
            signHere2.PageNumber = "1";
            signHere2.RecipientId = "2";
            signHere2.XPosition = "200";
            signHere2.YPosition = "200";
            signer2.Tabs.SignHereTabs.Add(signHere2);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer1);
            envDef.Recipients.Signers.Add(signer2);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            // print the JSON response
            Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));

            return envelopeSummary;

        } // end requestSignatureTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EnvelopeSummary requestSignatureFromTemplateTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "epercic@ripcordtech.ca";
            string password = "epercic";

            // Enter recipient (signer) name and email address
            string recipientName = "Erik Percic";
            string recipientEmail = "3r1k0n@gmail.com";

            string recipientName2 = "Suleima Diaz";
            string recipientEmail2 = "erik.percic@gmail.com";

            // the document (file) we want signed
            string templateId = "37c5956a-2fab-447e-995c-fb3733fb866d";
            string templateRoleName = "Primary Signer";
            string templateRoleName2 = "Adult 2";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Signature Request from Template 
            //===========================================================

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc - Template 2";

            // assign recipient to template role by setting name, email, and role name.  Note that the
            // template role name must match the placeholder role name saved in your account template.  
            TemplateRole tRole = new TemplateRole();
            tRole.Email = recipientEmail;
            tRole.Name = recipientName;
            tRole.RoleName = templateRoleName;

            tRole.Tabs = new Tabs();
            tRole.Tabs.TextTabs = new List<Text>();

            Text rent_amount_tab = new Text();
            rent_amount_tab.TabLabel = "\\*rent_amount";
            rent_amount_tab.Value = "$846";

            Text property_address_tab = new Text();
            property_address_tab.TabLabel = "\\*property_address";
            property_address_tab.Value = "123 Lakedrive Rd.";

            Text other_occupants_tab = new Text();
            other_occupants_tab.TabLabel = "\\*other_occupants";
            other_occupants_tab.Value = "Suleima Diaz, Kim Sabo, Jackie Salas";

            tRole.Tabs.TextTabs.Add(rent_amount_tab);
            tRole.Tabs.TextTabs.Add(property_address_tab);
            tRole.Tabs.TextTabs.Add(other_occupants_tab);

            TemplateRole tRole2 = new TemplateRole();
            tRole2.Email = recipientEmail2;
            tRole2.Name = recipientName2;
            tRole2.RoleName = templateRoleName2;

            List<TemplateRole> rolesList = new List<TemplateRole>() { tRole };
            rolesList.Add(tRole2);

            // add the role to the envelope and assign valid templateId from your account
            envDef.TemplateRoles = rolesList;
            envDef.TemplateId = templateId;

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            // print the JSON response
            Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));

            return envelopeSummary;

        } // end requestSignatureFromTemplateTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Envelope getEnvelopeInformationTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // provide a valid envelope ID from your account.  
            string envelopeId = "[ENVELOPE_ID]]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Get Envelope Information
            //===========================================================

            // |EnvelopesApi| contains methods related to creating and sending Envelopes including status calls
            EnvelopesApi envelopesApi = new EnvelopesApi();
            Envelope envInfo = envelopesApi.GetEnvelope(accountId, envelopeId);

            // print the JSON response
            Console.WriteLine("EnvelopeInformation:\n{0}", JsonConvert.SerializeObject(envInfo));

            return envInfo;
        } // end requestSignatureFromTemplateTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Recipients listRecipientsTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // provide a valid envelope ID from your account.  
            string envelopeId = "[ENVELOPE_ID]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: List Envelope Recipients
            //===========================================================

            // |EnvelopesApi| contains methods related to envelopes and envelope recipients
            EnvelopesApi envelopesApi = new EnvelopesApi();
            Recipients recips = envelopesApi.ListRecipients(accountId, envelopeId);

            // print the JSON response
            Console.WriteLine("Recipients:\n{0}", JsonConvert.SerializeObject(recips));

            return recips;

        } // end getRecipientInformationTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public EnvelopesInformation listEnvelopesTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: List Envelopes (using filters)
            //===========================================================

            // This example gets statuses of all envelopes in your account going back 1 full month...
            DateTime fromDate = DateTime.UtcNow;
            fromDate = fromDate.AddDays(-30);
            string fromDateStr = fromDate.ToString("o");

            // set a filter for the envelopes we want returned using the fromDate and count properties
            EnvelopesApi.ListStatusChangesOptions options = new EnvelopesApi.ListStatusChangesOptions()
            {
                count = "10",
                fromDate = fromDateStr
            };

            // |EnvelopesApi| contains methods related to envelopes and envelope recipients
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopesInformation envelopes = envelopesApi.ListStatusChanges(accountId, options);

            // print the JSON response
            Console.WriteLine("Envelopes:\n{0}", JsonConvert.SerializeObject(envelopes));

            return envelopes;

        } // end listEnvelopesTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void listDocumentsAndDownloadTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // provide a valid envelope ID from your account.  
            string envelopeId = "[ENVELOPE_ID]]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: List Envelope Document(s)
            //===========================================================

            // |EnvelopesApi| contains methods related to envelopes and envelope recipients
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeDocumentsResult docsList = envelopesApi.ListDocuments(accountId, envelopeId);

            // print the JSON response
            Console.WriteLine("EnvelopeDocumentsResult:\n{0}", JsonConvert.SerializeObject(docsList));

            //===========================================================
            // Step 3: Download Envelope Document(s)
            //===========================================================

            // read how many documents are in the envelope
            int docCount = docsList.EnvelopeDocuments.Count;
            string filePath = null;
            FileStream fs = null;

            // loop through the envelope's documents and download each doc
            for (int i = 0; i < docCount; i++)
            {
                // GetDocument() API call returns a MemoryStream
                MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(accountId, envelopeId, docsList.EnvelopeDocuments[i].DocumentId);
                // let's save the document to local file system
                filePath = Path.GetTempPath() + Path.GetRandomFileName() + ".pdf";
                fs = new FileStream(filePath, FileMode.Create);
                docStream.Seek(0, SeekOrigin.Begin);
                docStream.CopyTo(fs);
                fs.Close();
                Console.WriteLine("Envelope Document {0} has been downloaded to:  {1}", i, filePath);
            }

        } // end listDocumentsAndDownloadTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewUrl createEmbeddedSendingViewTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // Enter recipient (signer) name and email address
            string recipientName = "[RECIPIENT_NAME]";
            string recipientEmail = "[RECIPIENT_EMAIL]";

            // the document (file) we want signed
            const string SignTest1File = @"[PATH/TO/DOCUMENT/TEST.PDF]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Create A Draft Envelope
            //===========================================================

            // Read a file from disk to use as a document.
            byte[] fileBytes = File.ReadAllBytes(SignTest1File);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = "TestFile.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the documeent
            Signer signer = new Signer();
            signer.Email = recipientEmail;
            signer.Name = recipientName;
            signer.RecipientId = "1";

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "100";
            signHere.YPosition = "100";
            signer.Tabs.SignHereTabs.Add(signHere);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // must set status to "created" since we cannot open Sender View on an Envelope that has already been sent, only on draft envelopes
            envDef.Status = "created";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            //===========================================================
            // Step 3: Create Embedded Sending View (URL)
            //===========================================================

            ReturnUrlRequest options = new ReturnUrlRequest();
            options.ReturnUrl = "https://www.docusign.com/devcenter";

            // generate the embedded sending URL
            ViewUrl senderView = envelopesApi.CreateSenderView(accountId, envelopeSummary.EnvelopeId, options);

            // print the JSON response
            Console.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(senderView));

            // Start the embedded sending session
            System.Diagnostics.Process.Start(senderView.Url);

            return senderView;

        } // end createEmbeddedSendingViewTest()


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewUrl createEmbeddedSigningViewTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // Enter recipient (signer) name and email address
            string recipientName = "[RECIPIENT_NAME]";
            string recipientEmail = "[RECIPIENT_EMAIL]";

            // the document (file) we want signed
            const string SignTest1File = @"[PATH/TO/DOCUMENT/TEST.PDF]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Create and Send an Envelope with Embedded Recipient
            //===========================================================

            // Read a file from disk to use as a document.
            byte[] fileBytes = File.ReadAllBytes(SignTest1File);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = "TestFile.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the documeent
            Signer signer = new Signer();
            signer.Email = recipientEmail;
            signer.Name = recipientName;
            signer.RecipientId = "1";
            signer.ClientUserId = "1234"; // must set |clientUserId| to embed the recipient!

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "100";
            signHere.YPosition = "100";
            signer.Tabs.SignHereTabs.Add(signHere);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            //===========================================================
            // Step 3: Create Embedded Signing View (URL)
            //===========================================================

            RecipientViewRequest viewOptions = new RecipientViewRequest()
            {
                ReturnUrl = "https://www.docusign.com/devcenter",
                ClientUserId = "1234",  // must match clientUserId set in step #2!
                AuthenticationMethod = "email",
                UserName = envDef.Recipients.Signers[0].Name,
                Email = envDef.Recipients.Signers[0].Email
            };

            // create the recipient view (aka signing URL)
            ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeSummary.EnvelopeId, viewOptions);

            // print the JSON response
            Console.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(recipientView));

            // Start the embedded signing session
            System.Diagnostics.Process.Start(recipientView.Url);

            return recipientView;

        } // end createEmbeddedSigningViewTest()

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewUrl createEmbeddedConsoleViewTest()
        {
            // Enter your DocuSign credentials below.  Note: You only need a DocuSign account to SEND documents,
            // signing is always free and signers do not need an account.
            string username = "[EMAIL]";
            string password = "[PASSWORD]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            configureApiClient("https://demo.docusign.net/restapi");

            //===========================================================
            // Step 1: Login()
            //===========================================================

            // call the Login() API which sets the user's baseUrl and returns their accountId
            string accountId = loginApi(username, password);

            //===========================================================
            // Step 2: Create Embedded Console View (URL)
            //===========================================================

            ReturnUrlRequest urlRequest = new ReturnUrlRequest();
            urlRequest.ReturnUrl = "https://www.docusign.com/devcenter";

            // Adding the envelopeId start sthe console with the envelope open
            EnvelopesApi envelopesApi = new EnvelopesApi();
            ViewUrl viewUrl = envelopesApi.CreateConsoleView(accountId, null);

            // Start the embedded signing session.
            System.Diagnostics.Process.Start(viewUrl.Url);

            return viewUrl;

        } // end createEmbeddedConsoleViewTest()




        //**********************************************************************************************************************
        //**********************************************************************************************************************
        //*  HELPER FUNCTIONS
        //**********************************************************************************************************************
        //**********************************************************************************************************************
        public void configureApiClient(string basePath)
        {
            // instantiate a new api client
            ApiClient apiClient = new ApiClient(basePath);

            // set client in global config so we don't need to pass it to each API object.
            Configuration.Default.ApiClient = apiClient;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string loginApi(string usr, string pwd)
        {
            // we set the api client in global config when we configured the client 
            ApiClient apiClient = Configuration.Default.ApiClient;
            string authHeader = "{\"Username\":\"" + usr + "\", \"Password\":\"" + pwd + "\", \"IntegratorKey\":\"" + INTEGRATOR_KEY + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

            // find the default account for this user
            foreach (LoginAccount loginAcct in loginInfo.LoginAccounts)
            {
                if (loginAcct.IsDefault == "true")
                {
                    accountId = loginAcct.AccountId;
                    break;
                }
            }
            if (accountId == null)
            { // if no default found set to first account
                accountId = loginInfo.LoginAccounts[0].AccountId;
            }
            return accountId;
        }

        // Returns JSON string
        private string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        // POST a JSON string
        private void POST(string url, string jsonContent)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(jsonContent);

            request.ContentLength = byteArray.Length;
            request.ContentType = @"application/json";

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            long length = 0;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    length = response.ContentLength;
                }
            }
            catch (WebException ex)
            {
                // Log exception and throw as for GET example above
            }
        }

    } // end class
} // end namespace
